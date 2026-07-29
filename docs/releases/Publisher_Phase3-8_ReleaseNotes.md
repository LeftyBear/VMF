# Publisher Phase 3-8 Release Notes

Date: 2026-07-29

## Scope

Phase 3-8 completes Publisher productization operations without changing Frozen
Architecture, Google API contracts, identity/revision contracts, or domain API
contracts.

## Operational Changes

- Added per-run Publish Session ID structured JSON diagnostics.
- Added publish summary fields for exit code, classification, elapsed time, and
  published document identity when available.
- Added CLI commands: `publish`, `verify`, `diff`, and `dry-run`.
- Added explicit CLI exit-code mapping.
- Added appsettings validation and documented environment-variable precedence.
- Added CLI operation and HTTP timeout settings.
- Documented retry expectations for transient Google HTTP failures and
  non-retry behavior for authentication or permission failures.

## Configuration

`Cli:OperationTimeoutSeconds` and `Cli:HttpTimeoutSeconds` are available in
`appsettings.json`, `appsettings.local.json`, and environment-variable overrides:

- `VMF_PUBLISHER_OPERATION_TIMEOUT_SECONDS`
- `VMF_PUBLISHER_HTTP_TIMEOUT_SECONDS`

Settings priority is `appsettings.json`, then `appsettings.local.json`, then
environment variables.

## Compatibility

The legacy `Google` section remains accepted for service-account compatibility.
The current `GoogleApi` section remains preferred. No Frozen specification,
Google API request contract, identity contract, revision contract, or persisted
state schema is changed by this phase.

## Verification Checklist

- Release build: `dotnet build VMF.Publisher.sln --configuration Release --no-restore`
- Unit tests: `dotnet test tests\unit\Publisher\Vmf.Publisher.UnitTests.csproj --configuration Release --no-restore`
- Integration tests: `dotnet test tests\integration\Publisher\Vmf.Publisher.IntegrationTests.csproj --configuration Release --no-restore`
- Live E2E: run Google Docs end-to-end integration with explicit live-test
  authorization and configured OAuth credentials.
- Format/static: `dotnet format VMF.Publisher.sln --verify-no-changes --no-restore`
  and `git diff --check`
