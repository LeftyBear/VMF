# Publisher Avast Latest-Definition Rescan Release Gate Summary

Status  : Release blocked / owner decision not recorded
Scope   : Docs-only / local-only one-page summary for responsible-owner release authorization or owner risk acceptance review
Date    : 2026-08-12
HEAD    : `8a28e4c`

This summary records the release-gate state after the Avast latest-definition
local static rescan evidence for the existing local Publisher `dist` ZIP. It
is a decision input only. It does not grant release authorization, record owner
risk acceptance, claim Avast vendor clearance, claim Avast safety
certification, create or update packages, update `dist`, create tags, publish
artifacts, execute Live E2E, mutate Google Docs or Google Drive, or re-run a
flagged executable.

## Decision Summary

| Item | State |
| --- | --- |
| Current HEAD | `8a28e4c` |
| Scan target | Existing local `dist` ZIP |
| Artifact path | `dist/release/Publisher/vmf-publisher-0.0.1-dev-win-x64.zip` |
| Artifact size | 983422 bytes |
| SHA-256 | `0174810D21C6072B8206ACF2FED90B72C2E6BE499C65B231D7D72D71FD69CB76` |
| Avast rescan result | Detection not reproduced |
| Previous detection name | `IDP.HELU.PSD11` |
| Vendor clearance | Not obtained |
| Avast safety certification | Not claimed |
| Responsible-owner approval / owner risk acceptance | Not recorded for this latest-definition rescan evidence |
| Release authorization | Not granted by this evidence or summary |
| Release blocked | Continued for any future release-path operation until a separate responsible-owner decision and release authorization are recorded |
| Update command | Timeout / user canceled; not recorded as success |

## Evidence References

| Evidence | Reference |
| --- | --- |
| Local ZIP SHA-256 record | `docs/evidence/publisher/0.0.1-dev/20260812-false-positive-appeal/publisher-0.0.1-dev-local-dist-zip-sha256-20260812.md` |
| Avast latest-definition rescan record | `docs/evidence/publisher/0.0.1-dev/20260812-false-positive-appeal/publisher-0.0.1-dev-local-dist-zip-avast-latest-definition-rescan-20260812.md` |

The rescan record states that `ashUpd.exe vps` timed out once after 120
seconds and that the second attempt ended with `The operation was canceled by
the user`. The observed Avast definition state was read after those attempts.
The update command must therefore be treated as timeout / user canceled, not
as an update success.

## Interpretation

Detection not reproduced is useful local technical evidence for the exact
existing local ZIP identified above. It is not a vendor response, vendor
clearance, Avast safety certification, responsible-owner approval, owner risk
acceptance, release authorization, package approval, tag authorization,
publication authorization, or distribution authorization.

Because responsible-owner approval / owner risk acceptance has not been
recorded for this latest-definition rescan evidence, release authorization is
not granted and the release gate remains blocked for any future release-path
operation.

## Prohibited Operations Not Performed

This docs-only / local-only summary did not perform:

- release, tag, publication, or distribution;
- package creation, package update, `dist` creation, or `dist` update;
- Live E2E;
- Google Docs or Google Drive mutation;
- flagged executable re-run;
- staging, commit, or push.
