# Generates VMF v1.0 projects from a manifest.
# This wrapper preserves Build.xlam v1.0.1/v1.1 by reusing the manifest-driven
# template shape without changing frozen add-in artifacts.

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

$outputFullPath = [IO.Path]::GetFullPath($OutputRoot)
$workspaceFullPath = [IO.Path]::GetFullPath($workspaceRoot)

if (-not $outputFullPath.StartsWith($workspaceFullPath, [StringComparison]::OrdinalIgnoreCase)) {
    Write-Error "OutputRoot must stay inside the workspace: $outputFullPath"
    exit 1
}

function New-VmfModuleText {
    param(
        [string]$ModuleName,
        [string]$ModuleType,
        [string]$LayerName
    )

    if ($ModuleType -eq "EnumModule") {
@"
Option Explicit

'=========================================================================
' Enum: $ModuleName
' Layer: $LayerName
' Responsibility:
'=========================================================================

Public Enum $ModuleName
    ${ModuleName}_None = 0
End Enum
"@
        return
    }

    $kind = "Module"
    if ($ModuleType -eq "ClassModule") { $kind = "Class" }

@"
Option Explicit

'=========================================================================
' ${kind}: $ModuleName
' Layer: $LayerName
' Responsibility:
'=========================================================================
"@
}

function Test-ModuleName {
    param([string]$Name)
    return $Name -match "^[A-Za-z][A-Za-z0-9_]*$"
}

function Read-VmfManifest {
    param([string]$Path)

    $layers = New-Object System.Collections.Generic.List[object]
    $layerByName = @{}
    $currentModuleLayer = $null
    $currentModuleType = ""
    $section = ""

    foreach ($rawLine in Get-Content -Encoding UTF8 $Path) {
        $line = $rawLine.TrimEnd()
        $trimmed = $line.Trim()

        if ([string]::IsNullOrWhiteSpace($trimmed) -or $trimmed.StartsWith("#")) {
            continue
        }

        if ($trimmed -eq "Layers:") {
            $section = "layers"
            continue
        }

        if ($trimmed -eq "Modules:") {
            $section = "modules"
            continue
        }

        if ($section -eq "layers" -and $line -match "^\s{2}-\s*(.+)$") {
            $layerName = $matches[1].Trim()
            $layer = [pscustomobject]@{
                Name = $layerName
                Path = $layerName
                Modules = New-Object System.Collections.Generic.List[object]
            }
            $layers.Add($layer)
            $layerByName[$layerName] = $layer
            continue
        }

        if ($section -eq "modules" -and $line -match "^\s{2}(\S[^:]*):\s*$") {
            $currentModuleLayer = $matches[1].Trim()
            $currentModuleType = ""
            if (-not $layerByName.ContainsKey($currentModuleLayer)) {
                Write-Error "Modules section references undefined layer: $currentModuleLayer"
                exit 1
            }
            continue
        }

        if ($section -eq "modules" -and $line -match "^\s{4}(Classes|StandardModules|Enums):\s*$") {
            switch ($matches[1]) {
                "Classes" { $currentModuleType = "ClassModule" }
                "StandardModules" { $currentModuleType = "StandardModule" }
                "Enums" { $currentModuleType = "EnumModule" }
            }
            continue
        }

        if ($section -eq "modules" -and $null -ne $currentModuleLayer -and $line -match "^\s{6}-\s*(\S.+)$") {
            if ([string]::IsNullOrWhiteSpace($currentModuleType)) {
                Write-Error "Module type is missing before module: $($matches[1].Trim())"
                exit 1
            }

            $module = [pscustomobject]@{
                Name = $matches[1].Trim()
                Type = $currentModuleType
            }
            $layerByName[$currentModuleLayer].Modules.Add($module)
            continue
        }
    }

    return $layers
}

