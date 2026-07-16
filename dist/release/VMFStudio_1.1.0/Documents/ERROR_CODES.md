# Error Codes

Product : VMF Studio
Version : 1.1.0
Status  : v1.1 design freeze inventory

Error codes are stable identifiers. Tests and UI logic should rely on codes rather than message text.

## Review Summary

- Manifest validation uses VMF-MOD-*, VMF-MEM-*, and VMF-VAL-*.
- Template validation uses VMF-TPL-*, VMF-TPL-PH-*, VMF-TPL-SEC-*, and VMF-TPL-CLS-*.
- Generation uses VMF-GEN-*.
- Settings uses VMF-SET-*.
- Self Check uses VMF-SC-*.
- Backup/Restore has structured result messages and history entries in v1.1.0; a dedicated VMF-BAK-* code family is reserved for v1.2 Candidate unless a bug fix requires it.

## Codes

### `VMF-GEN-RENDER`

- `src\Build\Application\AppManifestGenerateService.cls:227` - `Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Failed", "Render Error: unresolved placeholder.", "VMF-GEN-RENDER", Timer - StartedAt, False)`
- `src\Build\Application\AppManifestGenerateService.cls:240` - `Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Failed", ClassifyRenderError(Err.Source, Err.Description), "VMF-GEN-RENDER", Timer - StartedAt, False)`

### `VMF-GEN-SKIP`

- `src\Build\Application\AppManifestGenerateService.cls:213` - `Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Skipped", "Existing File Skip: existing file was not overwritten.", "VMF-GEN-SKIP", Timer - StartedAt, False)`
- `src\Build\Application\AppManifestGenerateService.cls:218` - `Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Skipped", "Existing File Skip: overwrite was not confirmed.", "VMF-GEN-SKIP", Timer - StartedAt, False)`

### `VMF-GEN-UNEXPECTED`

- `src\Build\Application\AppManifestGenerateService.cls:146` - `AddProjectFailure GenerateResult, GenerateResult.OutputDirectory, "VMF-GEN-UNEXPECTED", "Unexpected Error: " & Err.Description`
- `src\Build\Application\AppManifestGenerateService.cls:244` - `Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Failed", "Unexpected Error: " & Err.Description, "VMF-GEN-UNEXPECTED", Timer - StartedAt, False)`

### `VMF-GEN-WRITE`

- `src\Build\Application\AppManifestGenerateService.cls:114` - `AddProjectFailure GenerateResult, Request.OutputDirectory, "VMF-GEN-WRITE", "File Write Error: " & ValidationResult.Message`
- `src\Build\Application\AppManifestGenerateService.cls:233` - `Set GenerateOneModule = CreateModuleResult(ModuleInfo, OutputPath, "Failed", "File Write Error: " & Result.Message, "VMF-GEN-WRITE", Timer - StartedAt, False)`

### `VMF-MEM-000`

- `src\Build\Application\AppManifestValidationService.cls:243` - `AddIssue Issues, SeverityError, "VMF-MEM-000", "Member definition is required.", ModuleName, vbNullString, "Member", SortBase`

### `VMF-MEM-001`

- `src\Build\Application\AppManifestValidationService.cls:256` - `AddIssue Issues, SeverityError, "VMF-MEM-001", "Member.Name is required.", ModuleName, MemberName, "Name", SortBase + 1`

### `VMF-MEM-002`

- `src\Build\Application\AppManifestValidationService.cls:258` - `AddIssue Issues, SeverityError, "VMF-MEM-002", "Member.Name must be a valid VBA identifier.", ModuleName, MemberName, "Name", SortBase + 2`

### `VMF-MEM-003`

- `src\Build\Application\AppManifestValidationService.cls:217` - `AddIssue Issues, SeverityError, "VMF-MEM-003", "Duplicate member name: " & CStr(GetValue(MemberInfo, "Name")), ModuleName, CStr(GetValue(MemberInfo, "Name")), "Name", SortBase + MemberIndex * 100 + 3`

### `VMF-MEM-004`

