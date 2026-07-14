# Creates the persistent VMF test runner add-in.

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

function Resolve-VmfRepositoryRoot {
    param([string]$StartPath)

    $currentPath = [IO.Path]::GetFullPath($StartPath)
    while ($true) {
        $markerPath = Join-Path $currentPath ".vmf-root"
        if (Test-Path -LiteralPath $markerPath -PathType Leaf) {
            return $currentPath
        }

        $parentPath = Split-Path -Parent $currentPath
        if ([string]::IsNullOrWhiteSpace($parentPath) -or $parentPath -eq $currentPath) {
            break
        }

        $currentPath = $parentPath
    }

    throw "VMF repository root could not be resolved from: $StartPath"
}

function Ensure-ExcelTrustedLocation {
    param(
        [string]$ExcelVersion,
        [string]$TrustedPath
    )

    $trustedPath = [IO.Path]::GetFullPath($TrustedPath)
    if (-not $trustedPath.EndsWith("\")) {
        $trustedPath = "$trustedPath\"
    }

    $trustedLocationsKey = "HKCU:\Software\Microsoft\Office\$ExcelVersion\Excel\Security\Trusted Locations"
    if (-not (Test-Path $trustedLocationsKey)) {
        New-Item -Path $trustedLocationsKey -Force | Out-Null
    }

    $existingLocation = Get-ChildItem -Path $trustedLocationsKey -ErrorAction SilentlyContinue |
        Where-Object {
            $pathValue = (Get-ItemProperty -Path $_.PSPath -Name Path -ErrorAction SilentlyContinue).Path
            $pathValue -and ([string]::Compare($pathValue, $trustedPath, $true) -eq 0)
        } |
        Select-Object -First 1

    if ($existingLocation) {
        Write-Host "Excel trusted location already registered: $trustedPath"
        return
    }

    $nextIndex = 0
    do {
        $locationKey = Join-Path $trustedLocationsKey ("Location{0}" -f $nextIndex)
        $nextIndex++
    } while (Test-Path $locationKey)

    New-Item -Path $locationKey -Force | Out-Null
    New-ItemProperty -Path $locationKey -Name Path -Value $trustedPath -PropertyType String -Force | Out-Null
    New-ItemProperty -Path $locationKey -Name AllowSubFolders -Value 1 -PropertyType DWord -Force | Out-Null
    New-ItemProperty -Path $locationKey -Name Description -Value "VMF test runner" -PropertyType String -Force | Out-Null
    Write-Host "Registered Excel trusted location: $trustedPath"
}

function Import-VbaFiles {
    param(
        [object]$Workbook,
        [string[]]$RootPaths
    )

    $extensions = @(".bas")
    foreach ($rootPath in $RootPaths) {
        Get-ChildItem -LiteralPath $rootPath -Recurse -File |
            Where-Object { $extensions -contains $_.Extension.ToLowerInvariant() } |
            Sort-Object FullName |
            ForEach-Object {
                $Workbook.VBProject.VBComponents.Import($_.FullName) | Out-Null
            }
    }
}

$root = Resolve-VmfRepositoryRoot -StartPath $scriptDir
$runnerDir = Join-Path $root "tools\test\runner"
$runnerSourcePath = Join-Path $runnerDir "VMFTestRunner.bas"
$runnerWorkbookPath = Join-Path $runnerDir "VMFTestRunner.xlam"
$buildPath = Join-Path $root "dist\release\Build\v1.1\Build.xlam"
$testRoot = Join-Path $root "tests"

if (-not (Test-Path $runnerSourcePath)) {
    Write-Error "Runner source not found at $runnerSourcePath"
    exit 1
}

if (-not (Test-Path $buildPath)) {
    Write-Error "Build.xlam not found at $buildPath. Run tools\build\build.ps1 first."
    exit 1
}

if (-not (Test-Path $runnerDir)) {
    New-Item -ItemType Directory -Path $runnerDir | Out-Null
}

try {
    $excel = New-Object -ComObject Excel.Application
}
catch {
    Write-Error "Failed to start Excel: $_"
    exit 1
}

$excel.AutomationSecurity = 1
$excel.Visible = $true
$excel.DisplayAlerts = $false
$excel.EnableEvents = $false

try {
    Ensure-ExcelTrustedLocation -ExcelVersion $excel.Version -TrustedPath $runnerDir

    if (Test-Path -LiteralPath $runnerWorkbookPath) {
        Remove-Item -LiteralPath $runnerWorkbookPath -Force
    }

    Copy-Item -LiteralPath $buildPath -Destination $runnerWorkbookPath -Force
    $workbook = $excel.Workbooks.Open($runnerWorkbookPath)
    $workbook.VBProject.VBComponents.Import($runnerSourcePath) | Out-Null
    Import-VbaFiles -Workbook $workbook -RootPaths @($testRoot)
    $workbook.Save()
    Write-Host "Created VMF test runner add-in: $runnerWorkbookPath"
    $workbook.Close($true)
}
catch {
    Write-Error "Failed to create VMF test runner workbook: $($_.Exception.Message)"
    exit 1
}
finally {
    $excel.Quit()
    [System.GC]::Collect()
    [System.GC]::WaitForPendingFinalizers()
}
