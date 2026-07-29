param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$OutputRoot = "dist\publisher",
    [string]$Version = ""
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repoRoot = Split-Path -Parent $PSScriptRoot
$scriptPath = Join-Path $repoRoot "tools\publisher\package-publisher.ps1"
$packageVersion = if ([string]::IsNullOrWhiteSpace($Version)) { "local" } else { $Version }

& $scriptPath `
    -Configuration $Configuration `
    -RuntimeIdentifier $Runtime `
    -OutputDirectory $OutputRoot `
    -Version $packageVersion