- `src\Build\Application\AppManifestValidationService.cls:262` - `AddIssue Issues, SeverityError, "VMF-MEM-004", "Member.TypeName is required.", ModuleName, MemberName, "TypeName", SortBase + 4`

### `VMF-MEM-005`

- `src\Build\Application\AppManifestValidationService.cls:266` - `AddIssue Issues, SeverityError, "VMF-MEM-005", "Accessor must be GetLet, GetSet, or GetOnly.", ModuleName, MemberName, "Accessor", SortBase + 5`

### `VMF-MEM-006`

- `src\Build\Application\AppManifestValidationService.cls:270` - `AddIssue Issues, SeverityError, "VMF-MEM-006", "CreateInstance must be True or False.", ModuleName, MemberName, "CreateInstance", SortBase + 6`

### `VMF-MEM-007`

- `src\Build\Application\AppManifestValidationService.cls:274` - `AddIssue Issues, SeverityError, "VMF-MEM-007", "IsObject must be True or False.", ModuleName, MemberName, "IsObject", SortBase + 7`

### `VMF-MEM-008`

- `src\Build\Application\AppManifestValidationService.cls:278` - `AddIssue Issues, SeverityError, "VMF-MEM-008", "ReadOnly must be True or False.", ModuleName, MemberName, "ReadOnly", SortBase + 8`

### `VMF-MEM-009`

- `src\Build\Application\AppManifestValidationService.cls:283` - `AddIssue Issues, SeverityError, "VMF-MEM-009", "Object members must not use Property Let.", ModuleName, MemberName, "Accessor", SortBase + 9`
- `tests\unit\Build\AppManifestEditorServiceTests.bas:179` - `AssertIssueCode Issues, "VMF-MEM-009"`

### `VMF-MEM-010`

- `src\Build\Application\AppManifestValidationService.cls:287` - `AddIssue Issues, SeverityError, "VMF-MEM-010", "Value members must not use Property Set.", ModuleName, MemberName, "Accessor", SortBase + 10`

### `VMF-MEM-011`

- `src\Build\Application\AppManifestValidationService.cls:291` - `AddIssue Issues, SeverityError, "VMF-MEM-011", "ReadOnly members must use GetOnly.", ModuleName, MemberName, "ReadOnly", SortBase + 11`

### `VMF-MEM-012`

- `src\Build\Application\AppManifestValidationService.cls:296` - `AddIssue Issues, SeverityError, "VMF-MEM-012", "CreateInstance=True requires IsObject=True.", ModuleName, MemberName, "CreateInstance", SortBase + 12`

### `VMF-MEM-013`

- `src\Build\Application\AppManifestValidationService.cls:300` - `AddIssue Issues, SeverityWarning, "VMF-MEM-013", "InitialValue is ignored when CreateInstance=True.", ModuleName, MemberName, "InitialValue", SortBase + 13`
- `tests\unit\Build\AppManifestEditorServiceTests.bas:220` - `AssertIssueCode Issues, "VMF-MEM-013"`

### `VMF-MOD-000`

- `src\Build\Application\AppManifestValidationService.cls:117` - `AddIssue Issues, SeverityError, "VMF-MOD-000", "At least one module is required.", vbNullString, vbNullString, "Modules", SortOffset + 1`
- `src\Build\Application\AppManifestValidationService.cls:122` - `AddIssue Issues, SeverityError, "VMF-MOD-000", "At least one module is required.", vbNullString, vbNullString, "Modules", SortOffset + 1`

### `VMF-MOD-001`

- `src\Build\Application\AppManifestValidationService.cls:157` - `AddIssue Issues, SeverityError, "VMF-MOD-001", "Module definition is required.", vbNullString, vbNullString, "Module", SortBase + 1`
- `src\Build\Application\AppManifestValidationService.cls:167` - `AddIssue Issues, SeverityError, "VMF-MOD-001", "ModuleName is required.", ModuleName, vbNullString, "ModuleName", SortBase + 1`

### `VMF-MOD-002`

