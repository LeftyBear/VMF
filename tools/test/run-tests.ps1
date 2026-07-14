# Runs VBA unit tests through the persistent VMF test runner workbook.

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

$root = Resolve-VmfRepositoryRoot -StartPath $scriptDir
$buildPath = Join-Path $root "dist\release\Build\v1.0.2\Build.xlam"
$runnerPath = Join-Path $root "tools\test\runner\VMFTestRunner.xlam"
$targetDir = Join-Path $root "dist\debug\test-target"
$targetPath = Join-Path $targetDir "VMF.xlam"

if (-not (Test-Path $buildPath)) {
    Write-Error "Build.xlam not found at $buildPath"
    exit 1
}

if (-not (Test-Path $runnerPath)) {
    Write-Error "VMFTestRunner.xlam not found at $runnerPath. Run tools\test\setup-test-runner.ps1 first."
    exit 1
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

$addin = $null
$runner = $null
$target = $null
$summary = $null
$exitCode = 0

try {
    if (-not (Test-Path -LiteralPath $targetDir)) {
        New-Item -ItemType Directory -Force -Path $targetDir | Out-Null
    }
    if (Test-Path -LiteralPath $targetPath) {
        Remove-Item -LiteralPath $targetPath -Force
    }

    $target = $excel.Workbooks.Add()
    $target.SaveAs($targetPath, 55)

    $addin = $excel.Workbooks.Open($buildPath)
    $runner = $excel.Workbooks.Open($runnerPath, 0, $false)
    $runner.Activate()

    $macroName = "'$($runner.Name)'!VMFRunAllTests"
    Write-Host "Invoking test runner: $macroName"
    $summary = $excel.Run($macroName, $root)
    Write-Host $summary
    if ($summary -match ": Failed") {
        $exitCode = 2
    }
}
catch {
    $exitCode = 2
    Write-Error "Test runner failed: $($_.Exception.Message)"
}
finally {
    if ($runner -ne $null) {
        $runner.Close($false)
    }
    if ($addin -ne $null) {
        $addin.Close($false)
    }
    if ($target -ne $null) {
        $target.Close($false)
    }
    $excel.Quit()
    [System.GC]::Collect()
    [System.GC]::WaitForPendingFinalizers()
}

exit $exitCode
