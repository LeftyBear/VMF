# P9-123 — Static Command Allowlist Drafting

## Status

- P9-123: **COMPLETE / docs-only static command allowlist drafting**
- Command authorization: **No**
- Technical execution: **NO-GO / SAFE-STOP**
- Avast detection: **unresolved**
- P9-94 allowance: **not reusable**

This record classifies command categories only. It does not authorize or execute any command. Every otherwise allowable read-only command requires a separate, explicit GO before use.

## Static command classification

| Command category | Example command form | Classification | Reason |
| --- | --- | --- | --- |
| Git working-tree status | `git status --short` | Requires separate GO | Read-only repository inspection, but command authorization is currently No. |
| Git branch inspection | `git branch --show-current` | Requires separate GO | Read-only branch inspection, but command authorization is currently No. |
| Git revision inspection | `git rev-parse HEAD` | Requires separate GO | Read-only revision inspection, but command authorization is currently No. |
| Git whitespace validation | `git diff --check` | Requires separate GO | Read-only diff validation, but command authorization is currently No. |
| Read-only file inspection | Read an explicitly named repository file without modification | Requires separate GO | Inspection must remain limited to an explicitly named file and separately authorized. |
| Named Markdown search | `rg <pattern> <explicitly-named.md>` | Requires separate GO | Search must be limited to explicitly named Markdown files and separately authorized. |
| Parser | Any parser invocation | Prohibited | Parser execution is outside this docs-only boundary and remains NO-GO. |
| Scripts | Any script invocation | Prohibited | Script execution is outside this docs-only boundary and remains NO-GO. |
| PowerShell execution | Any PowerShell command or script | Prohibited | PowerShell execution is outside this docs-only boundary. |
| Excel, workbook, or process operations | Any Excel launch, workbook open, macro, process query, or process control | Prohibited | These operations cross the technical-execution boundary and remain NO-GO. |
| Tests or build | Any test runner or build command | Prohibited | Verification involving executable code is outside this docs-only task. |
| Package, dist, release, publication, or tag | Any packaging, distribution, release, publication, or tag operation | Prohibited | These operations require distinct authorization and are outside scope. |
| External services | Any external-service access or mutation | Prohibited | External access is not authorized. |
| Flagged executable | Any invocation of the flagged executable | Prohibited | Avast detection remains unresolved; execution remains fail-closed. |
| Avast workaround | Any Avast workaround, exception, exclusion, or bypass | Prohibited | Security controls must not be altered or bypassed. |

## Boundary

P9-123 completes drafting only. It grants no command authorization, continuation authorization, security disposition, or technical execution permission. The P9-94 allowance cannot be reused. Any later command requires a new, explicit GO applicable to that exact command and scope; prohibited categories remain prohibited unless a later authoritative decision expressly changes the boundary.
