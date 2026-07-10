# Audits generated VMF v1.0 project structure against specs/vmf/manifest.yaml.

param(
    [string]$ManifestPath,
    [string]$OutputRoot
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$workspaceRoot = Resolve-Path (Join-Path $scriptDir "..\..")

if ([string]::IsNullOrWhiteSpace($ManifestPath)) {
    $ManifestPath = Join-Path $workspaceRoot "specs\vmf\manifest.yaml"
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

function Read-VmfManifestItems {
    param([string]$Path)

    $items = New-Object System.Collections.Generic.List[object]
    $currentLayer = $null
    $currentType = ""
    $section = ""

    foreach ($rawLine in Get-Content -Encoding UTF8 $Path) {
        $line = $rawLine.TrimEnd()
        $trimmed = $line.Trim()

        if ([string]::IsNullOrWhiteSpace($trimmed) -or $trimmed.StartsWith("#")) {
            continue
        }

        if ($trimmed -eq "Modules:") {
            $section = "modules"
            continue
        }

        if ($section -ne "modules") {
            continue
        }

        if ($line -match "^\s{2}(\S[^:]*):\s*$") {
            $currentLayer = $matches[1].Trim()
            $currentType = ""
            continue
        }

        if ($line -match "^\s{4}(Classes|StandardModules|Enums):\s*$") {
            switch ($matches[1]) {
                "Classes" { $currentType = "ClassModule" }
                "StandardModules" { $currentType = "StandardModule" }
                "Enums" { $currentType = "EnumModule" }
            }
            continue
        }

        if ($null -ne $currentLayer -and -not [string]::IsNullOrWhiteSpace($currentType) -and $line -match "^\s{6}-\s*(\S.+)$") {
            $extension = ".bas"
            if ($currentType -eq "ClassModule") { $extension = ".cls" }
            $items.Add([pscustomobject]@{
                Layer = $currentLayer
                ModuleName = $matches[1].Trim()
                Extension = $extension
            })
        }
    }

    return $items
}

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

$manifestItems = @(Read-VmfManifestItems -Path $ManifestPath)
foreach ($item in $manifestItems) {
    $expectedPath = Join-Path (Join-Path $OutputRoot $item.Layer) ($item.ModuleName + $item.Extension)
    if (-not (Test-Path $expectedPath)) {
        $failures.Add("Manifest item missing from generated output: $expectedPath")
    }
}

$candidatePath = Join-Path $workspaceRoot "candidates\VMFCandidates_v1.1.md"
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
Write-Host "Manifest: specs/vmf/manifest.yaml"
Write-Host "Release Gate: generated layers present with module headers and Option Explicit."