- `src\Build\Application\AppManifestValidationService.cls:169` - `AddIssue Issues, SeverityError, "VMF-MOD-002", "ModuleName must be a valid VBA identifier.", ModuleName, vbNullString, "ModuleName", SortBase + 2`
- `tests\unit\Build\AppManifestEditorServiceTests.bas:178` - `AssertIssueCode Issues, "VMF-MOD-002"`

### `VMF-MOD-003`

- `src\Build\Application\AppManifestValidationService.cls:135` - `AddIssue Issues, SeverityError, "VMF-MOD-003", "Duplicate ModuleName: " & CStr(GetValue(ModuleInfo, "ModuleName")), CStr(GetValue(ModuleInfo, "ModuleName")), vbNullString, "ModuleName", SortOffset + ModuleIndex * 1000 + 3`

### `VMF-MOD-004`

- `src\Build\Application\AppManifestValidationService.cls:173` - `AddIssue Issues, SeverityError, "VMF-MOD-004", "Layer is required.", ModuleName, vbNullString, "Layer", SortBase + 4`

### `VMF-MOD-005`

- `src\Build\Application\AppManifestValidationService.cls:175` - `AddIssue Issues, SeverityError, "VMF-MOD-005", "Layer must be a valid identifier.", ModuleName, vbNullString, "Layer", SortBase + 5`

### `VMF-MOD-006`

- `src\Build\Application\AppManifestValidationService.cls:179` - `AddIssue Issues, SeverityError, "VMF-MOD-006", "ModuleType is required.", ModuleName, vbNullString, "ModuleType", SortBase + 6`

### `VMF-MOD-007`

- `src\Build\Application\AppManifestValidationService.cls:181` - `AddIssue Issues, SeverityError, "VMF-MOD-007", "ModuleType must be StandardModule or ClassModule.", ModuleName, vbNullString, "ModuleType", SortBase + 7`

### `VMF-MOD-008`

- `src\Build\Application\AppManifestValidationService.cls:185` - `AddIssue Issues, SeverityError, "VMF-MOD-008", "TemplatePath is required.", ModuleName, vbNullString, "TemplatePath", SortBase + 8`

### `VMF-MOD-009`

- `src\Build\Application\AppManifestValidationService.cls:189` - `AddIssue Issues, SeverityError, "VMF-MOD-009", "TemplatePath does not exist: " & TemplatePath, ModuleName, vbNullString, "TemplatePath", SortBase + 9`

### `VMF-MOD-010`

- `src\Build\Application\AppManifestValidationService.cls:328` - `AddIssue Issues, SeverityError, "VMF-MOD-010", "Template contains an unresolved placeholder.", CStr(GetValue(ModuleInfo, "ModuleName")), vbNullString, "TemplatePath", SortBase + SortIndex`
- `src\Build\Application\AppManifestValidationService.cls:336` - `AddIssue Issues, SeverityError, "VMF-MOD-010", "Template contains an unresolved placeholder.", CStr(GetValue(ModuleInfo, "ModuleName")), vbNullString, "TemplatePath", SortBase + SortIndex`
- `src\Build\Application\AppManifestValidationService.cls:346` - `AddIssue Issues, SeverityError, "VMF-MOD-010", "Template placeholder is not supported: " & Token, CStr(GetValue(ModuleInfo, "ModuleName")), vbNullString, "TemplatePath", SortBase + SortIndex`

### `VMF-MOD-011`

- `src\Build\Application\AppManifestValidationService.cls:353` - `AddIssue Issues, SeverityError, "VMF-MOD-011", "Template could not be read: " & Err.Description, CStr(GetValue(ModuleInfo, "ModuleName")), vbNullString, "TemplatePath", SortBase`

### `VMF-MOD-012`

- `src\Build\Application\AppManifestValidationService.cls:371` - `AddIssue Issues, SeverityError, "VMF-MOD-012", Result.Failure.Description, ModuleName, vbNullString, "TemplatePath", SortBase`
- `src\Build\Application\AppManifestValidationService.cls:380` - `AddIssue Issues, SeverityError, "VMF-MOD-012", "Template section does not exist: " & CStr(SectionName), ModuleName, vbNullString, "SectionSources", SortBase + TemplateSections.Count + 1`

