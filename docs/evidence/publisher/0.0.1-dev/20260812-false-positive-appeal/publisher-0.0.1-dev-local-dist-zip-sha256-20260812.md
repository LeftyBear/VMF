# Publisher 0.0.1-dev Local Dist ZIP SHA-256 Record

Date: 2026-08-12
Local time: 2026-08-12T17:38:40+09:00
UTC time: 2026-08-12T08:38:40Z
Purpose: Latest-definition Avast rescan hash computation record

## Artifact

| Field | Value |
| --- | --- |
| Artifact role | Existing local `dist` ZIP selected for static Avast rescan |
| Artifact name | `vmf-publisher-0.0.1-dev-win-x64.zip` |
| Artifact path | `dist/release/Publisher/vmf-publisher-0.0.1-dev-win-x64.zip` |
| Artifact size | 983422 bytes |
| Last write time UTC | 2026-08-12T02:13:47Z |
| Hash algorithm | SHA-256 |
| Hash | `0174810D21C6072B8206ACF2FED90B72C2E6BE499C65B231D7D72D71FD69CB76` |

## Command Evidence

```powershell
Get-FileHash -Algorithm SHA256 -LiteralPath 'dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip'
Get-Item -LiteralPath 'dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip'
```

Observed result:

```text
Algorithm: SHA256
Hash: 0174810D21C6072B8206ACF2FED90B72C2E6BE499C65B231D7D72D71FD69CB76
Name: vmf-publisher-0.0.1-dev-win-x64.zip
Length: 983422
LastWriteTimeUtc: 2026-08-12T02:13:47Z
```

## Boundary Notes

- This record hashes the exact ZIP file selected for the local static Avast rescan.
- This is not the published GitHub Release asset identity. Existing records identify the published asset as 983404 bytes / SHA-256 `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6`.
- This record does not hash `vmf-publisher.exe` inside the ZIP and must not be used as executable-hash evidence.
- No package, `dist`, tag, release, publication, Live E2E, Google Docs, or Google Drive mutation was performed by this hash computation.
