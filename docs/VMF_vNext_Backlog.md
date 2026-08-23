# VMF vNext Backlog

Status  : Active Build vNext record
Scope   : VMF Build vNext planning, implementation closeout, and verification state

This backlog records VMF Build vNext planning, implementation closeout, and
verification status. It does not authorize work beyond each explicitly recorded
GO scope, and it does not authorize package or `dist` updates, external service
operations, release operations, publication, push, tag creation, or Frozen
specification changes.

## Current Items

| Item | Scope | Status | Record | Notes |
| --- | --- | --- | --- | --- |
| P2-01 | Blueprint Specification v0.1 scope definition | COMPLETE / docs-only scope definition | `docs/design/P2-01_BlueprintSpecificationV0_1ScopeDefinition.md` | Defines Blueprint v0.1 scope and the Blueprint / Manifest responsibility boundary. |
| P2-02 | Blueprint Specification v0.1 field model definition | COMPLETE / docs-only schema design | `docs/design/P2-02_BlueprintSpecificationV0_1FieldModelDefinition.md` | Defines the Blueprint v0.1 field model and preserves docs-only boundaries. |
| P2-03 | Blueprint Specification v0.1 example documents | COMPLETE / docs-only examples | `docs/design/P2-03_BlueprintSpecificationV0_1ExampleDocuments.md` | Provides valid and invalid Blueprint examples while preserving docs-only boundaries. |
| P2-04 | Blueprint v0.1 validation rule definition | COMPLETE / docs-only validation specification | `docs/design/P2-04_BlueprintV0_1ValidationRuleDefinition.md` | Defines validation rules and Manifest derivation eligibility while preserving docs-only boundaries. |
| P2-05 | Blueprint v0.1 error classification definition | COMPLETE / docs-only error classification specification | `docs/design/P2-05_BlueprintV0_1ErrorClassificationDefinition.md` | Defines validation error categories and future error-code readiness while preserving docs-only boundaries. |
| P2-06 | Blueprint v0.1 validation error code definition | COMPLETE / docs-only error code specification | `docs/design/P2-06_BlueprintV0_1ValidationErrorCodeDefinition.md` | Defines validation error codes and severity mapping while preserving docs-only boundaries. |
| P2-07 | Blueprint v0.1 validator implementation scope planning | COMPLETE / docs-only implementation scope planning | `docs/design/P2-07_BlueprintV0_1ValidatorImplementationScopePlanning.md` | Defines future Validator implementation scope and GO / NO-GO boundaries while preserving docs-only boundaries. |
| P2-08 | Blueprint validator minimal implementation candidate selection | COMPLETE / docs-only candidate selection | `docs/design/P2-08_BlueprintValidatorMinimalImplementationCandidateSelection.md` | Selects Candidate B -- Minimal Generatable Validation for a future implementation while preserving docs-only boundaries. |
| P2-09 | Blueprint validator Candidate B implementation scope definition | COMPLETE / docs-only implementation scope definition | `docs/design/P2-09_BlueprintValidatorCandidateBImplementationScopeDefinition.md` | Fixes Candidate B implementation boundaries for a later task while preserving docs-only boundaries. |
| P2-10 | Blueprint validator entry point and model design | COMPLETE / docs-only implementation design | `docs/design/P2-10_BlueprintValidatorEntryPointAndModelDesign.md` | Defines future Validator entry point and model design while preserving docs-only boundaries. |
| P2-11 | Blueprint validator Candidate B minimal implementation | COMPLETE / implementation verified | `db252d1` | Implements Candidate B -- Minimal Generatable Validation with focused Validator tests and preserved parser / generator boundaries. |
| P2-12 | Blueprint validator Candidate B verification and closeout | COMPLETE / verification and closeout | `docs/design/P2-12_BlueprintValidatorCandidateBVerificationAndCloseout.md` | Records P2-11 verification PASS, generated artifact cleanup, boundary confirmation, and P2 COMPLETE. |
| P3-07 | Validator integration completion review | COMPLETE / completion review | `docs/spec/ValidatorIntegrationCompletionReview.md` | Records Validator integration behavior PASS, Build PASS warnings 0 / errors 0, existing Build regression 18 runners PASS, focused integration and Validator test PASS, `git diff --check` PASS, generated artifact cleanup, no code-level blocker, and P3-07 COMPLETE. |
| P3-08 | Next candidate selection | COMPLETE / docs-only selection | `docs/spec/P3NextCandidateSelection.md` | Selects P4-01 -- Manifest Derivation Scope Planning as the next docs-only candidate and records NO-GO for direct Manifest derivation implementation in P3-08. |
| P4-01 | Manifest derivation scope planning | COMPLETE / docs-only planning | `docs/spec/P4-01_ManifestDerivationScopePlanning.md` | Fixes the Validated Blueprint -> Manifest derivation responsibility boundary, transformation rules, hard-stop conditions, existing-flow relationship, and minimum future implementation slice without implementation GO. |
| P4-02 | Manifest derivation minimum local implementation slice | COMPLETE / IMPLEMENTED / VERIFIED | `docs/spec/P4-02_ManifestDerivationImplementationRecord.md` | Adds `BlueprintManifestDeriver`, derives Manifest content deterministically from Validator-passed Validated Blueprint input, preserves the compatibility parser entry point by delegating formatting to the deriver, hard-stops incomplete / ambiguous / unsupported / unapproved / non-generatable input, does not infer missing `LayerName`, and preserves Parser / Validator / Template / GenerateContext / Generator boundaries. |
| P4-03 | Manifest derivation focused test completion | COMPLETE / TESTED / VERIFIED | `docs/spec/P4-03_ManifestDerivationFocusedTestCompletion.md` | Extends focused Manifest Derivation tests for Validator PASS input, validation-error hard-stop, Parser / Validator non-conversion, and Template / GenerateContext / Generator pre-boundary while preserving local-only and no-`dist` boundaries. |
| P4-04 | Template Mapping scope planning | COMPLETE / docs-only planning | `docs/spec/P4-04_TemplateMappingScopePlanning.md` | Fixes the post-Manifest-Derivation Template Mapping boundary, input / output rules, hard-stop conditions before GenerateContext and Generator, and minimum future implementation slice without implementation GO. |
| P4-05 | Template Mapping contract freeze | COMPLETE / docs-only contract freeze | `docs/spec/P4-05_TemplateMappingContractFreeze.md` | Freezes Template Mapping as the deterministic Manifest -> Template binding contract before GenerateContext, records what Template Mapping decides and does not decide, and defines hard-stops for unresolved, ambiguous, unsupported, or unapproved mapping state without implementation GO. |
| P4-06 | GenerateContext responsibility boundary freeze | COMPLETE / docs-only responsibility boundary freeze | `docs/spec/P4-06_GenerateContextResponsibilityBoundaryFreeze.md` | Freezes GenerateContext as the deterministic Template Mapping output + Manifest-derived data -> Generator-ready context boundary, records what GenerateContext owns and does not own, and defines hard-stops for unresolved, inconsistent, unsupported, or incomplete context state before Generator without implementation GO. |
| P4-07 | Generator Input Contract scope planning | COMPLETE / docs-only scope planning | `docs/spec/P4-07_GeneratorInputContractScopePlanning.md` | Fixes Generator input to a complete and successful GenerateContext result only, records required input items, responsibility and failure boundaries, and preserves upstream Parser / Validator / Manifest Derivation / Template Mapping / GenerateContext hard stops without implementation GO. |
| P4-08 | Generator focused test design | COMPLETE / docs-only test design | `docs/spec/P4-08_GeneratorFocusedTestDesign.md` | Fixes the future focused local test design for the Generator boundary, identifies docs-only target files and future candidate test areas, defines acceptance criteria and prohibited operations, and preserves Parser / Validator / Manifest Derivation / Template Mapping / GenerateContext / Generator separation without implementation GO. |
| P4-09 | Generator Focused Test Implementation Scope Planning | COMPLETE / docs-only implementation scope planning | `docs/spec/P4-09_GeneratorFocusedTestImplementationScopePlanning.md` | Fixes the future implementation decision boundary for P4-08 focused Generator tests, records candidate implementation and non-scope areas, acceptance criteria, and safety stops, and keeps production code, test code, Generator behavior, and runtime behavior changes as NO-GO. |
| P5-01 | Template Derivation Scope Planning | COMPLETE / docs-only planning | `docs/spec/P5-01_TemplateDerivationScopePlanning.md` | Fixes the Manifest-only Template Derivation boundary, output and GenerateContext handoff, failure boundaries for missing information / unsupported elements / non-unique conversion / unsatisfied Generator preconditions, existing implementation relationship, and minimum future implementation slice without implementation GO. |
| P5-02 | Template Inventory Review / Concrete Template Derivation Table | COMPLETE / docs-only inventory and derivation table | `docs/spec/P5-02_TemplateInventoryAndDerivationTable.md` | Reviews the existing Template inventory, records the concrete Manifest fact to Template binding table, classifies deterministic, unsupported, ambiguous, and deferred mapping cases, confirms P5-01 information-source and hard-stop boundaries, and keeps implementation, tests, Template, GenerateContext, Generator, package, `dist`, release, external service, staging, commit, and push operations as NO-GO. |
| P5-03 | Template Derivation Output Model Planning | COMPLETE / docs-only output model planning | `docs/spec/P5-03_TemplateDerivationOutputModelPlanning.md` | Defines the Template Derivation Model output fields from Validator PASS / approved Manifest input, normalizes approved P5-02 Template selection results for downstream GenerateContext, records unsupported / non-generatable hard stops, and keeps implementation, tests, Template generation, GenerateContext, Generator, package, `dist`, release, external service, and Frozen specification changes as NO-GO. |
| P5-04 | Template Derivation Failure Boundary Planning | COMPLETE / docs-only failure boundary planning | `docs/spec/P5-04_TemplateDerivationFailureBoundaryPlanning.md` | Fixes the Template Derivation failure boundary: only complete, approved, generatable P5-03 output model items from approved P5-02 rules may proceed toward GenerateContext; unsupported, non-generatable, ambiguous, incomplete, unapproved, fallback, or implicit Template selections must stop before GenerateContext and Generator without implementation GO. |
| P5-05 | Template Derivation Focused Test Design | COMPLETE / docs-only focused test design | `docs/spec/P5-05_TemplateDerivationFocusedTestDesign.md` | Fixes the future focused local test design for supported and blocking Template Derivation states while preserving the P5-04 failure boundary; fallback, implicit Template selection, Template content inference, and GenerateContext / Generator compensation remain prohibited without implementation GO. |
| P5-06 | GenerateContext Data Model Planning | COMPLETE / docs-only data model planning | `docs/spec/P5-06_GenerateContextDataModelPlanning.md` | Defines the future GenerateContext data model boundary from complete, approved, generatable Template Derivation output, records required and deferred data groups, preserves P5-04 / P5-05 hard stops before Generator, and keeps local-only implementation, fallback, implicit Template selection, Template content inference, GenerateContext / Generator compensation, package, `dist`, release, external service, and Frozen specification changes as NO-GO. |
| P5-07 | GenerateContext Focused Test Design | COMPLETE / docs-only focused test design | `docs/spec/P5-07_GenerateContextFocusedTestDesign.md` | Fixes the future focused local test design for successful GenerateContext construction and GenerateContext hard-stop classifications while preserving the P5-04 through P5-06 boundary; fallback, implicit Template selection, Template content inference, GenerateContext / Generator compensation, local-only implementation, code, test, package, `dist`, release, external service, and Frozen specification changes remain NO-GO. |