### `VMF-SC-BK-001`

- `src\Build\Application\AppSelfCheckService.cls:319` - `Result.AppFinish "Failed", "Backup restore failed.", "Restore success", CheckResult.Message, "VMF-SC-BK-001"`

### `VMF-SC-DIR-001`

- `src\Build\Application\AppSelfCheckService.cls:116` - `Result.AppFinish "Failed", "Required directories are not accessible.", "Accessible directories", "Missing", "VMF-SC-DIR-001"`

### `VMF-SC-MF-001`

- `src\Build\Application\AppSelfCheckService.cls:139` - `Result.AppFinish "Failed", "Member count mismatch after reload.", "1 member", CStr(Reloaded.Item(1)("Members").Count), "VMF-SC-MF-001"`

### `VMF-SC-MF-002`

- `src\Build\Application\AppSelfCheckService.cls:159` - `Result.AppFinish "Failed", "Invalid manifest was not detected.", "Validation error", "No error", "VMF-SC-MF-002"`

### `VMF-SC-PG-001`

- `src\Build\Application\AppSelfCheckService.cls:225` - `Result.AppFinish "Failed", "Preview and generate output differ.", "Equal code", "Different code", "VMF-SC-PG-001"`

### `VMF-SC-PG-002`

- `src\Build\Application\AppSelfCheckService.cls:247` - `Result.AppFinish "Failed", "Unresolved placeholder did not fail preview.", "Preview failure", "Preview success", "VMF-SC-PG-002"`

### `VMF-SC-SET-001`

- `src\Build\Application\AppSelfCheckService.cls:106` - `Result.AppFinish "Failed", "Settings did not load.", "Settings model", "Nothing", "VMF-SC-SET-001"`

### `VMF-SC-ST-001`

- `src\Build\Application\AppSelfCheckService.cls:287` - `Result.AppFinish "Failed", "Settings reload mismatch.", "Skip", Reloaded.DefaultOverwriteMode, "VMF-SC-ST-001"`

### `VMF-SC-ST-002`

- `src\Build\Application\AppSelfCheckService.cls:301` - `Result.AppFinish "Failed", "Unknown/bad settings were not reported.", "Warning", "No issue", "VMF-SC-ST-002"`

### `VMF-SC-TP-001`

- `src\Build\Application\AppSelfCheckService.cls:172` - `Result.AppFinish "Failed", "Template analysis returned empty results.", "Non-empty analysis", CStr(Template.Placeholders.Count) & "/" & CStr(Template.Sections.Count), "VMF-SC-TP-001"`

### `VMF-SC-TP-002`

- `src\Build\Application\AppSelfCheckService.cls:186` - `Result.AppFinish "Failed", "Invalid template was not detected.", "Template validation error", "No error", "VMF-SC-TP-002"`

### `VMF-SC-UNEXPECTED`

- `src\Build\Application\AppSelfCheckService.cls:77` - `Result.AppFinish "Failed", "Unexpected Error: " & Err.Description, "No unexpected error", Err.Description, "VMF-SC-UNEXPECTED"`

### `VMF-SC-VL-001`

- `src\Build\Application\AppSelfCheckService.cls:269` - `Result.AppFinish "Failed", "Validation rules did not detect errors.", "Validation error", "No error", "VMF-SC-VL-001"`

### `VMF-SET-000`

- `src\Build\Application\AppStudioSettingsService.cls:66` - `AddIssue Issues, SeverityInformation, "VMF-SET-000", "Settings file does not exist. Defaults were used.", "SettingsFile", 1`

### `VMF-SET-001`

- `src\Build\Application\AppStudioSettingsService.cls:280` - `AddIssue Issues, SeverityError, "VMF-SET-001", "Settings are required.", "Settings", 1`

### `VMF-SET-BKP-001`

