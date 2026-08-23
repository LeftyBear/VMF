# P6-01 - Generator Output Write Boundary Planning

## Status

COMPLETE / docs-only boundary planning

## Purpose

Start P6-01 after P5-15 by fixing the Generator output-write boundary before
any generated output write or target project mutation is considered.

P6-01 is docs-only. It records the allowed planning scope, required
preconditions, hard-stop conditions, and GO / NO-GO boundary for future
output-write work. It does not authorize local-only implementation.

## Scope

P6-01 records:

- upstream boundary:
  P5-15 Named Downstream Candidate Selection, commit
  `c919011a79ee49609e87efb3131ee9bf998e00cf`
- formal title:
  `Generator Output Write Boundary Planning`
- current classification:
  `docs-only boundary planning`
- implementation decision:
  `NO-GO until a later task explicitly authorizes a narrow local-only
  implementation scope`

## Decision

The Generator output-write boundary starts only after successful Generator
output has been produced from complete, successful GenerateContext input.

Output write is a separate downstream responsibility from Template Derivation,
GenerateContext construction, and Generator output construction. A future
implementation may be considered only after it identifies exact editable source
files, focused test files, target project mutation controls, acceptance
criteria, and verification commands.

No P6-01 production or test implementation is authorized. Local-only
implementation remains NO-GO.

## Output Write Boundary

GO for this docs-only record:

- define output-write as a post-Generator boundary
- require complete successful GenerateContext input before Generator output
- require successful Generator output before any write planning can proceed
- distinguish generated output construction from target project mutation
- record future hard-stop conditions before any write or mutation

NO-GO for this docs-only record:

- write generated output to disk, workbook, VBA project, package, or `dist`
- mutate a target VBA project
- create, update, remove, or import target project modules
- infer missing Template, Manifest, Template Derivation, or GenerateContext
  facts from generated output, Template contents, runtime state, or target
  project state
- repair, normalize, or complete upstream data inside Generator or output-write
  handling

## Future Implementation Preconditions

A later implementation task must explicitly record all of the following before
code or tests are changed:

- exact output-write entry point
- exact source files allowed to change
- exact focused test files and runner registration
- input model accepted by the write boundary
- target project mutation mode and rollback / no-partial-write expectations
- behavior for empty, partial, failed, ambiguous, unsupported, unapproved,
  fallback-derived, or implicitly selected upstream state
- verification commands
- excluded package, `dist`, release, and external-service operations

## Hard Stops

Any future output-write task must hard-stop before writing or mutating when:

- GenerateContext is missing, failed, partial, ambiguous, unsupported, or
  unapproved
- Generator output is missing, failed, partial, or contains unresolved required
  data
- Template selection is fallback-derived, implicit, ambiguous, unsupported, or
  inferred from Template contents
- required target project mutation controls are not explicitly authorized
- the requested change needs package, `dist`, release, external service, public
  API, persisted schema, canonical format, or Frozen specification changes

## Preserved Boundaries

P6-01 preserves the P5-04 through P5-15 boundaries:

- no fallback Template selection
- no implicit Template selection
- no Template content inference
- no GenerateContext or Generator compensation for incomplete upstream data
- no Generator-side repair, normalization, inference, or completion of missing
  Manifest, Template Derivation, or GenerateContext facts
- no Generator invocation from failed, partial, ambiguous, unsupported,
  unapproved, fallback-derived, or implicitly selected upstream state
- no output write, target project mutation, package artifact, `dist` artifact,
  release operation, external service operation, public API change, persisted
  schema change, canonical format change, or Frozen specification change

## GO / NO-GO Boundary

GO:

- documentation-only P6-01 boundary planning
- backlog and current-status updates recording P6-01 completion
- future candidate selection or planning that preserves the P5-04 through P6-01
  boundaries

NO-GO:

- local-only implementation
- production code changes
- test code changes
- Template file changes
- Template Derivation, GenerateContext, or Generator behavior changes
- fallback or implicit Template selection
- Template content inference
- GenerateContext or Generator-side compensation
- generated output writes
- target VBA project mutation
- package, `dist`, release, publication, external service, public API,
  persisted schema, canonical format, or Frozen specification changes

## Next Action Boundary

The next action may be a docs-only candidate selection or a separate explicit
GO / NO-GO decision for a narrow local-only output-write implementation.

Any implementation GO must name the output-write entry point, editable files,
focused tests, runner registration, target project mutation controls,
acceptance criteria, and verification commands. Without that explicit GO,
implementation remains NO-GO.

## Verification Plan

Required verification for this docs-only boundary planning:

- documentation diff review
- `git diff --check`

Build creation checks and VBA test execution are not required for this
docs-only boundary plan because no production code, test code, Template files,
GenerateContext behavior, Generator behavior, generated output, target project
mutation, package, `dist`, or runtime generation behavior are changed.
