# VMF-SOURCE-MANIFEST-V1 Current 32-File Authority Decision

> **Applicability after Schema-4 closeout:** This document remains the current authority for the 32-file manifest membership, exclusions, canonical serialization, sizes, hashes, and aggregate identity. Only its pre-closeout Schema-4 Experiment status wording is historical and superseded by the successful closeout recorded in `docs/development/CURRENT_STATUS.md`, `docs/development/HANDOFF.md`, and `docs/VMF_vNext_Backlog.md`. This note does not alter the manifest contract.

## Status

**ADOPTED**

The User explicitly adopted this governance decision.

## Purpose

This decision supplies the previously missing exact membership and serialization authority for the current 32-file VMF-SOURCE-MANIFEST-V1 identity.

The historical 30-file / 3910-byte manifest remains valid historical evidence, but it is not the current operational identity.

## Exact Current Membership

The current VMF-SOURCE-MANIFEST-V1 membership consists of exactly these 32 files:

1. `src/Build.ExcelOwnership.Experiment/ExperimentContracts.cs`
2. `src/Build.ExcelOwnership.Experiment/ExperimentController.cs`
3. `src/Build.ExcelOwnership.Experiment/ExperimentEvidence.cs`
4. `src/Build.ExcelOwnership.Experiment/ExperimentWorker.cs`
5. `src/Build.ExcelOwnership.Experiment/Program.cs`
6. `src/Build.ExcelOwnership.Experiment/Vmf.Build.ExcelOwnership.Experiment.csproj`
7. `src/Build.ExcelOwnership.Primitives/ActivationProcessIdentity.cs`
8. `src/Build.ExcelOwnership.Primitives/ApplicationIdentityValidation.cs`
9. `src/Build.ExcelOwnership.Primitives/ApplicationProcessIdentity.cs`
10. `src/Build.ExcelOwnership.Primitives/CleanupTopologyValidation.cs`
11. `src/Build.ExcelOwnership.Primitives/EvidencePersistenceConfirmation.cs`
12. `src/Build.ExcelOwnership.Primitives/ExcelOwnershipLease.cs`
13. `src/Build.ExcelOwnership.Primitives/ExcelOwnershipSession.cs`
14. `src/Build.ExcelOwnership.Primitives/LifecycleJournal.cs`
15. `src/Build.ExcelOwnership.Primitives/Ownership.cs`
16. `src/Build.ExcelOwnership.Primitives/ProcessIdentity.cs`
17. `src/Build.ExcelOwnership.Primitives/ProcessIdentityObservation.cs`
18. `src/Build.ExcelOwnership.Primitives/Properties/AssemblyInfo.cs`
19. `src/Build.ExcelOwnership.Primitives/ResidualProcessObserver.cs`
20. `src/Build.ExcelOwnership.Primitives/ResidualState.cs`
21. `src/Build.ExcelOwnership.Primitives/RotSentinelAttachment.cs`
22. `src/Build.ExcelOwnership.Primitives/Vmf.Build.ExcelOwnership.Primitives.csproj`
23. `src/Build.Host/Program.cs`
24. `src/Build.Host/Vmf.Build.Host.csproj`
25. `tests/unit/Build.ExcelOwnership.Experiment/Program.cs`
26. `tests/unit/Build.ExcelOwnership.Experiment/StructuralIsolationTests.cs`
27. `tests/unit/Build.ExcelOwnership.Experiment/Vmf.Build.ExcelOwnership.Experiment.UnitTests.csproj`
28. `tests/unit/Build.ExcelOwnership.Primitives/ExcelProcessBaselineObserver.cs`
29. `tests/unit/Build.ExcelOwnership.Primitives/Program.cs`
30. `tests/unit/Build.ExcelOwnership.Primitives/Vmf.Build.ExcelOwnership.Primitives.UnitTests.csproj`
31. `tests/unit/Build.Host/Program.cs`
32. `tests/unit/Build.Host/Vmf.Build.Host.UnitTests.csproj`

No other file is a member of this identity.

## Explicit Exclusions

The following are explicitly outside this 32-file manifest:

- `tools/build/build.ps1`
- governance and documentation files
- generated `bin/` and `obj/` content
- task-local SDK or NuGet state
- candidate artifacts
- U03

`tools/build/build.ps1` is deliberately outside this 32-file manifest. Its governance and production role is separate, and it is not an input to the Schema-4 candidate execution-artifact source identity.

## Canonical Serialization

For each of the exact 32 files, VMF-SOURCE-MANIFEST-V1 serializes one line in this form:

```text
<repository-relative-path><TAB><byte-length><TAB><lowercase-sha256><LF>
```

The canonical serialization rules are:

- encode the manifest as UTF-8 without BOM;
- use repository-relative paths;
- use `/` path separators;
- order paths ordinally;
- encode byte length in decimal with no padding;
- encode SHA-256 as exactly 64 lowercase hexadecimal characters;
- use exactly two TAB bytes per line;
- terminate every line with exactly one LF byte;
- include no CR bytes;
- end the manifest with exactly one final LF byte;
- include no header;
- include no footer;
- include no blank line; and
- include no incidental command, pipeline, or index output.

## Canonical Aggregate Identity

The expected canonical result is all three of:

- file and line count: **32**
- canonical manifest byte length: **4188**
- SHA-256: **`bbb5b0ca72d6411165e660246aeb8dae9fe3232741bfd1dae5d4350dcf489e35`**

Before any candidate may be pinned, the manifest MUST be freshly reproduced from the exact 32 files and MUST match all three values above.

Any mismatch requires **SAFE-STOP**. During candidate preparation, the expected digest MUST NOT be corrected to match newly produced bytes.

Any authorized modification to any of the 32 files invalidates this identity and requires a new manifest decision.

## Authorization Boundary

This decision does not:

- create a candidate;
- authorize `dotnet publish`;
- authorize Experiment execution;
- resolve security;
- authorize `tools/build/build.ps1`;
- authorize Excel or COM execution; or
- authorize Git mutation.

Security remains unresolved. Real Experiment execution remains **NO-GO / SAFE-STOP**.
