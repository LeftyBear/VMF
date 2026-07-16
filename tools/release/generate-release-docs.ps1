# Generates release inventory documents from source declarations.

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$root = Split-Path -Parent (Split-Path -Parent $scriptDir)
Set-Location $root

function Resolve-RelativePath {
    param([string]$Path)

    $full = [IO.Path]::GetFullPath($Path)
    if ($full.StartsWith($root, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $full.Substring($root.Length + 1)
    }

    return $full
}

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

$releaseInfoPath = Join-Path $root "src\Build\Application\AppReleaseInfo.bas"
$productName = Get-ReleaseInfoValue -SourcePath $releaseInfoPath -ConstantName "RELEASE_PRODUCT_NAME"
$productVersion = Get-ReleaseInfoValue -SourcePath $releaseInfoPath -ConstantName "RELEASE_PRODUCT_VERSION"

$apiLines = Get-ChildItem "src\Build" -Recurse -Include *.bas, *.cls, *.frm |
    Sort-Object FullName |
    ForEach-Object {
        $rel = Resolve-RelativePath -Path $_.FullName
        Select-String -Path $_.FullName -Encoding Default -Pattern "^(Public|Global) (Sub|Function|Property|Enum|Const)|^Public .* As" |
            ForEach-Object {
                [pscustomobject]@{
                    File = $rel
                    Line = $_.LineNumber
                    Text = $_.Line.Trim()
                }
            }
    }

$apiText = New-Object System.Collections.Generic.List[string]
$apiText.Add("# Public API")
$apiText.Add("")
$apiText.Add("Product : $productName")
$apiText.Add("Version : $productVersion")
$apiText.Add("Status  : v1.1 design freeze inventory")
$apiText.Add("")
$apiText.Add("This document lists Public VBA members exported by `src/Build` at the $productName v$productVersion release freeze. Public members are compatibility-sensitive unless explicitly documented as internal factory/model support.")
$apiText.Add("")
$apiText.Add("## Review Summary")
$apiText.Add("")
$apiText.Add("- The stable external entry points are the facade modules: `ComFacade`, `DomFacade`, `InfFacade`, `AppFacade`, and `PreFacade`.")
$apiText.Add("- Structured result/model classes expose Public initialization methods and properties so UI and services can exchange typed data without parsing display strings.")
$apiText.Add("- No v1.0 public API removals were made for v$productVersion.")
$apiText.Add("- UI-only behavior remains concentrated in Presentation; `PreManifestEditorForm.PreOpenManifest` is public only for form bootstrapping.")
$apiText.Add("")
$apiText.Add("## Public Members")
$apiText.Add("")

foreach ($group in $apiLines | Group-Object File) {
    $apiText.Add("### ``$($group.Name)``")
    $apiText.Add("")
    foreach ($item in $group.Group) {
        $apiText.Add(("- Line {0}: ``{1}``" -f $item.Line, $item.Text))
    }
    $apiText.Add("")
}

Set-Content -Path "PUBLIC_API.md" -Encoding UTF8 -Value $apiText

$codeRegex = "VMF-[A-Z]+(?:-[A-Z]+)*-[0-9]{3}|VMF-GEN-SKIP|VMF-GEN-RENDER|VMF-GEN-WRITE|VMF-GEN-UNEXPECTED|VMF-SC-UNEXPECTED"
$codeMatches = Get-ChildItem src, tests -Recurse -Include *.bas, *.cls, *.frm |
    Sort-Object FullName |
    ForEach-Object {
        $rel = Resolve-RelativePath -Path $_.FullName
        Select-String -Path $_.FullName -Encoding Default -Pattern $codeRegex -AllMatches |
            ForEach-Object {
                foreach ($match in $_.Matches) {
                    [pscustomobject]@{
                        Code = $match.Value
                        File = $rel
                        Line = $_.LineNumber
                        Text = $_.Line.Trim()
                    }
                }
            }
    }

$errText = New-Object System.Collections.Generic.List[string]
$errText.Add("# Error Codes")
$errText.Add("")
$errText.Add("Product : $productName")
$errText.Add("Version : $productVersion")
$errText.Add("Status  : v1.1 design freeze inventory")
$errText.Add("")
$errText.Add("Error codes are stable identifiers. Tests and UI logic should rely on codes rather than message text.")
$errText.Add("")
$errText.Add("## Review Summary")
$errText.Add("")
$errText.Add("- Manifest validation uses `VMF-MOD-*`, `VMF-MEM-*`, and `VMF-VAL-*`.")
$errText.Add("- Template validation uses `VMF-TPL-*`, `VMF-TPL-PH-*`, `VMF-TPL-SEC-*`, and `VMF-TPL-CLS-*`.")
$errText.Add("- Generation uses `VMF-GEN-*`.")
$errText.Add("- Settings uses `VMF-SET-*`.")
$errText.Add("- Self Check uses `VMF-SC-*`.")
$errText.Add("- Backup/Restore has structured result messages and history entries in v$productVersion; a dedicated `VMF-BAK-*` code family is reserved for v1.2 Candidate unless a bug fix requires it.")
$errText.Add("")
$errText.Add("## Codes")
$errText.Add("")

foreach ($group in $codeMatches | Sort-Object Code, File, Line | Group-Object Code) {
    $errText.Add("### ``$($group.Name)``")
    $errText.Add("")
    foreach ($item in $group.Group) {
        $errText.Add(("- ``{0}:{1}`` - ``{2}``" -f $item.File, $item.Line, $item.Text))
    }
    $errText.Add("")
}

Set-Content -Path "ERROR_CODES.md" -Encoding UTF8 -Value $errText