- `src\Build\Application\AppStudioSettingsService.cls:290` - `AddIssue Issues, SeverityError, "VMF-SET-BKP-001", "MaxBackupCountPerFile must be between 1 and 999.", "MaxBackupCountPerFile", 60`

### `VMF-SET-BKP-002`

- `src\Build\Application\AppStudioSettingsService.cls:293` - `AddIssue Issues, SeverityError, "VMF-SET-BKP-002", "MaxBackupAgeDays must be between 1 and 3650.", "MaxBackupAgeDays", 61`

### `VMF-SET-EDT-001`

- `src\Build\Application\AppStudioSettingsService.cls:303` - `AddIssue Issues, SeverityError, "VMF-SET-EDT-001", "PreviewFontName is required.", "PreviewFontName", 200`

### `VMF-SET-EDT-002`

- `src\Build\Application\AppStudioSettingsService.cls:306` - `AddIssue Issues, SeverityError, "VMF-SET-EDT-002", "PreviewFontSize must be between 6 and 24.", "PreviewFontSize", 201`
- `tests\unit\Build\AppManifestEditorServiceTests.bas:417` - `AssertStudioSettingIssueCode Issues, "VMF-SET-EDT-002"`

### `VMF-SET-GEN-001`

- `src\Build\Application\AppStudioSettingsService.cls:297` - `AddIssue Issues, SeverityError, "VMF-SET-GEN-001", "DefaultTargetScope is not supported.", "DefaultTargetScope", 100`

### `VMF-SET-GEN-002`

- `src\Build\Application\AppStudioSettingsService.cls:300` - `AddIssue Issues, SeverityError, "VMF-SET-GEN-002", "DefaultOverwriteMode is not supported.", "DefaultOverwriteMode", 101`

### `VMF-SET-LOAD-001`

- `src\Build\Application\AppStudioSettingsService.cls:80` - `AddIssue Issues, SeverityWarning, "VMF-SET-LOAD-001", "Settings file could not be loaded. Defaults were used: " & Err.Description, "SettingsFile", 1`
- `src\Build\Application\AppStudioSettingsService.cls:102` - `AddIssue Issues, SeverityWarning, "VMF-SET-LOAD-001", "Settings file could not be loaded. Defaults were used: " & Err.Description, "SettingsFile", 1`

### `VMF-SET-LOAD-002`

- `src\Build\Application\AppStudioSettingsService.cls:90` - `AddIssue Issues, SeverityError, "VMF-SET-LOAD-002", "Settings file does not exist: " & SettingsPath, "SettingsFile", 1`

### `VMF-SET-PARSE-001`

- `src\Build\Application\AppStudioSettingsService.cls:225` - `AddIssue Issues, SeverityWarning, "VMF-SET-PARSE-001", "Settings line is invalid and was ignored.", "Line" & CStr(LineIndex + 1), 100 + LineIndex`

### `VMF-SET-PARSE-002`

- `src\Build\Application\AppStudioSettingsService.cls:232` - `AddIssue Issues, SeverityWarning, "VMF-SET-PARSE-002", "Duplicate setting key was ignored: " & KeyName, KeyName, 200 + LineIndex`
- `tests\unit\Build\AppManifestEditorServiceTests.bas:452` - `If Issue.Code = "VMF-SET-PARSE-002" Then HasDuplicateWarning = True`

### `VMF-SET-PARSE-003`

- `src\Build\Application\AppStudioSettingsService.cls:270` - `AddIssue Issues, SeverityWarning, "VMF-SET-PARSE-003", "Unknown setting key was ignored: " & KeyName, KeyName, 300 + LineNumber`
- `tests\unit\Build\AppManifestEditorServiceTests.bas:451` - `If Issue.Code = "VMF-SET-PARSE-003" Then HasUnknownKeyWarning = True`

### `VMF-SET-PARSE-004`

- `src\Build\Application\AppStudioSettingsService.cls:275` - `AddIssue Issues, SeverityWarning, "VMF-SET-PARSE-004", "Setting value conversion failed: " & KeyName, KeyName, 400 + LineNumber`

