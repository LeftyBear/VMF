# VMF Studio Manifest Compatibility Notes

Product : VMF Studio
Version : 1.1.0
ManifestSchemaVersion : 1.0
Status : Compatibility note, not a replacement for frozen VMF v1.0 specs.

---

## Scope

VMF Studio v1.1 consumes the existing Build manifest inputs used by the
generator. This note documents Studio behavior for release verification without
changing `specs/vmf/VMF_v1.0.md` or frozen Build v1.0.x specifications.

---

## Supported Inputs

- Build module manifests under `src/Build/*.manifest`.
- Application manifests such as `applications/SchoolTimetable/manifest.yaml`
  through the existing project manifest tooling.
- In-memory Manifest Editor module/member models before they are saved.

---

## Module Fields

- `ModuleName`
- `ModuleType`
- `LayerName` or `Layer`
- `TemplatePath`
- Optional body, section, and member source references where supported by the
  existing manifest provider.

---

## Member Fields

- `Name`
- `TypeName`
- `Accessor`
- `InitialValue`
- `CreateInstance`
- Optional `IsObject`, `ReadOnly`, Initialize, and Terminate markers where
  present in the editor model.

---

## Compatibility Rules

- v1.0 manifests must remain readable.
- Field order differences must not be release blockers when the existing parser
  can resolve the required fields.
- Unknown future fields should be ignored or surfaced as warnings where
  possible.
- Studio validation must not introduce a generated-code format change by
  itself.
- Save, Preview, and Generate must use the same validator entry points.

---

## Release Verification

The v1.1.0 release verified manifest load/save, invalid manifest detection,
preview/generate consistency, full generation, selected generation, and
SchoolTimetable manifest compatibility through Self Check and regression tests.
