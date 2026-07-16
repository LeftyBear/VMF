# Build.xlam creation script (ASCII-only messages)
# Uses Excel COM automation to import .bas/.cls/.frm from src and save as an XLAM add-in.

param(
    [string]$OutputPath,
    [string]$BuildVersion,
    [string]$ReleaseType = "Release"
)

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

$workspaceRoot = Resolve-VmfRepositoryRoot -StartPath $scriptDir
$srcDir = Join-Path $workspaceRoot "src\Build"

function Get-ReleaseInfoValue {
    param(
        [string]$SourcePath,
        [string]$ConstantName
    )

    $text = Get-Content -LiteralPath $SourcePath -Raw -Encoding Default
    $pattern = "Private Const $ConstantName As String = `"([^`"]+)`""
    $match = [regex]::Match($text, $pattern)
    if (-not $match.Success) {
        throw "Release info constant not found: $ConstantName"
    }

    return $match.Groups[1].Value
}

$releaseInfoPath = Join-Path $srcDir "Application\AppReleaseInfo.bas"
$productName = Get-ReleaseInfoValue -SourcePath $releaseInfoPath -ConstantName "RELEASE_PRODUCT_NAME"
$productVersion = Get-ReleaseInfoValue -SourcePath $releaseInfoPath -ConstantName "RELEASE_PRODUCT_VERSION"
$manifestSchemaVersion = Get-ReleaseInfoValue -SourcePath $releaseInfoPath -ConstantName "RELEASE_MANIFEST_SCHEMA_VERSION"
$templateSchemaVersion = Get-ReleaseInfoValue -SourcePath $releaseInfoPath -ConstantName "RELEASE_TEMPLATE_SCHEMA_VERSION"
$minimumSupportedVersion = Get-ReleaseInfoValue -SourcePath $releaseInfoPath -ConstantName "RELEASE_MINIMUM_SUPPORTED_VERSION"
$buildDate = Get-ReleaseInfoValue -SourcePath $releaseInfoPath -ConstantName "RELEASE_BUILD_DATE"

if ([string]::IsNullOrWhiteSpace($BuildVersion)) {
    $BuildVersion = $productVersion
}

if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $versionFolder = $productVersion -replace '^(\d+\.\d+).*$', 'v$1'
    $output = Join-Path $workspaceRoot "dist\release\Build\$versionFolder\Build.xlam"
}
else {
    $output = [IO.Path]::GetFullPath($OutputPath)
}

Write-Host "Workspace: $workspaceRoot"
Write-Host "Source dir: $srcDir"
Write-Host "Output: $output"
Write-Host "Product: $productName $productVersion"

if (-not (Test-Path $srcDir)) {
    Write-Error "Build source directory not found: $srcDir"
    exit 1
}

$outputParent = Split-Path -Parent $output
if (-not (Test-Path $outputParent)) {
    New-Item -ItemType Directory -Force -Path $outputParent | Out-Null
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

# Import modules, classes, and forms
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

$files = Get-ChildItem -Path $srcDir -Recurse -Include *.bas, *.cls, *.frm -File | Sort-Object FullName
if ($files.Count -eq 0) { Write-Warning "No .bas/.cls/.frm files found to import." }
foreach ($f in $files) {
    Write-Host "Importing: $($f.FullName)"
    try {
        if ($f.Extension -eq ".frm") {
            $wb.VBProject.VBComponents.Import($f.FullName) | Out-Null
        }
        else {
            $tmp = Sanitize-VbaFile -Path $f.FullName
            $wb.VBProject.VBComponents.Import($tmp) | Out-Null
            Remove-Item $tmp -ErrorAction SilentlyContinue
        }
    }
    catch {
        Write-Warning "Failed to import: $($f.FullName) - $_"
    }
}

# Save as XLAM
$xlOpenXMLAddin = 55
$saveSucceeded = $false
try {
    $wb.SaveAs($output, $xlOpenXMLAddin)
    Write-Host "Saved add-in: $output"
    $saveSucceeded = $true
}
catch {
    Write-Warning "Could not save as XLAM: $_"
    try {
        $wb.SaveAs($output)
        Write-Host "Saved (fallback): $output"
        $saveSucceeded = $true
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

if (-not $saveSucceeded) {
    Write-Error "Build failed before release metadata could be recorded."
    exit 1
}

function Set-AddInReleaseMetadata {
    param(
        [string]$Path,
        [string]$ProductName,
        [string]$ProductVersion,
        [string]$BuildVersion,
        [string]$ReleaseType,
        [string]$ManifestSchemaVersion,
        [string]$TemplateSchemaVersion,
        [string]$MinimumSupportedVersion,
        [string]$BuildDate
    )

    Add-Type -AssemblyName System.IO.Compression
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [System.IO.Compression.ZipFile]::Open($Path, [System.IO.Compression.ZipArchiveMode]::Update)
    try {
        $customXml = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Properties xmlns="http://schemas.openxmlformats.org/officeDocument/2006/custom-properties" xmlns:vt="http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes"><property fmtid="{D5CDD505-2E9C-101B-9397-08002B2CF9AE}" pid="2" name="Product Name"><vt:lpwstr>$ProductName</vt:lpwstr></property><property fmtid="{D5CDD505-2E9C-101B-9397-08002B2CF9AE}" pid="3" name="Product Version"><vt:lpwstr>$ProductVersion</vt:lpwstr></property><property fmtid="{D5CDD505-2E9C-101B-9397-08002B2CF9AE}" pid="4" name="Build Version"><vt:lpwstr>$BuildVersion</vt:lpwstr></property><property fmtid="{D5CDD505-2E9C-101B-9397-08002B2CF9AE}" pid="5" name="Release Type"><vt:lpwstr>$ReleaseType</vt:lpwstr></property><property fmtid="{D5CDD505-2E9C-101B-9397-08002B2CF9AE}" pid="6" name="Manifest Schema Version"><vt:lpwstr>$ManifestSchemaVersion</vt:lpwstr></property><property fmtid="{D5CDD505-2E9C-101B-9397-08002B2CF9AE}" pid="7" name="Template Schema Version"><vt:lpwstr>$TemplateSchemaVersion</vt:lpwstr></property><property fmtid="{D5CDD505-2E9C-101B-9397-08002B2CF9AE}" pid="8" name="Minimum Supported Version"><vt:lpwstr>$MinimumSupportedVersion</vt:lpwstr></property><property fmtid="{D5CDD505-2E9C-101B-9397-08002B2CF9AE}" pid="9" name="Build Date"><vt:lpwstr>$BuildDate</vt:lpwstr></property></Properties>
"@
        $customEntry = $zip.GetEntry("docProps/custom.xml")
        if ($customEntry -ne $null) { $customEntry.Delete() }
        $customEntry = $zip.CreateEntry("docProps/custom.xml")
        $writer = New-Object System.IO.StreamWriter($customEntry.Open(), [System.Text.Encoding]::UTF8)
        try { $writer.Write($customXml) } finally { $writer.Dispose() }

        $contentTypesEntry = $zip.GetEntry("[Content_Types].xml")
        $reader = New-Object System.IO.StreamReader($contentTypesEntry.Open(), [System.Text.Encoding]::UTF8)
        try { $contentTypes = $reader.ReadToEnd() } finally { $reader.Dispose() }
        if ($contentTypes -notmatch "/docProps/custom\.xml") {
            $contentTypes = $contentTypes -replace "</Types>", '<Override PartName="/docProps/custom.xml" ContentType="application/vnd.openxmlformats-officedocument.custom-properties+xml"/></Types>'
            $contentTypesEntry.Delete()
            $contentTypesEntry = $zip.CreateEntry("[Content_Types].xml")
            $writer = New-Object System.IO.StreamWriter($contentTypesEntry.Open(), [System.Text.Encoding]::UTF8)
            try { $writer.Write($contentTypes) } finally { $writer.Dispose() }
        }

        $relsEntry = $zip.GetEntry("_rels/.rels")
        $reader = New-Object System.IO.StreamReader($relsEntry.Open(), [System.Text.Encoding]::UTF8)
        try { $rels = $reader.ReadToEnd() } finally { $reader.Dispose() }
        if ($rels -notmatch "custom-properties") {
            $rels = $rels -replace "</Relationships>", '<Relationship Id="rIdCustomDocProps" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties" Target="docProps/custom.xml"/></Relationships>'
            $relsEntry.Delete()
            $relsEntry = $zip.CreateEntry("_rels/.rels")
            $writer = New-Object System.IO.StreamWriter($relsEntry.Open(), [System.Text.Encoding]::UTF8)
            try { $writer.Write($rels) } finally { $writer.Dispose() }
        }
    }
    finally {
        $zip.Dispose()
    }
}

try {
    Set-AddInReleaseMetadata `
        -Path $output `
        -ProductName $productName `
        -ProductVersion $productVersion `
        -BuildVersion $BuildVersion `
        -ReleaseType $ReleaseType `
        -ManifestSchemaVersion $manifestSchemaVersion `
        -TemplateSchemaVersion $templateSchemaVersion `
        -MinimumSupportedVersion $minimumSupportedVersion `
        -BuildDate $buildDate
}
catch {
    Write-Error "Failed to record release metadata: $_"
    exit 1
}
Write-Host "Recorded release metadata: Product=$productName; Product Version=$productVersion; Build Version=$BuildVersion; Release Type=$ReleaseType"

Write-Host "Build script finished."
