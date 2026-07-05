# Build.xlam creation script (ASCII-only messages)
# Uses Excel COM automation to import .bas/.cls from src and save as an XLAM add-in.

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$workspaceRoot = Resolve-Path (Join-Path $scriptDir "..\..")
$srcDir = Join-Path $workspaceRoot "src"
$output = Join-Path $workspaceRoot "Build.xlam"

Write-Host "Workspace: $workspaceRoot"
Write-Host "Source dir: $srcDir"
Write-Host "Output: $output"

if (-not (Test-Path $srcDir)) {
    Write-Error "src directory not found: $srcDir"
    exit 1
}

try {
    $excel = New-Object -ComObject Excel.Application
}
catch {
    Write-Error "Failed to start Excel. Is Excel installed? Error: $_"
    exit 1
}

$excel.Visible = $false
$excel.DisplayAlerts = $false

$wb = $excel.Workbooks.Add()

# Remove all worksheets (add-in doesn't need sheets)
try {
    foreach ($ws in @($wb.Worksheets)) { $ws.Delete() }
}
catch {
    # ignore
}

# Check VBProject access
try {
    $vbproj = $wb.VBProject
}
catch {
    Write-Error "Access to the VBA project is denied. Enable 'Trust access to the VBA project object model' in Excel settings. Error: $_"
    $excel.Quit()
    exit 1
}

# Import modules and classes
function Sanitize-VbaFile {
    param([string]$Path)
    $content = Get-Content -Raw -Encoding UTF8 $Path
    $lines = $content -split "\r?\n"
    if ($lines.Length -ge 1 -and $lines[0] -match '^\s*VERSION\s+1\.0\s+CLASS') {
        for ($i = 0; $i -lt [Math]::Min(6,$lines.Length); $i++) {
            if ($lines[$i] -match '^\s*End\s*$') { $lines[$i] = 'END' }
        }
        if ($lines -notmatch 'Attribute VB_Name') {
            $name = [IO.Path]::GetFileNameWithoutExtension($Path)
            $attrs = @(
                ('Attribute VB_Name = "' + $name + '"'),
                'Attribute VB_GlobalNameSpace = False',
                'Attribute VB_Creatable = False',
                'Attribute VB_PredeclaredId = False',
                'Attribute VB_Exposed = False'
            )
            # Insert attributes after the END line (search first occurrence)
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

$files = Get-ChildItem -Path $srcDir -Recurse -Include *.bas, *.cls -File | Sort-Object FullName
if ($files.Count -eq 0) { Write-Warning "No .bas/.cls files found to import." }
foreach ($f in $files) {
    Write-Host "Importing: $($f.FullName)"
    try {
        $tmp = Sanitize-VbaFile -Path $f.FullName
        $wb.VBProject.VBComponents.Import($tmp) | Out-Null
        Remove-Item $tmp -ErrorAction SilentlyContinue
    }
    catch {
        Write-Warning "Failed to import: $($f.FullName) - $_"
    }
}

# Save as XLAM
$xlOpenXMLAddin = 55
try {
    $wb.SaveAs($output, $xlOpenXMLAddin)
    Write-Host "Saved add-in: $output"
}
catch {
    Write-Warning "Could not save as XLAM: $_"
    try {
        $wb.SaveAs($output)
        Write-Host "Saved (fallback): $output"
    }
    catch {
        Write-Error "Save failed. Error: $_"
    }
}

$wb.Close($false)
$excel.Quit()

# Release COM
[System.GC]::Collect()
[System.GC]::WaitForPendingFinalizers()

Write-Host "Build script finished."
