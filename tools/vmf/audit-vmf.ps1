# Audits generated VMF v1.0 project structure against spec/vmf/manifest.yaml.

param(
    [string]$ManifestPath,
    [string]$OutputRoot
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$workspaceRoot = Resolve-Path (Join-Path $scriptDir "..\..")

if ([string]::IsNullOrWhiteSpace($ManifestPath)) {
    $ManifestPath = Join-Path $workspaceRoot "spec\vmf\manifest.yaml"
}
if ([string]::IsNullOrWhiteSpace($OutputRoot)) {
    $OutputRoot = Join-Path $workspaceRoot "src\VMF"
}

if (-not (Test-Path $ManifestPath)) {
    Write-Error "Manifest not found: $ManifestPath"
    exit 1
}

if (-not (Test-Path $OutputRoot)) {
    Write-Error "Output root not found: $OutputRoot"
    exit 1
}

$requiredLayers = @("Common", "Core", "Domain", "Application", "Infrastructure", "Presentation")
$failures = New-Object System.Collections.Generic.List[string]

foreach ($layer in $requiredLayers) {
    $layerPath = Join-Path $OutputRoot $layer
    if (-not (Test-Path $layerPath)) {
        $failures.Add("Missing layer directory: $layer")
        continue
    }

    $files = @(Get-ChildItem -Path $layerPath -Include *.bas,*.cls -File -Recurse)
    if ($files.Count -eq 0) {
        $failures.Add("Layer contains no generated VBA files: $layer")
    }

    foreach ($file in $files) {
        $text = Get-Content -Raw -Encoding ASCII $file.FullName
        if ($text -notmatch "Option Explicit") {
            $failures.Add("Missing Option Explicit: $($file.FullName)")
        }
        if ($text -notmatch "Layer:\s*$layer") {
            $failures.Add("Layer header mismatch: $($file.FullName)")
        }
    }
}

$candidatePath = Join-Path $workspaceRoot "spec\vmf\VMFCandidates_v1.1.md"
if (-not (Test-Path $candidatePath)) {
    $failures.Add("Missing VMF candidate file: $candidatePath")
}

if ($failures.Count -gt 0) {
    Write-Host "VMF v1.0 audit result: FAIL"
    foreach ($failure in $failures) {
        Write-Host "FAIL: $failure"
    }
    exit 2
}

Write-Host "VMF v1.0 audit result: PASS"
Write-Host "Blueprint: BuildBlueprint_v1.0.1.md"
Write-Host "Manifest: spec/vmf/manifest.yaml"
Write-Host "Release Gate: generated layers present with module headers and Option Explicit."