## Boundary

- P1 is complete.
- P2-01 is complete and committed.
- P2-02 is complete as docs-only schema design.
- P2-03 is complete as docs-only examples.
- P2-04 is complete as docs-only validation specification.
- P2-05 is complete as docs-only error classification specification.
- P2-06 is complete as docs-only error code specification.
- P2-07 is complete as docs-only implementation scope planning.
- P2-08 is complete as docs-only candidate selection; selected candidate is
  Candidate B -- Minimal Generatable Validation.
- P2-09 is complete as docs-only implementation scope definition for Candidate
  B -- Minimal Generatable Validation.
- P2-10 is complete as docs-only implementation design.
- P2-11 is complete as Candidate B -- Minimal Generatable Validation
  implementation in commit `db252d1`; verification passed before commit.
- P2-12 is complete as verification and closeout.
- P3-07 is complete as Validator integration completion review.
- P3-08 is complete as next candidate selection; selected next candidate is
  P4-01 -- Manifest Derivation Scope Planning.
- P4-01 is complete as docs-only Manifest derivation scope planning. It fixes
  the Validated Blueprint -> Manifest boundary and does not authorize
  implementation.
- P4-02 is complete as the Manifest Derivation minimum local implementation
  slice. It implements and verifies `BlueprintManifestDeriver`, keeps
  `Build_BlueprintParser.BuildGenerateManifestContent` as a compatibility entry
  point, delegates Manifest formatting to the deriver, hard-stops before
  Generator input on derivation failure, and maintains the P4-01 boundary.