function Read-ApplicationManifest {
    param([string]$Path)

    $canonicalLayers = @{
        "common" = "Common"
        "core" = "Core"
        "domain" = "Domain"
        "application" = "Application"
        "infrastructure" = "Infrastructure"
        "presentation" = "Presentation"
    }
    $layers = @{}
    $section = ""
    $subsection = ""
    $currentLayerKey = ""

    foreach ($key in $canonicalLayers.Keys) {
        $layers[$canonicalLayers[$key]] = [pscustomobject]@{
            Name = $canonicalLayers[$key]
            Path = $canonicalLayers[$key]
            Modules = New-Object System.Collections.Generic.List[object]
        }
    }

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
            $layers["Domain"].Modules.Add([pscustomobject]@{ Name = $matches[1].Trim(); Type = "ClassModule" })
            continue
        }

        if ($section -eq "application_services" -and $line -match "^\s{2}-\s*(\S.+)$") {
            $layers["Application"].Modules.Add([pscustomobject]@{ Name = $matches[1].Trim(); Type = "ClassModule" })
            continue
        }

        if ($section -eq "infrastructure" -and $line -match "^\s{2}repositories:\s*$") {
            $subsection = "repositories"
            continue
        }

        if ($section -eq "infrastructure" -and $subsection -eq "repositories" -and $line -match "^\s{4}-\s*(\S.+)$") {
            $layers["Infrastructure"].Modules.Add([pscustomobject]@{ Name = $matches[1].Trim(); Type = "ClassModule" })
            continue
        }

        if ($section -eq "presentation" -and $line -match "^\s{2}(forms|modules):\s*$") {
            $subsection = $matches[1]
            continue
        }

        if ($section -eq "presentation" -and $subsection -eq "modules" -and $line -match "^\s{4}-\s*(\S.+)$") {
            $layers["Presentation"].Modules.Add([pscustomobject]@{ Name = $matches[1].Trim(); Type = "ClassModule" })
            continue
        }
    }

    $order = @("Common", "Core", "Domain", "Application", "Infrastructure", "Presentation")
    return $order | ForEach-Object { $layers[$_] }
}

$expectedOrder = @("Common", "Core", "Domain", "Application", "Infrastructure", "Presentation")
if ($isApplicationManifest) {
    $layers = @(Read-ApplicationManifest -Path $manifestFullPath)
}
else {
    $layers = @(Read-VmfManifest -Path $manifestFullPath)
}

if ($layers.Count -ne $expectedOrder.Count) {
    Write-Error "Manifest layer count does not match VMF v1.0 generation order."
    exit 1
}

for ($i = 0; $i -lt $expectedOrder.Count; $i++) {
    if ($layers[$i].Name -ne $expectedOrder[$i]) {
        Write-Error "Manifest layer order mismatch at index $i. Expected $($expectedOrder[$i]); found $($layers[$i].Name)."
        exit 1
    }
}

foreach ($layer in $layers) {
    if ($isApplicationManifest) {
        $layerDir = [IO.Path]::GetFullPath((Join-Path $manifestDirectory $layer.Path))
    }
    else {
        $layerDir = Join-Path $outputFullPath $layer.Name
    }

    if (-not $layerDir.StartsWith($workspaceFullPath, [StringComparison]::OrdinalIgnoreCase)) {
        Write-Error "Layer output must stay inside the workspace: $layerDir"
        exit 1
    }

    New-Item -ItemType Directory -Force -Path $layerDir | Out-Null

    foreach ($module in $layer.Modules) {
        if (-not (Test-ModuleName -Name $module.Name)) {
            Write-Error "Invalid module name: $($module.Name)"
            exit 1
        }

        switch ($module.Type) {
            "StandardModule" { $extension = ".bas" }
            "EnumModule" { $extension = ".bas" }
            "ClassModule" { $extension = ".cls" }
            default {
                Write-Error "Unsupported module type for $($module.Name): $($module.Type)"
                exit 1
            }
        }

        $modulePath = Join-Path $layerDir ($module.Name + $extension)
        $text = New-VmfModuleText -ModuleName $module.Name -ModuleType $module.Type -LayerName $layer.Name
        $text | Out-File -FilePath $modulePath -Encoding ASCII
        Write-Host "Generated: $modulePath"
    }
}

if ($isApplicationManifest) {
    Write-Host "Application project generated from $manifestFullPath"
    Write-Host "Presentation forms are declared in the manifest but are not generated by this tool."
}
else {
    Write-Host "VMF v1.0 project generated from $manifestFullPath"
}
