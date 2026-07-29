param(
    [string]$Configuration = "Release",
    [string]$RuntimeIdentifier = "win-x64",
    [string]$Version = "0.0.0-dev",
    [string]$OutputDirectory = "dist\release\Publisher",
    [switch]$NoBuild
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Resolve-Path (Join-Path $PSScriptRoot "..\..")
$projectPath = Join-Path $root "src\Publisher.Cli\Vmf.Publisher.Cli.csproj"
$outputRoot = [IO.Path]::GetFullPath((Join-Path $root $OutputDirectory))
$stageRoot = Join-Path $outputRoot "_stage"
$publishRoot = Join-Path $stageRoot "publish"
$packageName = "vmf-publisher-$Version-$RuntimeIdentifier.zip"
$packagePath = Join-Path $outputRoot $packageName

if (Test-Path $stageRoot) {
    Remove-Item -LiteralPath $stageRoot -Recurse -Force
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

New-Item -ItemType Directory -Path $publishRoot -Force | Out-Null
New-Item -ItemType Directory -Path $outputRoot -Force | Out-Null

$publishArgs = @(
    "publish",
    $projectPath,
    "--configuration",
    $Configuration,
    "--runtime",
    $RuntimeIdentifier,
    "--self-contained",
    "false",
    "--output",
    $publishRoot
)

if ($NoBuild) {
    $publishArgs += "--no-build"
}

dotnet @publishArgs
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE."
}

@("appsettings.json", "appsettings.local.json") | ForEach-Object {
    $path = Join-Path $publishRoot $_
    if (Test-Path $path) {
        Remove-Item -LiteralPath $path -Force
    }
}

$files = Get-ChildItem -Path $publishRoot -File -Recurse |
    Where-Object { $_.Name -ne "package-manifest.json" } |
    Sort-Object FullName |
    ForEach-Object {
        $relativePath = Get-PackageRelativePath -BasePath $publishRoot -Path $_.FullName
        [pscustomobject]@{
            path = $relativePath
            size = $_.Length
            sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $_.FullName).Hash.ToLowerInvariant()
        }
    }

$manifest = [ordered]@{
    product = "VMF Publisher"
    packageVersion = $Version
    runtimeIdentifier = $RuntimeIdentifier
    configuration = $Configuration
    selfContained = $false
    createdAtUtc = (Get-Date).ToUniversalTime().ToString("O")
    files = @($files)
}

$manifestPath = Join-Path $publishRoot "package-manifest.json"
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $manifestPath -Encoding UTF8

if (Test-Path $packagePath) {
    Remove-Item -LiteralPath $packagePath -Force
}

Compress-Archive -Path (Join-Path $publishRoot "*") -DestinationPath $packagePath -CompressionLevel Optimal

Remove-Item -LiteralPath $stageRoot -Recurse -Force

Write-Host "Publisher package created: $packagePath"