- P4-03 is complete as focused test completion for Manifest Derivation. It
  extends `AppBlueprintManifestTests`, keeps Parser and Validator out of
  Manifest conversion, and verifies local-only using a temporary Build.xlam
  without updating package or `dist` artifacts.
- P4-04 is complete as docs-only Template Mapping scope planning. It fixes the
  post-Manifest-Derivation Template Mapping boundary, keeps Parser, Validator,
  and Manifest Derivation responsibilities unchanged, and records hard-stop
  conditions before GenerateContext and Generator without authorizing
  implementation.
- P4-05 is complete as docs-only Template Mapping contract freeze. It fixes
  Template Mapping as the deterministic Manifest -> Template binding boundary
  before GenerateContext, records the decisions Template Mapping owns and does
  not own, and requires unresolved, ambiguous, unsupported, or unapproved
  mapping state to hard-stop before GenerateContext and Generator without
  authorizing implementation.
- P4-06 is complete as docs-only GenerateContext responsibility boundary
  freeze. It fixes GenerateContext as the deterministic boundary that consumes
  only successful Template Mapping output and approved Manifest-derived data,
  packages that data into Generator-ready context, and requires unresolved,
  inconsistent, unsupported, or incomplete context state to hard-stop before
  Generator without authorizing implementation.
