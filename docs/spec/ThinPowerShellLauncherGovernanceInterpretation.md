# Thin PowerShell Launcher Governance Interpretation

> **Current applicability:** The decision-time implementation gate below is
> historical context. PG-01 and Excel Ownership are now independently accepted
> working-tree slices. That acceptance grants no execution, release, security,
> or Git authorization and does not alter the normative thin-PowerShell
> interpretation in this record.

## 1. Record status

- Route: `C`
- Work mode: `docs-only`
- Governance Interpretation: `ADOPTED`
- Frozen Change: `NONE`
- Technical Execution Authorization: `NOT GRANTED`
- Technical execution: `NO-GO / SAFE-STOP`
- Avast detection: `UNRESOLVED`
- PG-01 implementation: `COMPLETE within its previously authorized scope`
- PG-01: `NOT ESTABLISHED`

This record documents the accepted governance interpretation of existing
Frozen PowerShell build wording. It is an interpretation only. It does not
amend, weaken, replace, or reopen a Frozen specification.

## 2. Context and decision basis

The Build Architecture Review established that PowerShell is not inherently
required by `Build.xlam` artifact semantics.

The Governance Interpretation Review result is:

`B - COMPATIBLE SUBJECT TO GOVERNANCE INTERPRETATION`

The interpretation is therefore recorded to distinguish an approved,
PowerShell-controlled build transaction from the mere launching of another
process. This record concerns build provenance, orchestration, and evidence;
it does not select or authorize an implementation architecture.

## 3. Authoritative Frozen references

This interpretation cross-references, without modifying, the following
Frozen requirements:

- `specs/build/BuildBlueprint_v1.0.1.md`, lines 109-115: the release audit
  includes PowerShell Build Artifact Verification; the latest `Build.xlam`
  produced after the approved PowerShell build is the audited release
  artifact; failed audit steps require correction and re-audit.
- `specs/build/BuildQualityStandard_v1.0.md`, sections 4 and 6: evidence must
  trace the release decision to inspected artifacts, and the latest
  PowerShell-built `Build.xlam` must be identified by path and timestamp
  before release audit treatment.
- `specs/build/BuildReleaseProcedure_v1.0.md`, Step 12: the PowerShell build
  command or approved build reference, build log, artifact path, timestamp,
  source inputs, output directory, and release-audit artifact identity are
  inspection and evidence requirements.
- `specs/build/BuildDocumentationStandard_v1.0.md`, lines 64-72 and 165-175:
  the quality standard and release procedure retain their distinct
  responsibilities, and the release procedure requires the latest
  `Build.xlam` produced after the approved PowerShell build to be audited.

These references remain authoritative as written. This record neither
restates them as a replacement specification nor changes their requirements.

## 4. Adopted interpretation

"Approved PowerShell build" and "PowerShell-built Build.xlam" identify an
artifact produced through the approved, PowerShell-controlled build
transaction.

Substantive build operations may execute in an approved local delegated host,
provided that PowerShell:

1. remains the official and required external build entry point;
2. invokes only the approved delegated host and configuration;
3. controls the invocation synchronously;
4. propagates delegated-host failure as build failure;
5. does not mask partial or abnormal failure;
6. verifies that the expected artifact belongs to that build invocation;
7. preserves artifact path, timestamp, source-input, output-directory,
   command or reference, delegated-host result, log, and final-result
   evidence;
8. exposes one authoritative final success or failure result; and
9. ensures subsequent release verification uses that exact artifact.

Launching another process by itself is insufficient to establish a
"PowerShell-built" artifact.

## 5. Minimum conformity conditions

Any future separately authorized design or implementation relying on this
interpretation must preserve all of these conditions:

- PowerShell remains the official external entry point.
- The delegated host and configuration are approved and deterministic.
- Inputs, source set, output, and release metadata remain governed by the
  approved path.
- Delegation is synchronous and bounded.
- Host failure or invalid, missing, or ambiguous output fails the build.
- The artifact is correlated to the exact invocation.
- A complete evidence chain is preserved.
- Existing artifact semantics and public behavior are not weakened.
- Subsequent release audit uses the exact resulting artifact.

These are conformity conditions for the interpretation, not a new technical
specification or an execution authorization.

## 6. Boundaries and explicit non-effects

This record does not:

- modify Frozen wording;
- authorize implementation or select an implementation architecture;
- authorize a .NET or other delegated host;
- authorize launcher or `build.ps1` modification;
- authorize build, test, prototype, mock, dry-run, Excel, or COM execution;
- establish that a thin launcher resolves the Avast detection;
- declare `build.ps1` safe or unsafe;
- establish PG-01;
- accept PG-01 evidence;
- modify the existing PG-01 working-tree changes;
- authorize an Avast exception, bypass, or other security-control change;
- grant evidence acceptance, security disposition, continuation authority, or
  technical GO;
- authorize package, `dist`, release, publication, or external operations; or
- authorize Git staging, commit, push, or any other Git mutation.

No full release-workflow replacement is required or authorized by this
interpretation. No public VBA/API, persisted schema, canonical format,
error-code contract, or architectural boundary is changed.

## 7. Current authorization and execution state

- Governance interpretation record: `ADOPTED`
- Frozen specification change required: `NO`
- Thin-launcher implementation: `NOT AUTHORIZED`
- Technical execution: `NO-GO / SAFE-STOP`
- Avast detection: `UNRESOLVED`
- PG-01: `NOT ESTABLISHED`
- PG-01 evidence acceptance: `NOT GRANTED`
- Existing PG-01 working-tree changes: `PRESERVED / UNTOUCHED`
- Git mutation: `NOT AUTHORIZED`

Any architecture or design decision, implementation, execution, security
disposition, evidence acceptance, or Git operation requires a separate,
scope-specific authorization. This adopted documentary interpretation does
not clear any existing `NO-GO / SAFE-STOP` boundary.

## 8. Static review and non-actions

Static review confirms that the interpretation matches the accepted Review
result, retains the Frozen requirements as authoritative, states all minimum
conformity conditions, and grants no technical authorization.

In creating this record:

- exactly one new documentation file was created;
- no existing repository file was modified;
- no repository script was executed;
- no build, test, prototype, mock, or dry-run was executed;
- no Excel or COM operation was executed;
- no Avast setting or security control was changed;
- no external operation was performed;
- no Git mutation or authenticated Git operation was performed; and
- the prohibited U03 file was not accessed.

## 9. Next independent gate

The next possible step is a separate Thin PowerShell Launcher architecture or
design authorization decision. This record does not start or authorize that
step.
