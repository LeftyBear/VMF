# Audits generated VMF v1.0 project structure against a VMF manifest.

param(
    [string]$ManifestPath,
    [string]$OutputRoot
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$workspaceRoot = Resolve-Path (Join-Path $scriptDir "..\..")

if ([string]::IsNullOrWhiteSpace($ManifestPath)) {
    $defaultManifestPath = Join-Path $workspaceRoot "specs\vmf\manifest.yaml"
    $applicationManifestPath = Join-Path $workspaceRoot "applications\SchoolTimetable\manifest.yaml"
    if (Test-Path $defaultManifestPath) {
        $ManifestPath = $defaultManifestPath
    }
    else {
        $ManifestPath = $applicationManifestPath
    }
}

if (-not (Test-Path $ManifestPath)) {
    Write-Error "Manifest not found: $ManifestPath"
    exit 1
}

$manifestFullPath = Resolve-Path $ManifestPath
$manifestDirectory = Split-Path -Parent $manifestFullPath
$manifestText = Get-Content -Raw -Encoding UTF8 $manifestFullPath
$isApplicationManifest = ($manifestText -match "(?m)^application:\s*$") -and ($manifestText -match "(?m)^layers:\s*$")

if ([string]::IsNullOrWhiteSpace($OutputRoot)) {
    if ($isApplicationManifest) {
        $OutputRoot = Join-Path $manifestDirectory "src"
    }
    else {
        $OutputRoot = Join-Path $workspaceRoot "src\VMF"
    }
}

if (-not (Test-Path $OutputRoot)) {
    Write-Error "Output root not found: $OutputRoot"
    exit 1
}

$requiredLayers = @("Common", "Core", "Domain", "Application", "Infrastructure", "Presentation")
$failures = New-Object System.Collections.Generic.List[string]
$warnings = New-Object System.Collections.Generic.List[string]

function New-LayerMap {
    $layers = @{}
    foreach ($layer in $requiredLayers) {
        $layers[$layer] = [pscustomobject]@{
            Name = $layer
            Path = $layer
            Items = New-Object System.Collections.Generic.List[object]
        }
    }
    return $layers
}

function Read-VmfManifestItems {
    param([string]$Path)

    $layers = New-LayerMap
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
            $layers[$currentLayer].Items.Add([pscustomobject]@{
                Layer = $currentLayer
                ModuleName = $matches[1].Trim()
                Extension = $extension
            })
        }
    }

    return $layers
}

function Read-ApplicationManifestItems {
    param([string]$Path)

    $canonicalLayers = @{
        "common" = "Common"
        "core" = "Core"
        "domain" = "Domain"
        "application" = "Application"
        "infrastructure" = "Infrastructure"
        "presentation" = "Presentation"
    }
    $layers = New-LayerMap
    $section = ""
    $subsection = ""
    $currentLayerKey = ""

    foreach ($rawLine in Get-Content -Encoding UTF8 $Path) {
        $line = $rawLine.TrimEnd()
        $trimmed = $line.Trim()

        if ([string]::IsNullOrWhiteSpace($trimmed) -or $trimmed.StartsWith("#")) {
            continue
        }

        if ($line -match "^([A-Za-z_][A-Za-z0-9_]*):\s*$") {
            $section = $matches[1]
            $subsection = ""
            $currentLayerKey = ""
            continue
        }

        if ($section -eq "layers" -and $line -match "^\s{2}([A-Za-z_][A-Za-z0-9_]*):\s*$") {
            $currentLayerKey = $matches[1]
            continue
        }

        if ($section -eq "layers" -and $line -match "^\s{4}path:\s*(.+)$") {
            if ($canonicalLayers.ContainsKey($currentLayerKey)) {
                $layers[$canonicalLayers[$currentLayerKey]].Path = $matches[1].Trim()
            }
            continue
        }

        if ($section -eq "domain" -and $line -match "^\s{2}entities:\s*$") {
            $subsection = "entities"
            continue
        }

        if ($section -eq "domain" -and $subsection -eq "entities" -and $line -match "^\s{4}-\s*(\S.+)$") {
            $layers["Domain"].Items.Add([pscustomobject]@{ Layer = "Domain"; ModuleName = $matches[1].Trim(); Extension = ".cls" })
            continue
        }

        if ($section -eq "application_services" -and $line -match "^\s{2}-\s*(\S.+)$") {
            $layers["Application"].Items.Add([pscustomobject]@{ Layer = "Application"; ModuleName = $matches[1].Trim(); Extension = ".cls" })
            continue
        }

        if ($section -eq "infrastructure" -and $line -match "^\s{2}repositories:\s*$") {
            $subsection = "repositories"
            continue
        }

        if ($section -eq "infrastructure" -and $subsection -eq "repositories" -and $line -match "^\s{4}-\s*(\S.+)$") {
            $layers["Infrastructure"].Items.Add([pscustomobject]@{ Layer = "Infrastructure"; ModuleName = $matches[1].Trim(); Extension = ".cls" })
            continue
        }

        if ($section -eq "presentation" -and $line -match "^\s{2}(forms|modules):\s*$") {
            $subsection = $matches[1]
            continue
        }

        if ($section -eq "presentation" -and $subsection -eq "forms" -and $line -match "^\s{4}-\s*(\S.+)$") {
            $warnings.Add("Presentation form is declared but not generated by VMF tools: $($matches[1].Trim())")
            continue
        }

        if ($section -eq "presentation" -and $subsection -eq "modules" -and $line -match "^\s{4}-\s*(\S.+)$") {
            $layers["Presentation"].Items.Add([pscustomobject]@{ Layer = "Presentation"; ModuleName = $matches[1].Trim(); Extension = ".cls" })
            continue
        }
    }

    return $layers
}

if ($isApplicationManifest) {
    $manifestLayers = Read-ApplicationManifestItems -Path $manifestFullPath
}
else {
    $manifestLayers = Read-VmfManifestItems -Path $manifestFullPath
}

foreach ($layer in $requiredLayers) {
    $layerSpec = $manifestLayers[$layer]
    if ($isApplicationManifest) {
        $layerPath = [IO.Path]::GetFullPath((Join-Path $manifestDirectory $layerSpec.Path))
    }
    else {
        $layerPath = Join-Path $OutputRoot $layer
    }

    if (-not (Test-Path $layerPath)) {
        $failures.Add("Missing layer directory: $layer")
        continue
    }

    $files = @(Get-ChildItem -Path $layerPath -Include *.bas,*.cls -File -Recurse)
    if (-not $isApplicationManifest -and $files.Count -eq 0) {
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

    foreach ($item in $layerSpec.Items) {
        $expectedPath = Join-Path $layerPath ($item.ModuleName + $item.Extension)
        if (-not (Test-Path $expectedPath)) {
            $failures.Add("Manifest item missing from generated output: $expectedPath")
        }
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
if ($warnings.Count -gt 0) {
    foreach ($warning in $warnings) {
        Write-Host "WARN: $warning"
    }
}
Write-Host "Blueprint: BuildBlueprint_v1.0.1.md"
Write-Host "Manifest: $manifestFullPath"
Write-Host "Release Gate: generated layers present with module headers and Option Explicit."
