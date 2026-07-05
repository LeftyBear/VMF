# Runs VBA unit tests by importing test modules and invoking their public runners.

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$root = Resolve-Path (Join-Path $scriptDir "..\..")
$buildPath = Join-Path $root "Build.xlam"
$testDir = Join-Path $root "test"

if (-not (Test-Path $buildPath)) { Write-Error "Build.xlam not found at $buildPath"; exit 1 }

$testModules = Get-ChildItem -Path $testDir -Recurse -Include *.bas -File | Sort-Object FullName
$runners = @(
    'ComRunCommonPhase1Tests',
    'InfRunInfrastructurePhase2Tests',
    'DomRunVmfCorePhase3Tests',
    'AppRunGeneratorPhase1Tests',
    'AppRunGeneratorPhase1NegativeTests',
    'AppRunGeneratorPhase2Tests',
    'AppRunBuildPhase4Tests',
    'PreRunUiPhase5Tests',
    'PreRunGenerateModulePhase6Tests'
)

$results = @()

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

# Open add-in (ensures VB components available)
$addin = $excel.Workbooks.Open($buildPath)

# Create temp workbook for tests
$wb = $excel.Workbooks.Add()

# Import test modules
foreach ($m in $testModules) {
    Write-Host "Importing test module: $($m.FullName)"
    try {
        # Sanitize before import to avoid malformed headers
        function Sanitize-VbaFile {
            param([string]$Path)
            $content = Get-Content -Raw -Encoding UTF8 $Path
            $lines = $content -split "\r?\n"
            if ($lines.Length -ge 1 -and $lines[0] -match '^\s*VERSION\s+1\.0\s+CLASS') {
                for ($i = 0; $i -lt [Math]::Min(6,$lines.Length); $i++) { if ($lines[$i] -match '^\s*End\s*$') { $lines[$i] = 'END' } }
                if ($lines -notmatch 'Attribute VB_Name') {
                    $name = [IO.Path]::GetFileNameWithoutExtension($Path)
                    $attrs = @(
                        ('Attribute VB_Name = "' + $name + '"'),
                        'Attribute VB_GlobalNameSpace = False',
                        'Attribute VB_Creatable = False',
                        'Attribute VB_PredeclaredId = False',
                        'Attribute VB_Exposed = False'
                    )
                    $insertIndex = 0
                    for ($j=0; $j -lt $lines.Length; $j++) { if ($lines[$j] -match '^\s*END\s*$') { $insertIndex = $j+1; break } }
                    $newLines = $lines[0..($insertIndex-1)] + $attrs + $lines[$insertIndex..($lines.Length-1)]
                    $lines = $newLines
                }
                $content = ($lines -join "`r`n")
            }
            $tmp = [IO.Path]::Combine([IO.Path]::GetTempPath(), "vmf_sanitized_$(Get-Random)-" + [IO.Path]::GetFileName($Path))
            $content | Out-File -FilePath $tmp -Encoding ASCII
            return $tmp
        }
        $tmp = Sanitize-VbaFile -Path $m.FullName
        $wb.VBProject.VBComponents.Import($tmp) | Out-Null
        Remove-Item $tmp -ErrorAction SilentlyContinue
    } catch { Write-Warning "Import failed: $_" }
}

# Also import project source modules so tests can early-bind to project types
$srcDir = Join-Path $root "src"
if (Test-Path $srcDir) {
    $srcFiles = Get-ChildItem -Path $srcDir -Recurse -Include *.bas, *.cls -File | Sort-Object FullName
    foreach ($s in $srcFiles) {
        Write-Host "Importing source module: $($s.FullName)"
        try {
            $tmp = Sanitize-VbaFile -Path $s.FullName
            $wb.VBProject.VBComponents.Import($tmp) | Out-Null
            Remove-Item $tmp -ErrorAction SilentlyContinue
        } catch { Write-Warning "Import failed: $($s.FullName) - $_" }
    }
}
else {
    Write-Warning "src directory not found: $srcDir"
}

# Save workbook as macro-enabled temporary file so macros can run
$tempPath = Join-Path $env:TEMP "vmf_tests_$(Get-Random).xlsm"
try {
    $xlOpenXMLWorkbookMacroEnabled = 52
    $wb.SaveAs($tempPath, $xlOpenXMLWorkbookMacroEnabled)
    Write-Host "Saved test workbook: $tempPath"
}
catch {
    Write-Warning "Failed to save test workbook as xlsm: $_"
}

# Run each runner
foreach ($r in $runners) {
    Write-Host "Running: $r"
    try {
        $owner = $null
        foreach ($comp in $wb.VBProject.VBComponents) {
            $cnt = $comp.CodeModule.CountOfLines
            if ($cnt -gt 0) {
                $txt = $comp.CodeModule.Lines(1, $cnt)
                if ($txt -match "\b(Sub|Function)\s+$r\b") { $owner = $comp.Name; break }
            }
        }
        if ($owner) { $macroName = "$(Split-Path $tempPath -Leaf)!$owner.$r" } else { $macroName = "$(Split-Path $tempPath -Leaf)!$r" }
        Write-Host "Invoking macro: $macroName"
        $excel.Run($macroName)
        $results += [pscustomobject]@{Runner = $r; Status = 'Passed'; Error = $null }
        Write-Host "Passed: $r"
    }
    catch {
        $err = $_.Exception.Message
        $results += [pscustomobject]@{Runner = $r; Status = 'Failed'; Error = $err }
        Write-Warning "Failed: $r - $err"
    }
}

# Close workbook without saving
$wb.Close($false)
$addin.Close($false)
$excel.Quit()
[System.GC]::Collect(); [System.GC]::WaitForPendingFinalizers();

Write-Host "Test run summary:"
$results | Format-Table -AutoSize

# Exit with non-zero if any failures
if ($results | Where-Object { $_.Status -eq 'Failed' }) { exit 2 } else { exit 0 }