- P4-07 is complete as docs-only Generator Input Contract scope planning. It
  fixes Generator input to a complete and successful GenerateContext result
  only, records the required input items, Generator responsibility boundary,
  failure boundary, and upstream connection, and keeps implementation,
  Generator code changes, and runtime behavior changes as NO-GO.
- P4-08 is complete as docs-only Generator focused test design. It fixes the
  future focused local test design for the Generator boundary, identifies the
  current docs-only target files and future candidate test areas, records
  acceptance criteria and execution prohibitions, and keeps implementation,
  test code changes, Generator code changes, and runtime behavior changes as
  NO-GO.
- P4-09 is complete as docs-only Generator focused test implementation scope
  planning. It connects the P4-08 focused test design to a future
  implementation decision by recording candidate implementation targets,
  candidate non-scope, future acceptance criteria, and safety-stop conditions,
  while keeping production code, test code, Generator behavior, runtime
  behavior, package, `dist`, release, external service, staging, commit, and
  push operations as NO-GO.
- P5-01 is complete as docs-only Template Derivation scope planning. It fixes
  the Manifest-only input boundary, Template binding output boundary,
  GenerateContext handoff, and hard-stop conditions for missing required
  information, unsupported elements, non-unique conversion, and unsatisfied
  Generator preconditions, while keeping implementation, tests, Template file
  changes, GenerateContext changes, Generator changes, package, `dist`,
  release, external service, staging, commit, and push operations as NO-GO.
- P5-02 is complete as docs-only Template inventory review and concrete
  derivation table. It records the current Template inventory, fixes the
  current deterministic mapping set as `StandardModule -> ModuleTemplate.txt`,
  `ClassModule + Domain -> DomainClassTemplate.txt`, and
  `ClassModule + non-Domain supported layer -> ClassTemplate.txt`, classifies
  `DomainModuleTemplate.txt` and procedure / parameter / return value /
  dependency / generation-policy binding from the current Manifest surface as
  unsupported or deferred, confirms TemplatePath consistency hard stops, and
  preserves the P5-01 Manifest-only input and GenerateContext / Generator
  downstream boundaries without authorizing implementation.
- P5-03 is complete as docs-only Template Derivation output model planning. It
  defines the Template Derivation Model fields `templateKey`, `templatePath`,
  `templateRole`, `selectionRuleId`, `derivationReason`, `isGeneratable`, and
  `unsupportedReason`; limits input to Validator PASS / approved Manifest
  state; normalizes only the approved P5-02 Template selection results; and
  requires unapproved Manifest state, non-unique Template selection, missing
  Template inventory, Manifest-only Template misuse, unsupported state, or
  non-generatable state to stop before GenerateContext and Generator without
  authorizing implementation.
- P5-04 is complete as docs-only Template Derivation failure boundary
  planning. It fixes that only complete, approved, generatable P5-03 Template
  Derivation Model items selected by exactly one approved P5-02 rule may
  proceed toward GenerateContext planning. Unsupported, non-generatable,
  ambiguous, incomplete, unapproved, fallback-derived, or implicitly selected
  Template candidates must stop before GenerateContext and Generator, and
  downstream repair, inference, normalization, or compensation remains
  prohibited without authorizing implementation.
- P5-05 is complete as docs-only Template Derivation focused test design. It
  fixes future focused local test cases for approved supported Template
  selections and blocking Template Derivation classifications, preserves the
  P5-04 failure boundary before GenerateContext and Generator, and keeps
  fallback, implicit Template selection, Template content inference, downstream
  compensation, implementation, test code changes, runtime behavior changes,
  package, `dist`, release, external service, and Frozen specification changes
  as NO-GO.
- P5-06 is complete as docs-only GenerateContext data model planning. It
  defines the future GenerateContext data model boundary from complete,
  approved, generatable Template Derivation output, records required and
  deferred data groups, preserves P5-04 / P5-05 hard stops before Generator,
  and keeps fallback, implicit Template selection, Template content inference,
  downstream compensation, implementation, test code changes, runtime behavior
  changes, package, `dist`, release, external service, and Frozen
  specification changes as NO-GO.
- P5-07 is complete as docs-only GenerateContext focused test design. It fixes
  future focused local test cases for successful GenerateContext construction
  and GenerateContext hard-stop classifications from P5-06, preserves the
  P5-04 through P5-06 boundary before Generator, and keeps fallback, implicit
  Template selection, Template content inference, downstream compensation,
  implementation, test code changes, runtime behavior changes, package,
  `dist`, release, external service, and Frozen specification changes as
  NO-GO.
- P2 is COMPLETE.
- No package / `dist`, external service, release, publication, push, tag, or
  Frozen specification change is authorized by this record.
