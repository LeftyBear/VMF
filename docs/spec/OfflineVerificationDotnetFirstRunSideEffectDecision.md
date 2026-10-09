# Schema-4 ROT-Only — Option C0 .NET SDK First-Run Side-Effect Decision

> **Current applicability:** The certificate-side-effect disposition below
> remains historical. The SDK/environment controls in this record remain
> applicable to any separately authorized future offline verification. The
> earlier "ready for verification" routing outcome is **CONSUMED** and grants
> no current verification or execution authority.

## Status

**ADOPTED**

## Decision

The User selected **Option C0**.

During the prior offline-verification attempt, .NET SDK 8.0.425 first-run
initialization reported installation or generation of an ASP.NET Core HTTPS
development certificate. That already-occurred event is accepted only as a
bounded historical environmental side effect of the failed verification
attempt.

This acceptance does not establish that the certificate is trusted or safe,
does not resolve the security disposition, and does not authorize any general
security-related change.

## Historical Side-Effect Disposition

- Reported ASP.NET Core development-certificate generation:
  `ACCEPTED AS HISTORICAL SIDE EFFECT`.
- Automatic or manual certificate removal: `NOT AUTHORIZED`.
- Certificate trust or untrust modification: `NOT AUTHORIZED`.
- Certificate-store inspection or manipulation: `NOT AUTHORIZED`.
- Avast or other security-configuration changes: `NOT AUTHORIZED`.
- Security bypass: `NOT AUTHORIZED`.
- Security disposition: `UNRESOLVED`.

The previously reported certificate must not be removed, trusted, untrusted,
or otherwise changed under this decision.

## Required Controls for the Next Authorized Verification

Before the first `dotnet` CLI invocation of the next separately authorized
verification, the verification process environment must:

1. use a task-specific temporary `DOTNET_CLI_HOME`; and
2. set `DOTNET_GENERATE_ASPNET_CERTIFICATE=false`.

Both controls must be established before invoking `dotnet`. Their limited
purpose is to prevent recurrence of ASP.NET development-certificate generation
and isolate SDK first-run writable state from the normal user CLI home.

## Durable Offline Verification Configuration

For any separately authorized future offline verification:

- SDK `8.0.425` must be selected through task-local or external configuration
  unless separately adopted as repository-level project configuration.
- Runtime selection must use normal latest-patch roll-forward. Historical
  run-local `rollForward = disable` and `DOTNET_ROLL_FORWARD=Disable` are not
  part of the current accepted verification procedure.
- External NuGet package sources must be cleared, no network acquisition may
  occur, and restore may use only already-available local package content.
- Package and cache locations, when needed, must be task-specific and
  transient. Historical run-relative `packages`, `cache`, and `http-cache`
  paths are not durable repository paths.
- Before the first `dotnet` invocation, SDK `8.0.425` must be selected, a
  task-specific `DOTNET_CLI_HOME` must be established, and
  `DOTNET_GENERATE_ASPNET_CERTIFICATE=false` must be set.

No historical run-local verification config file is authoritative. The three
run-local `global.json` files are **DELETE CANDIDATE** because their durable SDK
selection semantics are recorded here and their `rollForward = disable`
setting is not current procedure. The three run-local `NuGet.Config` files are
**DELETE CANDIDATE AFTER CONSOLIDATION** because their durable cleared-source,
offline/no-network restore semantics are recorded here while their relative
package/cache paths are transient or stale. This classification does not
authorize deletion of any file.

This record does not authorize that verification or any `dotnet` invocation.
It does not authorize certificate removal, certificate trust changes,
certificate-store manipulation, or unrelated SDK initialization changes.

## Relationship to Option B0

Option B0 remains applicable only according to its existing next-run scope in
`docs/spec/OfflineVerificationBootstrapPowerShellExceptionDecision.md`.

Bootstrap-only read-only PowerShell may be used solely within that exact B0
scope. After bootstrap, PowerShell remains prohibited for substantive
verification. `build.ps1` remains prohibited.

Option C0 does not extend, renew, consume, or otherwise alter Option B0 and
does not itself authorize the next verification run.

## Preserved Governance Boundaries

- Security disposition remains `UNRESOLVED`.
- The real Experiment remains `NO-GO / SAFE-STOP`.
- No candidate authorization is introduced.
- No verification, restore, build, test, Excel/COM, Experiment, `Build.Host`,
  `build.ps1`, network, security-change, or U03-access authorization is
  introduced.
- No certificate removal or certificate-store modification is authorized.
- No broader security-change authorization is introduced.
- Frozen specifications, public contracts, persisted schemas, canonical
  formats, error-code contracts, and architectural boundaries remain
  unchanged.
- No Git mutation is authorized.

## Governance Outcome

Option C0 is historical-side-effect acceptance only. Subject to a separate,
fresh authorization for the verification itself, the next full offline
verification must apply both required process-environment controls before its
first `dotnet` invocation.

```text
A. OPTION C0 RECORDED — READY FOR FRESH FULL OFFLINE VERIFICATION
```

“Ready” identifies the next possible separately authorized governance step. It
does not authorize verification, technical execution, a candidate, or the real
Experiment.
