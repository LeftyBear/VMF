# P4-03 - Manifest Derivation Focused Test Completion

## Status

COMPLETE / TESTED / VERIFIED

## Purpose

Complete focused test coverage for the P4-02 Manifest Derivation minimum local
implementation slice while preserving the P4-01 and P4-02 responsibility
boundaries.

## Existing Build Tests

Existing Build VBA tests are present under `tests/unit/Build`, and the shared
runner registers Build test entry points in `tools/test/runner/VMFTestRunner.bas`.

The Manifest Derivation focused test module remains under
`tests/unit/Build/AppBlueprintManifestDeriverTests.bas`.

## Test Result

P4-03 updates the Manifest Derivation focused tests to fix these boundaries:

- only Validator-output Validated Blueprint input may enter Manifest derivation
- Validator error diagnostics hard-stop before Manifest output
- Parser and Validator remain separate from Manifest conversion
- Manifest derivation remains after Validator and before Template,
  GenerateContext, and Generator
- incomplete, ambiguous, unsupported, unapproved, and non-generatable Blueprint
  input does not derive Manifest content

The VBA test component name and runner entry point were shortened to
`AppBlueprintManifestTests` so the Excel runner can invoke the test module
reliably. The source file path remains unchanged.

## Tooling Adjustment

`tools/test/setup-test-runner.ps1` now accepts optional `-BuildPath` so
local-only validation can use a temporary Build.xlam created from current
source. This avoids updating `dist/release` while still testing the P4-02 local
implementation.

## Verification Evidence

| Check | Result | Notes |
| --- | --- | --- |
| `powershell -ExecutionPolicy Bypass -File tools/build/build.ps1 -OutputPath tmp/p4-03/Build.xlam -BuildVersion p4-03-local` | PASS | Temporary local Build output only. |
| `powershell -ExecutionPolicy Bypass -File tools/test/setup-test-runner.ps1 -BuildPath tmp/p4-03/Build.xlam` | PASS | Runner generated from current local Build. |
| `powershell -ExecutionPolicy Bypass -File tools/test/run-tests.ps1` | PASS | Existing Build regression passed; `AppRunBlueprintManifestTests` passed. |
| `git diff --check` | PASS | LF-to-CRLF warnings only. |

The generated `tools/test/runner/VMFTestRunner.xlam` was removed after
verification. No package, `dist/release`, release, tag, push, publication, or
external service operation was performed.

## Boundary

P4-03 changes tests and local test tooling only. It does not change production
Manifest derivation behavior, Parser responsibility, Validator responsibility,
Template, GenerateContext, Generator, Frozen specifications, public contracts,
package artifacts, or `dist/release` artifacts.
