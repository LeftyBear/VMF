# P9-336 — P9-334 PowerShell Read-Only Compliance Reassessment

## 1. Record status and decision boundary

- Work item: `P9-336`
- Route: `C`
- Work mode: `docs-only`
- Decision date: `2026-09-30`
- Decision owner: `VMF Owner (User)`
- Record result: `P9-334 REMAINS NOT ACCEPTED / FAIL-CLOSED`
- P9-335 compatibility: `NOT COMPATIBLE`
- P9-334 substantive status: `NOT ACCEPTED`
- Technical execution: `NO-GO / SAFE-STOP`
- Broader workflow: `SAFE-STOP`

The VMF Owner accepts the completed read-only reassessment of the already
performed P9-334 PowerShell-hosted operation. This record evaluates procedural
compatibility with P9-335 only. It does not adopt, reject, validate, or otherwise
convert P9-334's substantive next-gate analysis.

## 2. Controlling boundary

P9-335 permits PowerShell only as an inspection host within its explicitly
bounded read-only scope. It permits child-process execution only for its defined
read-only Git information classes and prohibits shell chaining that invokes an
unapproved process.

P9-335 remains accepted and unchanged. It does not automatically or
retroactively authorize P9-334.

## 3. Exact established P9-334 operation

The contemporaneous P9-334 session evidence establishes this exact
PowerShell-hosted operation:

```powershell
Get-Content -Raw -LiteralPath 'C:\Users\biz\.codex\attachments\65ca40f8-d425-4ab8-a6f7-0aa73bd10fc6\貼り付けたテキスト.txt';
Get-Content -Raw -LiteralPath 'C:\Users\biz\Documents\Project\VMF\VMF_CODEX_PLAYBOOK.md';
rg -n "P9-333|P9-334|next independent gate|eligibility" 'C:\Users\biz\.codex\memories\MEMORY.md'
```

Provenance:

- session ID: `01a0f1ae-81c0-7f21-b1dd-df1f46f2729a`;
- session record:
  `C:\Users\biz\.codex\sessions\2026\09\30\rollout-2026-09-30T18-39-01-01a0f1ae-81c0-7f21-b1dd-df1f46f2729a.jsonl`;
- exact command record: line `14`; and
- completed command-output record: line `16`.

P9-336 did not recreate, approximate, or rerun this operation.

## 4. P9-335 eight-condition determination

| # | Condition | Result | Basis |
|---|---|---|---|
| 1 | Strictly read-only | `PASS` | Each operation retrieved or searched already-existing text. |
| 2 | No file/repository mutation | `PASS` | `Get-Content` and the recorded `rg` invocation had read-only effects; no output redirection or mutating command was present. |
| 3 | No project/generated `.ps1` execution | `PASS` | No `.ps1` file was invoked. |
| 4 | No unapproved child process | `FAIL` | PowerShell invoked `rg`, which is not within P9-335's permitted read-only Git child-process class. |
| 5 | No technical verification | `PASS` | The operation inspected task, playbook, and memory text for a docs-only governance review. |
| 6 | No external access | `PASS` | Every referenced source was an already-existing local file. |
| 7 | No Avast interaction | `PASS` | The operation contained no Avast access or interaction. |
| 8 | Within P9-335 authorized class | `FAIL` | Although `Get-Content` is permitted, the chained `rg` child-process invocation is outside the authorized class and violates the shell-chaining boundary. |

Because conditions 4 and 8 are `FAIL`, the all-eight-conditions requirement is
not satisfied.

## 5. Historical authorization status

At the time P9-334 was executed, PowerShell use was prohibited by the
then-controlling boundary and by the P9-334 instruction packet. The operation
was therefore historically unauthorized.

P9-335 does not rewrite, cure, or retroactively authorize that historical fact.

## 6. P9-335 compatibility

Result: `NOT COMPATIBLE`.

The actual effects were read-only, but the operation fails P9-335 conditions 4
and 8 because PowerShell invoked `rg` as an unapproved child process through a
chained command.

## 7. P9-334 procedural and substantive disposition

Procedural classification:

`P9-334 REMAINS NOT ACCEPTED / FAIL-CLOSED`

Substantive status:

`NOT ACCEPTED`

P9-336 does not accept P9-334's substantive conclusions, select a next gate,
reject any proposed gate on its merits, or authorize preparation, review,
acceptance, or execution of any downstream gate.

## 8. Current governance-state preservation

- `G-145-U03` remains
  `RESOLVED / docs-only candidate selection / ACCEPT`.
- `G-145-U01` remains
  `RESOLVED / docs-only candidate-specific security disposition / ACCEPT — HOLD`.
- Security disposition remains
  `HOLD / CONDITIONAL DOCUMENTARY CONTINUATION ONLY`.
- Residual risk remains `UNRESOLVED / NOT ACCEPTED`.
- Continuation authorization remains `NOT GRANTED`.
- Technical execution remains `NO-GO / SAFE-STOP`.
- The broader workflow remains `SAFE-STOP`.
- P9-335 remains accepted and unchanged.

No evidence, control, control-effectiveness, residual-risk, security,
continuation, command, technical-GO, execution-instruction, implementation,
test, or execution gate is accepted, resolved, or authorized by P9-336.

## 9. Verification and non-actions

Static review confirms that:

- the exact established operation and contemporaneous provenance are recorded;
- conditions 4 and 8 are `FAIL`;
- P9-334 remains `NOT ACCEPTED / FAIL-CLOSED`;
- P9-334's substantive analysis is not accepted;
- P9-335 remains unchanged;
- historical authorization status is preserved;
- technical execution remains `NO-GO / SAFE-STOP`;
- the broader workflow remains `SAFE-STOP`; and
- no downstream gate is resolved.

P9-334 and its historical PowerShell command were not rerun. No project or
generated `.ps1`, test, build, parser, Excel operation, artifact or executable,
external service, Avast operation, staging, commit, push, tag, or other Git
mutation was performed.

## 10. Decision result and next boundary

Result:

`P9-334 REMAINS NOT ACCEPTED / FAIL-CLOSED`

Any future next-gate eligibility analysis requires a separate explicit Route C
instruction and a newly performed compliant review. P9-334's substantive
analysis must not be reused as accepted decision material merely because its
procedural compatibility was reassessed.