### `VMF-SET-PATH-001`

- `src\Build\Application\AppStudioSettingsService.cls:319` - `AddIssue Issues, SeverityError, "VMF-SET-PATH-001", KeyName & " is required.", KeyName, SortOrder`
- `src\Build\Application\AppStudioSettingsService.cls:332` - `AddIssue Issues, SeverityError, "VMF-SET-PATH-001", KeyName & " is required.", KeyName, SortOrder`

### `VMF-SET-PATH-002`

- `src\Build\Application\AppStudioSettingsService.cls:325` - `AddIssue Issues, SeverityError, "VMF-SET-PATH-002", KeyName & " does not exist: " & ResolvedPath, KeyName, SortOrder + 1`

### `VMF-SET-PATH-003`

- `src\Build\Application\AppStudioSettingsService.cls:344` - `AddIssue Issues, SeverityError, "VMF-SET-PATH-003", KeyName & " is not writable: " & ResolvedPath, KeyName, SortOrder + 1`

### `VMF-SET-PATH-004`

- `src\Build\Application\AppStudioSettingsService.cls:339` - `AddIssue Issues, SeverityError, "VMF-SET-PATH-004", KeyName & " cannot be created: " & ResolvedPath, KeyName, SortOrder + 2`

### `VMF-SET-PATH-005`

- `src\Build\Application\AppStudioSettingsService.cls:341` - `AddIssue Issues, SeverityWarning, "VMF-SET-PATH-005", KeyName & " does not exist and will be created when needed: " & ResolvedPath, KeyName, SortOrder + 1`

### `VMF-SET-SAVE-001`

- `src\Build\Application\AppStudioSettingsService.cls:139` - `AddIssue Issues, SeverityError, "VMF-SET-SAVE-001", "Settings folder could not be created: " & Result.Message, "SettingsFile", 900`

### `VMF-SET-STU-001`

- `src\Build\Application\AppStudioSettingsService.cls:309` - `AddIssue Issues, SeverityWarning, "VMF-SET-STU-001", "WindowWidth is outside the recommended range.", "WindowWidth", 300`

### `VMF-SET-STU-002`

- `src\Build\Application\AppStudioSettingsService.cls:312` - `AddIssue Issues, SeverityWarning, "VMF-SET-STU-002", "WindowHeight is outside the recommended range.", "WindowHeight", 301`

### `VMF-SET-VAL-001`

- `src\Build\Application\AppStudioSettingsService.cls:115` - `AddIssue Issues, SeverityError, "VMF-SET-VAL-001", "Settings validation failed: " & Err.Description, "Internal", 1`

### `VMF-TPL-000`

- `src\Build\Application\AppTemplateService.cls:139` - `AddIssue Issues, SeverityError, "VMF-TPL-000", "Template validation failed: " & Err.Description, vbNullString, vbNullString, 0, 1`

### `VMF-TPL-001`

- `src\Build\Application\AppTemplateService.cls:294` - `AddIssue Issues, SeverityError, "VMF-TPL-001", "Template is required.", vbNullString, vbNullString, 0, 1`
- `src\Build\Application\AppTemplateService.cls:303` - `AddIssue Issues, SeverityError, "VMF-TPL-001", "Template content is empty.", Template.TemplateName, "Content", 0, 1`

### `VMF-TPL-002`

- `src\Build\Application\AppTemplateService.cls:306` - `AddIssue Issues, SeverityError, "VMF-TPL-002", "TemplateType is required.", Template.TemplateName, "TemplateType", 0, 2`

### `VMF-TPL-007`

- `src\Build\Application\AppTemplateService.cls:309` - `AddIssue Issues, SeverityWarning, "VMF-TPL-007", "Template line endings are not CRLF.", Template.TemplateName, "LineEnding", 0, 7`

### `VMF-TPL-CLS-001`

- `src\Build\Application\AppTemplateService.cls:338` - `AddIssue Issues, SeverityError, "VMF-TPL-CLS-001", "Class template requires Option Explicit.", Template.TemplateName, "Option Explicit", 0, 500`

