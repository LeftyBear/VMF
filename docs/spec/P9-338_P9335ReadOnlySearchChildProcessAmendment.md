# P9-338 — P9-335 Read-Only Search Child-Process Amendment

## 1. Record status and decision boundary

- Work item: `P9-338`
- Route: `C`
- Work mode: `docs-only`
- Decision date: `2026-09-30`
- Decision owner: `VMF Owner (User)`
- Record result: `ACCEPT / BOUNDED NATIVE RG READ-ONLY SEARCH PERMITTED`
- Security disposition: `HOLD / CONDITIONAL DOCUMENTARY CONTINUATION ONLY`
- Residual risk: `UNRESOLVED / NOT ACCEPTED`
- Continuation authorization: `NOT GRANTED`
- Technical execution: `NO-GO / SAFE-STOP`
- Broader workflow: `SAFE-STOP`

The VMF Owner accepts a prospective amendment to the P9-335 PowerShell
child-process boundary. P9-335 remains the authoritative historical original
and is not rewritten. Except for the bounded native `rg.exe` read-only search
class defined by this record, every P9-335 restriction remains unchanged.

This amendment grants no evidence or control acceptance, residual-risk
acceptance, security clearance, continuation authorization, technical command
authorization, technical GO, EV-09, implementation, verification, execution,
or `SAFE-STOP` clearance.

## 2. Amended child-process boundary

P9-335 permitted PowerShell to invoke only approved read-only Git inspection
child processes. Prospectively, the permitted classes are:

1. the approved P9-335 read-only Git inspection class; and
2. the bounded native `rg.exe` read-only search class in this record.

No arbitrary executable, child process, helper program, wrapper, script, or
substituted binary is authorized.

## 3. Permitted purpose and source boundary

PowerShell may invoke the approved native `rg.exe` solely to search and display
matching text from already-existing local documentary sources during:

- docs-only governance review;
- static repository inspection;
- documentary evidence review; and
- bounded instruction or bootstrap inspection.

Permitted sources are limited to existing repository text files or
directories, supplied instruction attachments, task-relevant existing Codex
session or documentary text, task-relevant existing Codex memory text, and
other existing local documentary sources only when the instruction packet or
controlling record explicitly identifies the source or bounded root.

Each input path must be existing, local, explicitly identified, and bounded to
the assigned documentary purpose. This amendment grants no general authority
to search a user profile. Network or special-device paths are prohibited.
Symlinks or reparse points must not be followed into an unspecified root.

## 4. Executable identity and required command shape

The command must resolve to the approved native `rg.exe`. Aliases, PowerShell
functions, scripts, wrappers, repository-local shadow executables, and
substituted binaries are prohibited. If executable identity cannot be
established sufficiently, the operation must fail closed.

Every invocation must include `--no-config` and use an allowlisted read-only
shape equivalent to:

```text
rg --no-config -n [approved-search-options] <pattern> -- <literal-existing-path>
```

The `--` path separator is mandatory before input paths.

## 5. Positive option allowlist

Only options affirmatively established as immediately auditable for one of the
following effects are permitted:

- search selection;
- matching;
- line-number or display formatting; or
- bounded file or type filtering.

Every other option is prohibited. A version-dependent, ambiguous, or
insufficiently understood option must fail closed. PCRE2 and every other
non-default search engine require separate review and authorization.

## 6. Explicitly prohibited features

The following are prohibited:

- configuration loading other than explicit `--no-config`;
- `--pre` and `--pre-glob`;
- `-z` and `--search-zip`;
- preprocessors, decompression helpers, helper-program invocation, and any
  option that invokes another process;
- network paths or resources and special-device paths;
- output-file redirection and filesystem mutation;
- following symlinks or reparse points into unspecified roots;
- unrestricted recursion across user profiles, temporary directories,
  credential stores, system directories, secrets, or unrelated user data; and
- PCRE2 or another non-default search engine without separate authorization.

## 7. Compound-command and bootstrap rules

A PowerShell sequence may combine permitted P9-335 built-in read operations
with permitted `rg` only when every component independently complies, the
composition remains read-only, source provenance remains identifiable, no
unapproved process is introduced, and no mutation or external access occurs.

A direct pipeline of the following shape is permitted only when the literal
source remains attributable and there is no intervening transformation, script
block, invocation operator, uncertain variable expansion, output redirection,
or provenance ambiguity:

```text
Get-Content -LiteralPath <approved-source> | rg ... -
```

Component compliance is insufficient when the composition introduces
ambiguity.

The recurring bootstrap class may read the supplied instruction packet, read
`VMF_CODEX_PLAYBOOK.md`, and search explicitly bounded, task-relevant existing
local documentary or memory text using compliant `rg`. Every component must
independently satisfy P9-335 as amended by P9-338.

## 8. Fail-closed rule

The operation must stop if executable identity, option allowlisting, source
scope or provenance, or local-only status is uncertain; a symlink or reparse
point could expand scope; a network or special-device path is involved;
configuration, preprocessing, decompression, helper execution, or another
unapproved child process occurs; output redirection or mutation is possible;
project logic could execute; or the operation differs materially from the
accepted class.

## 9. Historical boundary

This amendment is prospective. It does not retroactively cure, validate, or
reclassify P9-334 or any failed P9-337 run.

- P9-334 remains `NOT ACCEPTED / FAIL-CLOSED`.
- Failed P9-337 runs remain non-authoritative.
- P9-336's historical reassessment remains intact.

## 10. Governance-state preservation

- P9-332 remains `HOLD / CONDITIONAL DOCUMENTARY CONTINUATION ONLY`.
- Residual risk remains `UNRESOLVED / NOT ACCEPTED`.
- Continuation authorization remains `NOT GRANTED`.
- Technical execution remains `NO-GO / SAFE-STOP`.
- The broader workflow remains `SAFE-STOP`.
- Arbitrary executable invocation remains prohibited.
- No evidence acceptance, control acceptance, residual-risk acceptance,
  security clearance, technical command authorization, technical GO, EV-09,
  implementation, verification, execution, or downstream authority is created.

## 11. Verification and non-actions

Static documentary review confirms that P9-335 remains historically intact;
only its prospective child-process/search boundary is amended; native `rg.exe`
identity, mandatory `--no-config`, literal bounded sources, a positive option
allowlist, explicit prohibited features, compound-command restrictions, and
fail-closed behavior are recorded; arbitrary executable invocation remains
prohibited; and historical failed runs and all security, HOLD, risk,
continuation, technical, and workflow states remain unchanged.

No project script, test, build, parser, Excel operation, artifact or executable
verification, Avast operation, external-service access, package, `dist`,
release, tag, staging, commit, push, or other Git mutation was performed or
authorized.
