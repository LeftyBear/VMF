# P5-09 - GenerateContext Focused Test Implementation Start

## Status

STARTED / local-only implementation scope

## Purpose

Start the local-only implementation phase for the P5-07 GenerateContext
focused test design, using the P5-08 implementation scope planning record as
the decision boundary.

P5-09 is not docs-only. It is a local-only focused test implementation scope.
This start record does not itself add or change production code or test code.

## Scope

P5-09 may proceed only within the local-only focused test implementation
boundary fixed by P5-08:

- identify the current GenerateContext entry boundary from the codebase before
  any code edit
- identify the exact focused test file or runner registration target before
  any test edit
- implement focused local tests for successful GenerateContext construction
  from complete, approved, generatable Template Derivation output or an
  approved narrow local equivalent
- implement focused local tests for GenerateContext hard-stop classifications
  before Generator
- preserve Parser, Validator, Manifest Derivation, Template Derivation,
  GenerateContext, and Generator responsibility separation

Production GenerateContext code changes are allowed only if they are separately
identified as required by the focused tests and remain limited to the approved
GenerateContext entry boundary.

## Preserved Boundaries

P5-09 preserves the P5-04 through P5-08 boundaries:

- unsupported, non-generatable, ambiguous, incomplete, unapproved,
  fallback-derived, or implicitly selected Template candidates hard-stop before
  GenerateContext and Generator
- GenerateContext must not select Templates by fallback, implicit selection,
  Template contents, Generator behavior, generated output, or runtime state
- GenerateContext must not infer, repair, normalize, or complete missing
  upstream Template Derivation or Manifest facts
- GenerateContext must not consume raw Blueprint text, unvalidated parsed
  Blueprint state, Validator diagnostics, Manifest Derivation diagnostics,
  Template file contents, Generator behavior, generated VBA output, or external
  runtime state as design input
- hard-stop cases must produce no Generator input, Generator invocation,
  runtime generation, output writes, package artifacts, or `dist` artifacts

## Non-Scope

P5-09 does not authorize:

- fallback Template selection
- implicit Template selection
- Template content inference
- GenerateContext or Generator downstream compensation
- Parser behavior changes
- Validator behavior changes
- Manifest Derivation behavior changes
- Template Derivation behavior changes
- Template file changes
- Generator invocation or behavior changes
- runtime generation behavior changes
- public API changes
- persisted schema changes
- canonical format changes
- Frozen specification changes
- package, `dist`, release, tag, publication, or external service operations

## Immediate Next Action

The next P5-09 action is local codebase inspection for the exact
GenerateContext entry boundary and focused test target. If that inspection
cannot identify a narrow implementation surface without violating the preserved
boundaries, P5-09 must stop before code or test edits.

## Verification Performed

This start record verification is documentation/status only:

- reviewed `docs/VMF_vNext_Backlog.md`
- reviewed `docs/development/CURRENT_STATUS.md`
- reviewed P5-08 GenerateContext Focused Test Implementation Scope Planning
- confirmed P5-09 was not yet registered in backlog or current status
- recorded P5-09 as local-only implementation scope, not docs-only

Required post-edit verification:

- `git diff --check`
- docs/status diff confirmation
