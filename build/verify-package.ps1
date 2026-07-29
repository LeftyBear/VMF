param(
    [Parameter(Mandatory = $true)]
    [string]$PackagePath,
    [string]$Sha256Path = ""
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repoRoot = Split-Path -Parent $PSScriptRoot
function Resolve-InputPath([string]$Path) {
    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path (Get-Location) $Path))
}

$resolvedPackagePath = Resolve-InputPath $PackagePath
if ([string]::IsNullOrWhiteSpace($Sha256Path)) {
    $Sha256Path = "$resolvedPackagePath.sha256"
}
else {
    $Sha256Path = Resolve-InputPath $Sha256Path
}

$extractRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("vmf-publisher-package-" + [Guid]::NewGuid().ToString("N"))

function Test-TextFile([string]$Path) {
    $extension = [System.IO.Path]::GetExtension($Path)
    if ($extension -in @(".json", ".config", ".xml", ".txt", ".md", ".ps1", ".cmd", ".bat")) {
        return $true
    }

    return $false
}

function Assert-NoSecretMaterial([string]$Root) {
    $blockedNames = @(
        "appsettings.local.json",
        "credentials.json",
        "client_secret.json",
        "token.json"
    )

    $blockedNamePatterns = @(
        "(^|[._-])secret([._-]|$)",
        "(^|[._-])token([._-]|$)",
        "\.p12$",
        "\.pem$",
        "\.key$"
    )

    $contentPatterns = @(
        "BEGIN (RSA |EC |OPENSSH )?PRIVATE KEY",
        '"private_key"\s*:',
        '"client_secret"\s*:',
        '"refresh_token"\s*:',
        '"access_token"\s*:',
        '"TokenStorePath"\s*:',
        '"CredentialsPath"\s*:',
        "C:\\Secrets\\"
    )

    foreach ($file in Get-ChildItem -LiteralPath $Root -File -Recurse) {
        $name = $file.Name
        if ($blockedNames -contains $name) {
            throw "Secret-like file is not allowed in package: $($file.FullName)"
        }

        foreach ($pattern in $blockedNamePatterns) {
            if ($name -match $pattern) {
                throw "Secret-like file name is not allowed in package: $($file.FullName)"
            }
        }

        if (Test-TextFile $file.FullName) {
            $content = Get-Content -LiteralPath $file.FullName -Raw -Encoding UTF8
            foreach ($pattern in $contentPatterns) {
                if ($content -match $pattern) {
                    throw "Secret-like content is not allowed in package: $($file.FullName)"
                }
            }
        }
    }
}

function Assert-ZipEntrySafe([string]$EntryName) {
    if ([string]::IsNullOrWhiteSpace($EntryName)) {
        throw "Package contains an empty ZIP entry name."
    }

    if ($EntryName.StartsWith("/", [StringComparison]::Ordinal) -or
        $EntryName.StartsWith("\", [StringComparison]::Ordinal) -or
        $EntryName.Contains("..")) {
        throw "Package contains an unsafe ZIP entry: $EntryName"
    }
}

function Invoke-HelpSmokeTest([string]$ExePath) {
    $startInfo = [System.Diagnostics.ProcessStartInfo]::new()
    $startInfo.FileName = $ExePath
    $startInfo.Arguments = "--help"
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true

    $process = [System.Diagnostics.Process]::new()
    $process.StartInfo = $startInfo
    if (!$process.Start()) {
        throw "Failed to start publisher.exe --help."
    }

    $stdout = $process.StandardOutput.ReadToEnd()
    $stderr = $process.StandardError.ReadToEnd()
    $process.WaitForExit()

    if ($process.ExitCode -ne 0) {
        throw "publisher.exe --help failed with exit code $($process.ExitCode). Output: $stdout $stderr"
    }

    $helpText = "$stdout`n$stderr"
    if (!$helpText.Contains("VMF Publisher") -or !$helpText.Contains("Usage:")) {
        throw "publisher.exe --help output did not contain the expected help text."
    }
}

try {
    if (!(Test-Path -LiteralPath $resolvedPackagePath -PathType Leaf)) {
        throw "Package does not exist: $resolvedPackagePath"
    }

    if (!(Test-Path -LiteralPath $Sha256Path -PathType Leaf)) {
        throw "SHA-256 file does not exist: $Sha256Path"
    }

    $actualHash = (Get-FileHash -LiteralPath $resolvedPackagePath -Algorithm SHA256).Hash.ToLowerInvariant()
    $shaContent = Get-Content -LiteralPath $Sha256Path -Raw -Encoding ASCII
    $expectedHash = (($shaContent -split "\s+")[0]).ToLowerInvariant()
    if ($actualHash -ne $expectedHash) {
        throw "SHA-256 mismatch. Expected $expectedHash but found $actualHash."
    }

    Add-Type -AssemblyName System.IO.Compression
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [System.IO.Compression.ZipFile]::OpenRead($resolvedPackagePath)
    try {
        foreach ($entry in $zip.Entries) {
            Assert-ZipEntrySafe $entry.FullName
        }
    }
    finally {
        $zip.Dispose()
    }

    New-Item -ItemType Directory -Force -Path $extractRoot | Out-Null
    [System.IO.Compression.ZipFile]::ExtractToDirectory($resolvedPackagePath, $extractRoot)

    $requiredFiles = @(
        "publisher.exe",
        "vmf-publisher.dll",
        "vmf-publisher.deps.json",
        "vmf-publisher.runtimeconfig.json",
        "Vmf.Publisher.dll"
    )

    foreach ($requiredFile in $requiredFiles) {
        $path = Join-Path $extractRoot $requiredFile
        if (!(Test-Path -LiteralPath $path -PathType Leaf)) {
            throw "Required package file is missing after extraction: $requiredFile"
        }
    }

    Assert-NoSecretMaterial $extractRoot

    Invoke-HelpSmokeTest (Join-Path $extractRoot "publisher.exe")

    Write-Host "Package verification passed: $resolvedPackagePath"
}
finally {
    if (Test-Path -LiteralPath $extractRoot) {
        Remove-Item -LiteralPath $extractRoot -Recurse -Force
    }
}
