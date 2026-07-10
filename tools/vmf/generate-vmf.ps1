# Generates the VMF v1.0 project from specs/vmf/manifest.yaml.
# This wrapper preserves Build.xlam v1.0.1 by reusing its manifest-driven
# template shape without changing the frozen add-in artifact.

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

$manifestFullPath = Resolve-Path $ManifestPath
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
                CanonicalLayer = $layerName
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

function Test-ModuleName {
    param([string]$Name)
    return $Name -match "^[A-Za-z][A-Za-z0-9_]*$"
}

$expectedOrder = @("Common", "Core", "Domain", "Application", "Infrastructure", "Presentation")
$layers = @(Read-VmfManifest -Path $manifestFullPath)

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
    $layerDir = Join-Path $outputFullPath $layer.Name
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

Write-Host "VMF v1.0 project generated from $manifestFullPath"
