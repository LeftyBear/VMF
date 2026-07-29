param(
    [Parameter(Mandatory = $true)]
    [string]$PackagePath
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

function Add-Failure {
    param(
        [System.Collections.Generic.List[string]]$Failures,
        [string]$Message
    )

    $Failures.Add($Message) | Out-Null
}

function Test-SecretLikeContent {
    param([string]$Path)

    $patterns = @(
        "client_secret",
        "refresh_token",
        "private_key",
        "-----BEGIN PRIVATE KEY-----",
        "AIza[0-9A-Za-z_-]{20,}",
        "ya29\.[0-9A-Za-z_-]+"
    )

    $text = Get-Content -Raw -Encoding UTF8 -LiteralPath $Path -ErrorAction SilentlyContinue
    if ($null -eq $text) {
        return $false
    }

    foreach ($pattern in $patterns) {
        if ($text -match $pattern) {
            return $true
        }
    }

    return $false
}

function Get-PackageRelativePath {
    param(
        [string]$BasePath,
        [string]$Path
    )

    $baseUri = [Uri](([IO.Path]::GetFullPath($BasePath).TrimEnd("\") + "\"))
    $pathUri = [Uri]([IO.Path]::GetFullPath($Path))
    return [Uri]::UnescapeDataString($baseUri.MakeRelativeUri($pathUri).ToString())
}

$fullPackagePath = [IO.Path]::GetFullPath($PackagePath)
if (-not (Test-Path $fullPackagePath)) {
    throw "Package not found: $PackagePath"
}

$failures = [System.Collections.Generic.List[string]]::new()
$tempRoot = Join-Path ([IO.Path]::GetTempPath()) ("vmf-publisher-package-" + [Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $tempRoot -Force | Out-Null

try {
    Expand-Archive -LiteralPath $fullPackagePath -DestinationPath $tempRoot -Force

    $requiredFiles = @(
        "vmf-publisher.exe",
        "vmf-publisher.dll",
        "vmf-publisher.deps.json",
        "vmf-publisher.runtimeconfig.json",
        "package-manifest.json"
    )

    foreach ($requiredFile in $requiredFiles) {
        if (-not (Test-Path (Join-Path $tempRoot $requiredFile))) {
            Add-Failure $failures "Required package file is missing: $requiredFile"
        }
    }

    foreach ($excludedConfig in @("appsettings.json", "appsettings.local.json")) {
        if (Test-Path (Join-Path $tempRoot $excludedConfig)) {
            Add-Failure $failures "Configuration file must not be included: $excludedConfig"
        }
    }

    $manifestPath = Join-Path $tempRoot "package-manifest.json"
    if (Test-Path $manifestPath) {
        $manifest = Get-Content -Raw -Encoding UTF8 -LiteralPath $manifestPath | ConvertFrom-Json
        if ($manifest.product -ne "VMF Publisher") {
            Add-Failure $failures "Package manifest product must be VMF Publisher."
        }

        if ($manifest.selfContained -ne $false) {
            Add-Failure $failures "Package manifest must declare selfContained=false."
        }

        $manifestPaths = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
        foreach ($file in @($manifest.files)) {
            $relativePath = [string]$file.path
            if ([string]::IsNullOrWhiteSpace($relativePath)) {
                Add-Failure $failures "Package manifest contains an empty file path."
                continue
            }

            if ($relativePath.Contains("..") -or [IO.Path]::IsPathRooted($relativePath)) {
                Add-Failure $failures "Package manifest contains an unsafe file path: $relativePath"
                continue
            }

            $manifestPaths.Add($relativePath) | Out-Null
            $actualPath = Join-Path $tempRoot ($relativePath.Replace("/", "\"))
            if (-not (Test-Path $actualPath)) {
                Add-Failure $failures "Manifested file is missing from package: $relativePath"
                continue
            }

            $actualFile = Get-Item -LiteralPath $actualPath
            if ($actualFile.Length -ne [int64]$file.size) {
                Add-Failure $failures "Manifested file size mismatch: $relativePath"
            }

            $actualHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $actualPath).Hash.ToLowerInvariant()
            if ($actualHash -ne ([string]$file.sha256).ToLowerInvariant()) {
                Add-Failure $failures "Manifested file hash mismatch: $relativePath"
            }
        }

        Get-ChildItem -Path $tempRoot -File -Recurse |
            Where-Object { $_.Name -ne "package-manifest.json" } |
            ForEach-Object {
                $relativePath = Get-PackageRelativePath -BasePath $tempRoot -Path $_.FullName
                if (-not $manifestPaths.Contains($relativePath)) {
                    Add-Failure $failures "Package file is not listed in manifest: $relativePath"
                }
            }
    }

    $secretNamePattern = "(?i)(credential|credentials|client_secret|token|secret|private[_-]?key)"
    Get-ChildItem -Path $tempRoot -File -Recurse | ForEach-Object {
        $relativePath = Get-PackageRelativePath -BasePath $tempRoot -Path $_.FullName
        if ($_.Name -match $secretNamePattern) {
            Add-Failure $failures "Secret-like file name is not allowed in package: $relativePath"
        }

        if ($_.Extension -in @(".json", ".config", ".txt", ".md", ".yaml", ".yml") -and
            $_.Name -ne "package-manifest.json" -and
            (Test-SecretLikeContent $_.FullName)) {
            Add-Failure $failures "Secret-like content is not allowed in package: $relativePath"
        }
    }

    if ($failures.Count -gt 0) {
        Write-Error ("Publisher package verification failed:`n" + ($failures -join "`n"))
        exit 1
    }

    Write-Host "Publisher package verification passed: $fullPackagePath"
}
finally {
    if (Test-Path $tempRoot) {
        Remove-Item -LiteralPath $tempRoot -Recurse -Force
    }
}
