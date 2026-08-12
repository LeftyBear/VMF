# Publisher 0.0.1-dev Local Dist ZIP Avast Latest-Definition Rescan

Date: 2026-08-12
Local time: 2026-08-12T17:38:40+09:00
UTC time: 2026-08-12T08:38:40Z
Purpose: Latest-definition Avast local static rescan evidence

## Scanner State

| Field | Value |
| --- | --- |
| Scanner vendor | Avast |
| Scanner product | Avast Antivirus |
| Scanner product version | 26.7.11086.1051 (`AvastUI.exe`) / 26.7.11086.0 (`ashQuick.exe`) |
| Definition / signature version | VPS `26081104`; VPSVersion `18`; VPSType `production`; stream definition directories observed through `26081202_stream` |
| Definition confirmation time | 2026-08-12T17:38:40+09:00 / 2026-08-12T08:38:40Z |
| Update command result | `ashUpd.exe vps` timed out once after 120 seconds; second attempt ended with `The operation was canceled by the user`; observed definition state above was read after those attempts |
| Scan mode | Local static quick scan using `ashQuick.exe` against the selected ZIP file |

## Scanned Artifact

| Field | Value |
| --- | --- |
| Artifact role | Existing local `dist` ZIP selected for static Avast rescan |
| Artifact name | `vmf-publisher-0.0.1-dev-win-x64.zip` |
| Artifact path | `dist/release/Publisher/vmf-publisher-0.0.1-dev-win-x64.zip` |
| Artifact size | 983422 bytes |
| Hash algorithm | SHA-256 |
| Hash | `0174810D21C6072B8206ACF2FED90B72C2E6BE499C65B231D7D72D71FD69CB76` |
| Target commit | `d3a71a0` |
| Previous detection name | `IDP.HELU.PSD11` |

## Scan Command

```powershell
& 'C:\Program Files\Avast Software\Avast\ashQuick.exe' 'C:\Users\<redacted>\Documents\Project\VMF\dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip'
```

Observed result:

```text
Exit code: 0
Standard output: empty
Standard error: empty
No Avast deletion, quarantine event, block message, or `IDP.HELU.PSD11` detection was observed during the command run.
```

## Interpretation

Selected result: Detection not reproduced.

This local latest-definition static scan did not reproduce the previous Avast detection for the exact local ZIP artifact identified above. This is technical evidence only. It is not Avast vendor clearance, Avast safety certification, responsible-owner approval, release authorization, package approval, tag authorization, publication authorization, or distribution authorization.

## Evidence References

| Evidence Type | Reference |
| --- | --- |
| Hash computation record | `docs/evidence/publisher/0.0.1-dev/20260812-false-positive-appeal/publisher-0.0.1-dev-local-dist-zip-sha256-20260812.md` |
| Scanner state and scan result record | `docs/evidence/publisher/0.0.1-dev/20260812-false-positive-appeal/publisher-0.0.1-dev-local-dist-zip-avast-latest-definition-rescan-20260812.md` |

## Boundary Notes

- The scanned file was the existing local `dist` ZIP. No package or `dist` file was created, changed, or replaced.
- The scanned file is not the previously recorded published GitHub Release asset and is not the executable inside the package.
- The ZIP hash in this record must not be used as executable hash evidence.
- The scan result does not change the recorded state that Avast vendor clearance is not obtained and Avast safety certification is not claimed.