### `VMF-TPL-CLS-002`

- `src\Build\Application\AppTemplateService.cls:341` - `AddIssue Issues, SeverityError, "VMF-TPL-CLS-002", "Class template requires ModuleName.", Template.TemplateName, "ModuleName", 0, 501`

### `VMF-TPL-CLS-003`

- `src\Build\Application\AppTemplateService.cls:344` - `AddIssue Issues, SeverityError, "VMF-TPL-CLS-003", "Class template requires BODY insertion point.", Template.TemplateName, "BODY", 0, 502`

### `VMF-TPL-CLS-004`

- `src\Build\Application\AppTemplateService.cls:347` - `AddIssue Issues, SeverityWarning, "VMF-TPL-CLS-004", "Class template has no member declaration insertion point.", Template.TemplateName, "MemberBlock", 0, 503`

### `VMF-TPL-CLS-005`

- `src\Build\Application\AppTemplateService.cls:350` - `AddIssue Issues, SeverityWarning, "VMF-TPL-CLS-005", "Class template has no property insertion point.", Template.TemplateName, "Properties", 0, 504`

### `VMF-TPL-PH-001`

- `src\Build\Application\AppTemplateService.cls:314` - `AddIssue Issues, SeverityError, "VMF-TPL-PH-001", "Unknown placeholder: " & CStr(Placeholder("Name")), Template.TemplateName, CStr(Placeholder("Name")), CLng(Placeholder("LineNumber")), 100 + CLng(Placeholder("LineNumber"))`

### `VMF-TPL-PH-002`

- `src\Build\Application\AppTemplateService.cls:320` - `AddIssue Issues, SeverityError, "VMF-TPL-PH-002", "Required placeholder is missing: " & CStr(RequiredName), Template.TemplateName, CStr(RequiredName), 0, 200 + SortIndex`

### `VMF-TPL-SEC-001`

- `src\Build\Application\AppTemplateService.cls:423` - `Sections.Add CreateSectionInfo(vbNullString, LineIndex + 1, False, "VMF-TPL-SEC-001", "@endsection has no matching @section.")`

### `VMF-TPL-SEC-002`

- `src\Build\Application\AppTemplateService.cls:410` - `Sections.Add CreateSectionInfo(vbNullString, LineIndex + 1, False, "VMF-TPL-SEC-002", "Section name is required.")`

### `VMF-TPL-SEC-003`

- `src\Build\Application\AppTemplateService.cls:414` - `Sections.Add CreateSectionInfo(SectionName, LineIndex + 1, False, "VMF-TPL-SEC-003", "Duplicate section name: " & SectionName)`

### `VMF-TPL-SEC-004`

- `src\Build\Application\AppTemplateService.cls:412` - `Sections.Add CreateSectionInfo(SectionName, LineIndex + 1, False, "VMF-TPL-SEC-004", "Nested sections are not supported.")`

### `VMF-TPL-SEC-005`

- `src\Build\Application\AppTemplateService.cls:431` - `Sections.Add CreateSectionInfo(ActiveName, UBound(Lines) + 1, False, "VMF-TPL-SEC-005", "Section is not closed: " & ActiveName)`

### `VMF-VAL-000`

- `src\Build\Presentation\PreManifestEditorForm.frm:2237` - `ValidationListBox.List(0, 1) = "VMF-VAL-000"`
- `src\Build\Presentation\PreManifestEditorForm.frm:2246` - `ValidationListBox.List(0, 1) = "VMF-VAL-000"`

### `VMF-VAL-001`

- `src\Build\Application\AppManifestValidationService.cls:49` - `AddIssue Issues, SeverityError, "VMF-VAL-001", Err.Description, vbNullString, vbNullString, "Internal", 1`
- `src\Build\Application\AppManifestValidationService.cls:74` - `AddIssue Issues, SeverityError, "VMF-VAL-001", Err.Description, vbNullString, vbNullString, "Internal", 1`

