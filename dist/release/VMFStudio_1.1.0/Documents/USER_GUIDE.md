# VMF Studio User Guide

Product : VMF Studio
Version : 1.1.0

---

## 1. Start VMF Studio

Open Excel with the VMF Studio add-in installed. Use the VMF/Build ribbon entry
or the exposed Presentation entry point to open the Manifest Editor.

---

## 2. Open A Project

Use Browse in the Manifest Editor and select a manifest file. The Project
Explorer shows modules grouped by layer and marks validation/dirty state.

---

## 3. Edit Manifest

Select a module in Project Explorer. Edit ModuleName, Layer, ModuleType, and
TemplatePath in the Manifest area. Unsaved edits are reflected in the in-memory
model and marked dirty.

---

## 4. Add, Edit, Or Delete Members

Use the member fields to enter Name, TypeName, Accessor, InitialValue, and
CreateInstance. Use Add, Edit, and Delete to update the selected module's
member list.

---

## 5. Validate

Use Validate to run the shared Manifest Validator. Errors stop save, preview,
and generate. Warnings are shown separately from errors.

---

## 6. Preview

Select a module and use Preview. The preview uses the unsaved in-memory manifest
and the same render path as generation. Preview does not save the manifest or
write generated files.

---

## 7. Generate

Use Generate All for every module or Generate Selected for selected targets.
Generation runs project validation first and writes structured Build Log rows
for generated, skipped, warning, and failed modules.

---

## 8. Edit Templates

Open Template Manager to view templates, placeholders, sections, and validation
results. Save validates before replacing the existing template file. Preview can
use unsaved template content with the selected module.

---

## 9. Settings

Open Settings to edit paths, generate defaults, preview font settings, backup
retention, and Studio UI state. Cancel discards changes; Apply saves and
applies valid settings.

---

## 10. Restore From Backup

Open Backup/Restore, filter by target type, select a backup, and restore. The
current file is backed up before replacement and the restored content is
validated before it is written.

---

## 11. Self Check

Open Self Check and run all checks. Self Check uses temporary files and the
production services to verify manifest, template, preview/generate, validation,
settings, and backup/restore behavior.

---

## 12. Troubleshooting

- If validation fails, open Validation Results and fix the listed Module or
  Member.
- If preview fails, check template placeholders and section names.
- If generation skips files, check overwrite mode and existing output files.
- If settings fail to save, confirm that the settings folder is writable.
- If backup restore fails, validate the backup content and check the backup
  directory.
- If Excel blocks macros or VBE access, add the repository/test runner folder
  as a trusted location according to local policy.
