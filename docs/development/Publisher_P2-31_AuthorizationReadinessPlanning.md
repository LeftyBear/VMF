# Publisher P2-31 Authorization Readiness Planning

Status: COMPLETE
Decision: GO — authorize the first narrow local-only preview-update implementation slice.
Scope: docs-only / local-only authorization-readiness planning
Implementation: not performed in P2-31

- docs-only authorization/readiness record
- implementation itself is not performed in P2-31
- preview-update remains local-only
- input boundary:
  - local Markdown
  - existing Verified State store/root
  - explicitly supplied local/synthetic current snapshot fixture
- no Google Docs / Google Drive access
- no OAuth or token-store operation
- no Live E2E
- no adapter apply
- no post-apply readback
- no Verified State promotion/save
- no package/dist/release/tag/publication
- no Avast/vendor/flagged executable operation
- no new dependency
- no Frozen specification change
- no public API change
- no persisted schema change
- existing dry-run contract remains unchanged

local Verified State input shape:
SATISFIED
Use the existing Verified State store/schema; do not introduce a new persisted schema.

local/synthetic snapshot input shape:
SATISFIED
Use an explicitly supplied local/synthetic fixture. No Google snapshot acquisition.

orchestration:
SATISFIED
Reuse existing DiffEngine / PhysicalUpdatePlanner boundaries. A narrow internal preview coordinator may be added only if necessary and without public API changes.

stable failure mapping:
SATISFIED FOR IMPLEMENTATION START
Map P2-29 bounded statuses to existing CLI failure classification/exit behavior without changing existing global exit-code contracts.

safe-value boundary:
SATISFIED
Only bounded labels, booleans, stable error codes and non-content counts may be emitted.

Production:
- src/Publisher.Cli/Program.cs
- narrowly scoped Publisher Application preview coordinator, only if required
- existing local planning/composition code only where necessary

Tests:
- tests/unit/Publisher/CliApplicationTests.cs
- focused PhysicalUpdate / Verified State unit tests where required
