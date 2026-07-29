param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$OutputRoot = "dist\publisher",
    [string]$Version = ""
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repoRoot "src\Publisher.Cli\Vmf.Publisher.Cli.csproj"
$workRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("vmf-publisher-package-" + [Guid]::NewGuid().ToString("N"))
$publishRoot = Join-Path $workRoot "publish"
$stageRoot = Join-Path $workRoot "stage"
$distRoot = Join-Path $repoRoot $OutputRoot
$stamp = if ([string]::IsNullOrWhiteSpace($Version)) { "local" } else { $Version }
$zipPath = Join-Path $distRoot "vmf-publisher-$Runtime-$stamp.zip"
$shaPath = "$zipPath.sha256"
$fixedTimestamp = [DateTimeOffset]::new(2026, 1, 1, 0, 0, 0, [TimeSpan]::Zero)

function Remove-DirectoryIfExists([string]$Path) {
    if (Test-Path -LiteralPath $Path) {
        Remove-Item -LiteralPath $Path -Recurse -Force
    }
}

function Assert-PathInsideRepository([string]$Path) {
    $resolved = [System.IO.Path]::GetFullPath($Path)
    $root = [System.IO.Path]::GetFullPath($repoRoot)
    if (!$resolved.StartsWith($root, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to write outside repository: $resolved"
    }
}

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

function New-DeterministicZip([string]$SourceRoot, [string]$DestinationPath) {
    Add-Type -AssemblyName System.IO.Compression
    Add-Type -AssemblyName System.IO.Compression.FileSystem

    if (Test-Path -LiteralPath $DestinationPath) {
        [System.IO.File]::Delete($DestinationPath)
    }

    $files = Get-ChildItem -LiteralPath $SourceRoot -File -Recurse |
        Sort-Object { $_.FullName.Substring($SourceRoot.Length).Replace("\", "/") }

    $zip = [System.IO.Compression.ZipFile]::Open($DestinationPath, [System.IO.Compression.ZipArchiveMode]::Create)
    try {
        foreach ($file in $files) {
            $relativePath = $file.FullName.Substring($SourceRoot.Length).TrimStart("\", "/").Replace("\", "/")
            $entry = $zip.CreateEntry($relativePath, [System.IO.Compression.CompressionLevel]::Optimal)
            $entry.LastWriteTime = $fixedTimestamp
            $entryStream = $entry.Open()
            $fileStream = [System.IO.File]::OpenRead($file.FullName)
            try {
                $fileStream.CopyTo($entryStream)
            }
            finally {
                $fileStream.Dispose()
                $entryStream.Dispose()
            }
        }
    }
    finally {
        $zip.Dispose()
    }
}

Assert-PathInsideRepository $distRoot
try {
    New-Item -ItemType Directory -Force -Path $distRoot | Out-Null
    Remove-DirectoryIfExists $publishRoot
    Remove-DirectoryIfExists $stageRoot
    New-Item -ItemType Directory -Force -Path $stageRoot | Out-Null

    dotnet publish $projectPath --configuration $Configuration --runtime $Runtime --self-contained false --output $publishRoot

    $excludedFiles = @(
        "appsettings.json",
        "appsettings.local.json"
    )

    foreach ($file in Get-ChildItem -LiteralPath $publishRoot -File) {
        if ($excludedFiles -contains $file.Name) {
            continue
        }

        $targetName = if ($file.Name -eq "vmf-publisher.exe") { "publisher.exe" } else { $file.Name }
        Copy-Item -LiteralPath $file.FullName -Destination (Join-Path $stageRoot $targetName)
    }

    $requiredFiles = @(
        "publisher.exe",
        "vmf-publisher.dll",
        "vmf-publisher.deps.json",
        "vmf-publisher.runtimeconfig.json",
        "Vmf.Publisher.dll"
    )

    foreach ($requiredFile in $requiredFiles) {
        $path = Join-Path $stageRoot $requiredFile
        if (!(Test-Path -LiteralPath $path -PathType Leaf)) {
            throw "Required package file is missing: $requiredFile"
        }
    }

    Assert-NoSecretMaterial $stageRoot
    if (Test-Path -LiteralPath $shaPath) {
        [System.IO.File]::Delete($shaPath)
    }

    New-DeterministicZip $stageRoot $zipPath
    $hash = (Get-FileHash -LiteralPath $zipPath -Algorithm SHA256).Hash.ToLowerInvariant()
    Set-Content -LiteralPath $shaPath -Encoding ASCII -NoNewline -Value "$hash  $(Split-Path -Leaf $zipPath)"

    & (Join-Path $PSScriptRoot "verify-package.ps1") -PackagePath $zipPath

    Write-Host "Package: $zipPath"
    Write-Host "SHA-256: $shaPath"
}
finally {
    Remove-DirectoryIfExists $workRoot
}
