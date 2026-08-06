# Publisher v1.0 Implementation Voyage Log

## Phase 2-2: Inline TextStyle Live Verification

| Field | Value |
|---|---|
| Date | 2026-07-20 |
| Status | PASS |
| Source | `samples/publisher-poc.md` |
| Authentication | OAuth Desktop with persisted token reuse |
| Destination | Configured Google Drive folder |
| Document ID | `1JeWru-UMnsoQmhdA2VETEkvwexIjQakZJuhTL3V2174` |
| Document URL | <https://docs.google.com/document/d/1JeWru-UMnsoQmhdA2VETEkvwexIjQakZJuhTL3V2174/edit> |
| Verification surfaces | Publisher CLI, Google Docs API readback, and Google Docs UI |

### Execution

The sample was published with:

```powershell
dotnet run --project src/Publisher.Cli -- publish samples/publisher-poc.md
```

The CLI reported successful Google Drive and Google Docs API calls. The returned
Document ID matched the ID in the returned document URL. The generated document
was then read through `documents.get` and its `StructuralElement`, paragraph,
bullet, and `TextRun.TextStyle` data were inspected.

### Inline style readback

Google Docs indexes below are UTF-16 code-unit ranges with an exclusive end.
Fields omitted by the API are recorded as `false` for this verification.

| Context | Range | Text | Bold | Italic | Link URL |
|---|---:|---|:---:|:---:|---|
| Paragraph | 50-54 | `bold` | true | false | - |
| Paragraph | 56-62 | `italic` | false | true | - |
| Paragraph | 64-75 | `bold italic` | true | true | - |
| Paragraph link | 84-95 | `inline link` | false | false | `https://example.com/` |
| Underscore bold | 146-161 | `underscore bold` | true | false | - |
| Underscore italic | 163-180 | `underscore italic` | false | true | - |
| Heading | 309-323 | `Styled heading` | true | false | - |
| Heading | 329-340 | `italic text` | false | true | - |
| Heading link | 348-359 | `inline link` | false | false | `https://example.com/heading` |
| Unordered level 1 | 388-407 | `Unordered level one` | true | false | - |
| Unordered level 2 | 421-440 | `Unordered level two` | false | true | - |
| Unordered level 3 | 456-477 | `Unordered level three` | true | true | - |
| Unordered level 4 link | 493-513 | `Unordered level four` | false | false | `https://example.com/unordered` |
| Ordered level 1 | 530-547 | `Ordered level one` | true | false | - |
| Ordered level 2 | 571-588 | `Ordered level two` | false | true | - |
| Styled ordered link | 611-630 | `Ordered level three` | true | true | `https://example.com/ordered` |
| Mixed list level 1 | 654-664 | `mixed list` | true | false | - |
| Mixed list level 2 | 700-712 | `second level` | false | true | - |
| Mixed list styled link | 729-740 | `third level` | true | false | `https://example.com/mixed` |
| Mixed list link remainder | 740-761 | ` returns to unordered` | false | false | `https://example.com/mixed` |
| Mixed list level 4 | 767-779 | `fourth level` | true | true | - |
| Post-list paragraph | 805-838 | `styled paragraph follows the list` | true | false | - |

The adjacent plain-text runs did not contain `bold`, `italic`, or `link`
properties. The styled-link cases therefore confirmed both complete and partial
label decoration without leaking style to surrounding text.

### List structure and range alignment

The uninterrupted unordered list used one `listId` with `nestingLevel` 0-3 and
the glyph sequence `disc/circle/square/disc`. This is the readback signature of
`BULLET_DISC_CIRCLE_SQUARE`. Its paragraph ranges were 388-421, 421-456,
456-493, and 493-530.

The uninterrupted ordered list used one `listId` with `nestingLevel` 0-2 and
the glyph types `DECIMAL/ALPHA/ROMAN`. This is the readback signature of
`NUMBERED_DECIMAL_ALPHA_ROMAN`. Its paragraph ranges were 530-571, 571-611,
and 611-652.

The mixed list changed list preset at every item. Google Docs consequently
returned a distinct `listId` and `nestingLevel=0` for each item, while the
paragraph indentation preserved the Markdown hierarchy:

| Markdown level | Kind | Paragraph range | Indent start | First-line indent |
|---:|---|---:|---:|---:|
| 1 | Unordered | 652-696 | 36 pt | 18 pt |
| 2 | Ordered | 696-725 | 72 pt | 54 pt |
| 3 | Unordered | 725-763 | 108 pt | 90 pt |
| 4 | Ordered | 763-800 | 144 pt | 126 pt |

The leading Markdown tabs were absent from the published text. All inline style
ranges remained aligned after their removal. The following normal paragraph
started at index 800, ended at 939, had no `listId` or list indentation, and its
bold run occupied 805-838. No list style or index offset leaked into it.

### Success criteria

| Criterion | Result |
|---|---|
| `Bold=true` occurs only on intended ranges | PASS |
| `Italic=true` occurs only on intended ranges | PASS |
| `Link.Url` values match the Markdown URLs | PASS |
| Combined bold and italic styling is present | PASS |
| List-item style ranges remain aligned after tab removal | PASS |
| The post-list paragraph has no style or index drift | PASS |
| Google Docs API readback matches the Google Docs UI | PASS - visually confirmed by the user on 2026-07-20 |

No credential contents, access tokens, refresh tokens, or client secrets were
captured in this record.

## Phase 2-3: Markdown Table Live Verification

| Field | Value |
|---|---|
| Date | 2026-07-20 |
| Status | PASS |
| Source | `samples/publisher-poc.md` |
| Authentication | OAuth Desktop with persisted token reuse |
| Destination | Configured Google Drive folder |
| Document ID | `1324qmL6hwc-zDpGmYE34ntIxQIoB3M77z8VQGIdfz24` |
| Document URL | <https://docs.google.com/document/d/1324qmL6hwc-zDpGmYE34ntIxQIoB3M77z8VQGIdfz24/edit> |
| Verification surfaces | Publisher CLI, Google Docs API readback, and Google Docs UI in Chrome |

### Execution

The sample was published with the complete current CLI command:

```powershell
dotnet run --project src/Publisher.Cli -- publish samples/publisher-poc.md
```

The CLI reported successful Google Drive and Google Docs API calls. The returned
Document ID matched the ID embedded in the returned document URL. The document
was read back through `documents.get`, including its `StructuralElement`,
`Table`, `TableRow`, `TableCell`, `ParagraphStyle`, and `TextRun.TextStyle`
fields.

### Table structure and cell text

Google Docs indexes below are UTF-16 code-unit ranges with an exclusive end.
Each API table dimension matched the Markdown source.

| Table | API range | Rows x columns | Cell text by row |
|---|---:|---:|---|
| Outer-pipe table | 1077-1167 | 4 x 3 | `Name / Status / Note`; `Publisher / Active / v1.0`; `Renderer / Ready / 100%`; `Empty / [empty] / Right` |
| No-outer-pipe table | 1342-1416 | 3 x 3 | `Name / Status / Note`; `Parser / Ready / Right`; `Escaped pipe / A \| B / Safe` |

The empty cell at row 4, column 2 had the cell range 1157-1159. Its only text
run was the required cell-ending newline at 1158-1159; no cell body text or
placeholder was inserted. The escaped source pipe read back as the ordinary
text `A | B` at 1403-1409.

### ParagraphStyle and TextStyle readback

Every cell contained one `NORMAL_TEXT` paragraph. Paragraph alignment was
consistent for every row: column 1 was `START`, column 2 was `CENTER`, and
column 3 was `END`.

| Context | Range | Text | Bold | Italic | Link URL |
|---|---:|---|:---:|:---:|---|
| Table 1 header, column 1 | 1080-1085 | `Name` | true | false | - |
| Table 1 header, column 2 | 1086-1093 | `Status` | true | false | - |
| Table 1 header, column 3 | 1094-1099 | `Note` | true | false | - |
| Bold cell content | 1101-1111 | `Publisher` | true | false | - |
| Italic cell content | 1120-1125 | `v1.0` | false | true | - |
| Linked cell content | 1127-1135 | `Renderer` | false | false | `https://example.com/` |
| Table 2 header, column 1 | 1345-1350 | `Name` | true | false | - |
| Table 2 header, column 2 | 1351-1358 | `Status` | true | false | - |
| Table 2 header, column 3 | 1359-1364 | `Note` | true | false | - |

Adjacent plain-text runs did not contain unintended bold, italic, or link
properties.

### Post-table paragraph placement

The first table ended at index 1167. The following normal paragraph started at
1167 and ended at 1298, so there was no index gap or overlap. Its intended bold
run, `styled paragraph follows the table`, occupied 1172-1206.

The second table ended at index 1416. The following normal paragraph started at
1416 and ended at 1534. Google Docs rendered it at the start of page 2 because
the preceding table reached the bottom of page 1; API adjacency confirmed that
pagination did not introduce content or range drift.

### Google Docs visual comparison

The generated document was opened in the authenticated Chrome profile. The
first table appeared immediately after its preceding paragraph, and its
following styled paragraph appeared immediately after the table. Both tables
visually had three columns and the expected row counts. Headers were bold;
column 1 was left-aligned, column 2 centered, and column 3 right-aligned.
`Publisher` was bold, `v1.0` italic, and `Renderer` displayed as a link. The
empty cell remained blank, and the second table displayed `A | B` as ordinary
text. The visible Google Docs rendering matched the API readback.

### Success criteria

| Criterion | Result |
|---|---|
| Table structures match the expected row and column counts | PASS |
| Every cell body matches the Markdown source | PASS |
| Header cells read back with `Bold=true` | PASS |
| Column alignments read back as `START`, `CENTER`, and `END` | PASS |
| Inline `TextStyle` and `Link.Url` values match the source | PASS |
| The empty cell contains no unnecessary body text | PASS |
| Escaped pipes display as ordinary characters | PASS |
| Following paragraphs start at each table `EndIndex` | PASS |
| Google Docs API readback matches the Google Docs UI | PASS - visually confirmed in Chrome on 2026-07-20 |

No credential contents, access tokens, refresh tokens, or client secrets were
captured in this record.

## Phase 2-4: Code and Quote Live Verification

| Field | Value |
|---|---|
| Date | 2026-07-20 |
| Status | PASS |
| Source | `samples/publisher-poc.md` |
| Authentication | OAuth Desktop with persisted token reuse |
| Destination | Configured Google Drive folder |
| Document ID | `1C7ZdUxlJhhc4nc5P5yZAWODJ_BRfTqg1ACYb3m5LvOY` |
| Document URL | <https://docs.google.com/document/d/1C7ZdUxlJhhc4nc5P5yZAWODJ_BRfTqg1ACYb3m5LvOY/edit> |
| Verification surfaces | Publisher CLI, Google Docs API readback, and Google Docs UI in Chrome |

### Execution and compatibility correction

The complete sample was published with:

```powershell
dotnet run --project src/Publisher.Cli --configuration Release --no-build -- publish samples/publisher-poc.md
```

An initial live readback found that applying `weightedFontFamily` after `bold`
removed the effective bold weight in Google Docs. Text-style operation ordering
was corrected to apply code formatting before overlapping bold formatting, and
table-header bold was likewise moved after cell inline styles. Unit and
integration tests passed before the final publication above.

`BorderLeft` was intentionally omitted because the compatibility fallback is
the v1.0 contract: incremental left indentation plus whole-quote italics.

### Fenced code and inline-code readback

The opening and closing fences and the `csharp` language string were absent from
the published text. The literal code occupied paragraphs 1583-1610 and
1610-1635. `**not bold**` remained literal and had neither bold nor italic
styling. Both lines read back with `weightedFontFamily.fontFamily=Roboto Mono`,
light gray background color, 18 pt start/end indentation, and 6 pt space
above/below. The generated code-style range excluded the required final newline.

Inline code read back with Roboto Mono and the same background in every required
context:

| Context | Range | Text |
|---|---:|---|
| Paragraph | 1448-1459 | `dotnet test` |
| Heading | 1648-1660 | `inline code` |
| List | 1677-1688 | `dotnet test` |
| Quote | 1931-1942 | `inline code` |
| Table cell | 1820-1832 | `dotnet test` |

The nested-list `inline code` and table-cell `bold code` both read back with
`Bold=true` after the compatibility correction. The table-cell `linked code`
retained both its code style and `https://example.com/code` link target.

### Quote readback

Quote levels 1 through 6 read back with start indentation of 18, 36, 54, 72,
90, and 108 pt. The seven-marker sample also read back at 108 pt, confirming
normalization to level 6. Every non-empty quote run was italic, while nested bold,
link, and inline-code styles remained present. Space above and below was 3 pt;
the requested zero first-line indentation was represented by the omitted API
default value.

The empty quote paragraph occupied 2107-2108 and retained its quote indentation.
The final level-1 quote ended at 2152, and the following normal paragraph began
at 2152 with no quote indentation or italic style. Malformed inline constructs
remained literal and did not terminate publication.

### Google Docs visual comparison

The final document was opened in the authenticated Chrome profile. The code
lines visibly used a monospaced face and light gray background without visible
fences or language text. Inline code appeared in the heading, paragraph, list,
table, and quote. Bold code was visibly bold in both list and table contexts.
Quotes were italic and progressively indented through six levels; the excess
level matched level 6, the empty quote line was visible as spacing, and `After
quote.` returned to the normal paragraph position and style.

### Success criteria

| Criterion | Result |
|---|---|
| Fences and language are excluded from output | PASS |
| Code-body Markdown remains literal | PASS |
| Code font, background, indentation, and spacing read back correctly | PASS |
| Final code newline is excluded from the generated background range | PASS |
| Inline code works in all five required contexts | PASS |
| Bold, italic, and link overlap remains effective | PASS |
| Quote levels 1-6 and excess-depth normalization are correct | PASS |
| Empty quote line and post-quote placement are preserved | PASS |
| Malformed syntax completes without exception | PASS |
| Google Docs API readback matches the Google Docs UI | PASS - visually confirmed in Chrome on 2026-07-20 |

No credential contents, access tokens, refresh tokens, or client secrets were
captured in this record.

## Phase 2-5: Markdown Image Implementation and Verification

| Field | Value |
|---|---|
| Date | 2026-07-20 |
| Implementation status | COMPLETE |
| Automated verification | PASS |
| Live Google Docs publication | PASS |
| Document ID | `1aD78YrEaHAFsoy1-QUq4Qwhq06A_scVBLn7zD_Hove4` |
| Document URL | <https://docs.google.com/document/d/1aD78YrEaHAFsoy1-QUq4Qwhq06A_scVBLn7zD_Hove4/edit> |
| Verification surfaces | Publisher CLI, Google Docs API readback, and Google Docs UI in Chrome |
| Source | `samples/publisher-poc.md` and `samples/images/publisher-image-sample.png` |

### Implemented behavior

Standalone Markdown image lines now compile to isolated image publish steps.
Inline images and image-looking lines with trailing content remain ordinary
paragraphs. Empty Alt Text and escaped `\]` are accepted.

Local paths are normalized relative to the Markdown file and restricted to PNG,
JPEG, and GIF with matching extensions and signatures. Remote sources require
absolute HTTP(S) URIs without credentials. Host names, every DNS result, and
each manual redirect target are checked to reject localhost, loopback, private,
link-local, and unique-local destinations.

Metadata inspection reads pixel dimensions, DPI, and MIME type, defaulting to
96 DPI. Output is capped at 450 pt, retains pixel aspect ratio, and is not
enlarged by default. Local images use a short-lived Drive file with a generated
`publisher-temp-{guid}` name, SHA-256 appProperty, and `anyone/reader` permission
only through insertion/readback. Cleanup runs in `finally`; cleanup failure is
logged without hiding an earlier exception.

Google Docs insertion requests specify calculated width/height and START
paragraph alignment. A newline immediately after the inline object terminates
the independent image paragraph. Each insertion is read back to verify its
`InlineObjectElement`, `InlineObjectId`, and actual size. The containing image
paragraph's returned `EndIndex`, rather than inferred structural length, becomes
the following insertion index. Alt Text remains in the publish model and emits
`IMAGE_ALT_TEXT_UPDATE_FAILED` as a warning because
`InsertInlineImageRequest` exposes no Title or Description update field.

### Automated evidence

| Check | Result |
|---|---|
| Release solution build | PASS - 0 warnings, 0 errors |
| Publisher Unit Tests | PASS - 132/132 |
| Publisher Integration Tests | PASS - 6/6 |
| Image-focused Unit Tests | PASS - 33/33 |
| `dotnet format --verify-no-changes` | PASS |
| `git diff --check` | PASS |

### Live Google Docs evidence

After explicit approval for external publication and temporary public access,
the complete PoC was published with the Release CLI. The first run exposed a
Google Docs structural constraint: inserting only an inline object left it in
the terminal paragraph, whose `EndIndex` was not a valid location for following
content. The insertion batch was corrected to add the paragraph-terminating
newline immediately after the object. The final run completed successfully.

Both local and remote images returned an `InlineObjectElement`, non-empty
`InlineObjectId`, actual width and height matching the publish plan, and a valid
containing-paragraph `EndIndex`. Subsequent text was inserted from those returned
indexes. The local PNG was uploaded, granted temporary `anyone/reader` access,
inserted and read back, then deleted successfully before the CLI reported
success. Two expected `IMAGE_ALT_TEXT_UPDATE_FAILED` warnings confirmed that
both Alt Text values remained in the model but could not be mapped to unsupported
Title or Description request fields.

### Google Docs visual comparison

After the native messaging host was re-registered, the final document was
opened in the authenticated Chrome profile. The local Publisher pipeline image
and remote landscape image were both visible, START-aligned at the left margin,
and visually matched the API-verified 450 pt maximum-width plan while retaining
their expected aspect ratios. The local-image following paragraph appeared
immediately after the first image, and the remote-image following paragraph
appeared immediately after the second, with no overlap or index drift. The
subsequent malformed-syntax paragraph remained ordinary literal text on the
next page.

No access token, credential content, image public URI, or temporary Drive file
identifier was logged.

## Phase 3-1.5: Diff Planning Hardening Decisions

| Field | Value |
|---|---|
| Date | 2026-07-22 |
| Scope | Pure differential planning, fingerprint generation, and Unit Tests |
| Frozen specification changes | None |


| Google Docs update execution | Out of scope |
| Persistence, CLI, Managed Region, Snapshot, Recovery | Out of scope |

### Identity and conflict decisions

The Diff Engine validates candidate identities before using the documented
`ExplicitId`, `GeneratedId`, `ContentHash` precedence. `ExplicitId` and
`GeneratedId` are each unique within the baseline and candidate. Duplicate
values stop planning with `DIFF_DUPLICATE_IDENTITY`.

When multiple identity tiers on one candidate resolve to different baseline
blocks, or separate candidate blocks attempt to consume one baseline block
through different strong identity tiers, planning stops with
`DIFF_IDENTITY_CONFLICT`. Priority is used only when every resolving identity
agrees or when only one identity resolves.

Content-hash fallback considers only blocks left unmatched by explicit and
generated identities. A hash is matched only when exactly one unmatched
baseline block and exactly one unmatched candidate block remain. Any viable
one-to-many, many-to-one, or many-to-many fallback stops with
`DIFF_CONTENT_HASH_AMBIGUOUS`. FIFO matching is not the v1.0 production
default.

Publication ID and Document ID use ordinal comparison. Google Document ID is
also ordinal: equal non-null values and two null values are accepted, while a
one-sided null or unequal values stop with `DIFF_DOCUMENT_IDENTITY_MISMATCH`.

### Verified baseline and candidate lifecycle

`VerifiedPublishState` and `PublishCandidate` are separate sealed types with no
common public state base. Both constructors are internal. Public callers create
a candidate only through `PublishCandidateFactory`, which binds one canonical
input to its generated fingerprint and aligned block identities. The
verified-state constructor is reserved for a future state manager restoring
data that was persisted only after target verification.

The intended future lifecycle is:

1. parse, validate, and prepare canonical candidate input;
2. atomically generate its versioned fingerprint and candidate through the factory;
3. compare it with a restored `VerifiedPublishState`;
4. apply the logical plan through a target-specific executor;
5. verify target readback;
6. only then construct and persist the next verified state.

A `PublishCandidate` cannot be passed where a `VerifiedPublishState` is
required. This phase does not implement promotion, persistence, or target
verification.

### Fingerprint canonical input

Fingerprint algorithm version 1 uses these inputs in this exact top-level
order:

1. canonical format name;
2. fingerprint algorithm version;
3. hash algorithm name;
4. Publisher implementation version;
5. Publisher transformation specification version;
6. PublishState schema version;
7. Publication ID;
8. Document ID;
9. output-setting count and settings;
10. document-block count and ordered document blocks.

Google Document ID is intentionally excluded. It identifies and protects the
update target but does not change the desired document content. It is checked
independently by the Diff Engine.

Every document block contains its zero-based order, nullable Explicit ID,
nullable Generated ID, ContentHash, fixed block-kind token, level, and complete
canonical Domain model payload. Payload serialization covers ordered nested
inline content, list kind/depth/items, table columns/alignments/rows/cells, code
language/text, quote level/content, and image alt text/source kind/source
value/optional dimensions. Collection order is always preserved.

Output settings are sorted by ordinal setting name before serialization. The
canonical-input constructor rejects a missing member of the current required
setting set. Additional output-affecting settings are allowed. For the current
Publisher pipeline the required set is:

- `markdown.inline.maxDepth`
- `markdown.list.indentSize`
- `markdown.list.maxDepth`
- `publisher.allowImageUpscale`
- `publisher.imageMaxWidthPoints`

Authentication, credential paths, token-store paths, Drive folder IDs,
temporary-hosting folder IDs, and application names are excluded because they
do not alter generated document content. Temporary public-hosting permission is
a transport/security policy and is also excluded.

### Canonical serialization and hash format

The canonical byte stream is a sequence of records:

```text
<field-name>:<UTF-8-byte-length>:<value-bytes>\n
```

Field names and record order are fixed by algorithm version 1. String values
normalize CRLF and CR to LF before encoding. Encoding is UTF-8 without BOM, and
record separators are LF. A null value uses length `-1` and no value bytes; an
empty string uses length `0`, so null and empty remain distinct.

Integers use invariant decimal form, Booleans use lowercase `true` or `false`,
and floating-point values use invariant round-trip form. Enum-like values use
fixed lowercase tokens rather than runtime enum names. URI values use their
absolute representation. All collection counts and item orders are explicit.

SHA-256 is applied to the canonical byte stream. The public fingerprint uses
lowercase hexadecimal with this version prefix:

```text
v1:sha256:<64 lowercase hexadecimal characters>
```

Algorithm or canonical-shape changes require a new algorithm version and value
prefix. `PublishFingerprint` has no public constructor and accepts only the
current prefix followed by exactly 64 lowercase hexadecimal characters inside
the Publisher boundary. A known-vector Unit Test protects version 1 from
accidental drift.

### Move semantics

Move detection constructs the sequence of matched baseline indexes in candidate
order and computes a deterministic strict longest increasing subsequence in
O(n log n). Matched blocks outside the selected LIS are logical `Move`
operations. Insertions and deletions that only shift absolute indexes do not
create false moves.

Diff operations describe logical source-to-target correspondence. Their list
order is not a Google Docs physical execution order. A future target executor
must translate the full logical plan into safe target-specific operations and
may expand a logical Move into Delete plus Insert.

### Automated evidence

| Check | Result |
|---|---|
| Phase 3-1.5 focused Unit Tests | PASS - 71/71 |
| Publisher Unit Tests | PASS - 203/203 |
| Publisher Integration Tests | PASS - 6/6 |
| Release solution build | PASS - 0 warnings, 0 errors |
| `dotnet format --verify-no-changes` | PASS |
| Frozen specification changes | None |

No Google Docs API call, state persistence, CLI integration, Managed Region
mutation, Snapshot, or Recovery behavior was added or executed in this phase.

## Phase 3-2A: Identity and Content Hash Pipeline Decisions

| Field | Value |
|---|---|
| Date | 2026-07-22 |
| Scope | Explicit ID parsing, generated block IDs, content hashes, and candidate construction |
| Frozen specification changes | None |
| Google Docs update execution | Out of scope |
| Persistence, CLI commands, Managed Region, Snapshot, Recovery | Out of scope |

### Generated identifier design comparison

Four designs were evaluated before implementation:

| Candidate | Decision | Reason |
|---|---|---|
| Parent context + block kind + content feature | Adopted | Stable under unrelated front or tail insertion and deletion, deterministic without persisted state, and aligned with the canonical model |
| Stable structural path | Rejected as default | Absolute or sibling ordinals cause cascading identifier changes after front insertion or deletion |
| Neighbor anchors | Rejected as default | Editing or inserting a neighbor propagates identity changes into otherwise unchanged blocks |
| Persisted UUID | Deferred | Provides the strongest edit stability but requires persistence and verified reinjection, which are outside Phase 3-2A |

The adopted algorithm uses heading ancestry as parent context, a fixed block-kind
token, and a separately domain-versioned digest of canonical block content. It
does not use the document block index. When multiple blocks have exactly the
same parent context, kind, and canonical content, a zero-based occurrence is
used only inside that equivalence class. An unrelated insertion or deletion
therefore does not renumber existing identifiers.

Content edits change the generated content feature and therefore may change the
Generated ID. Authors must use an Explicit ID for blocks whose identity must
survive content edits. This limitation cannot be removed deterministically
without an author-provided or persisted stable identifier.

### Explicit block identifier syntax

An Explicit ID is declared by a reserved standalone directive associated with
the next non-blank canonical block:

```markdown
<!-- vmf:block-id=introduction -->
## Introduction
```

The canonical directive spelling is case-sensitive. Horizontal whitespace is
accepted around `=` and before the comment close. The value is trimmed,
normalized to Unicode NFC, and stored on `DocumentBlock.ExplicitId`. Absence of
the directive produces `null`.

After NFC normalization, the identifier must contain at most 128 Unicode scalar
values. Its first scalar must be a Unicode letter or `_`. Remaining scalars may
be Unicode letters, digits, combining marks, `_`, `-`, `.`, or `:`. Case is
preserved and compared using ordinal semantics, so `Example` and `example` are
different identifiers. Empty, malformed, invalid, orphaned, and duplicate
directives stop parsing with stable pipeline error codes. Canonically equivalent
Unicode spellings are duplicates.

Blocks carrying an Explicit ID do not receive a Generated ID. This keeps the
authoritative identity unambiguous and avoids introducing a second strong tier
that could conflict with an author-supplied identifier.

### Generated ID algorithm version 1

The content-feature digest input is serialized in this order:

1. format `vmf-publisher-generated-id-feature-canonical`;
2. Generated ID Algorithm Version `1`;
3. hash algorithm `sha-256`;
4. canonical block kind, level, and complete payload.

The final Generated ID input is serialized in this order:

1. format `vmf-publisher-generated-block-id-canonical`;
2. Generated ID Algorithm Version `1`;
3. hash algorithm `sha-256`;
4. parent-heading count;
5. each parent heading level and anchor in ascending heading level;
6. fixed block-kind token;
7. content-feature digest;
8. zero-based occurrence within the same parent, kind, and feature equivalence class.

A heading anchor is `explicit:<ExplicitId>` when explicit, otherwise
`generated:<GeneratedId>`. Publication ID, Document ID, Google Document ID,
document index, absolute structural path, and unrelated neighbor values are not
inputs. Reordering distinct blocks in the same parent scope preserves their
Generated IDs. Moving a block to a different heading parent changes its ID by
design.

Both stages use SHA-256 and lowercase hexadecimal. The public representation is:

```text
gid-v1:sha256:<64 lowercase hexadecimal characters>
```

Any change to the canonical shape, parent semantics, feature projection, or
duplicate discriminator requires a new Generated ID Algorithm Version and
prefix.

### Content Hash algorithm version 1

The Content Hash input is serialized in this exact order:

1. format `vmf-publisher-block-content-canonical`;
2. Content Hash Algorithm Version `1`;
3. hash algorithm `sha-256`;
4. fixed block-kind token;
5. block level;
6. the complete canonical block payload.

The payload covers ordered nested inline content, list kind/depth/items, table
columns/alignments/rows/cells, code language/text, quote level/content, and
image alt text/source kind/source value/optional dimensions. Explicit ID,
Generated ID, document index, Publication ID, Document ID, Google Document ID,
and Document State are excluded because they are not content.

SHA-256 and lowercase hexadecimal produce:

```text
ch-v1:sha256:<64 lowercase hexadecimal characters>
```

Content Hash and Generated ID have independent interfaces, canonical domain
strings, versions, and prefixes. The Generated ID implementation does not call
the Content Hash generator. Both use the shared canonical block serializer so
their string, collection, number, URI, and enum rules cannot drift from the
Fingerprint model accidentally.

### Shared canonical serialization

Fingerprint, Generated ID features, Generated IDs, and Content Hashes use the
Phase 3-1.5 record format:

```text
<field-name>:<UTF-8-byte-length>:<value-bytes>\n
```

Encoding is UTF-8 without BOM. CRLF and CR normalize to LF. Record separators
are LF. Null uses length `-1`; empty uses length `0`. Integers use invariant
decimal form, Booleans use lowercase tokens, doubles use invariant round-trip
form, URIs use their absolute representation, enums use fixed lowercase tokens,
and every collection count and order is explicit.

### Candidate pipeline and composition

`PublishCandidateBuilder` validates Explicit ID uniqueness, requests aligned
Generated IDs and Content Hashes, builds `BlockIdentity` values, constructs the
complete `PublishFingerprintInput`, and delegates atomic candidate creation to
`PublishCandidateFactory`. A dedicated target-neutral `PublisherCompositionRoot`
binds the current generators without adding an Update CLI command.

The existing Create Mode remains on the same `PublishService` path. Markdown
without an identity directive is parsed and compiled as before. A directive is
metadata and is not rendered. Image preparation explicitly preserves the
block's Explicit ID when replacing the prepared image payload.

### Automated evidence

| Check | Result |
|---|---|
| Phase 3-2A focused Unit Tests | PASS - 39/39 |
| Publisher Unit Tests | PASS - 242/242 |
| Publisher Integration Tests | PASS - 7/7 |
| Release solution build | PASS - 0 warnings, 0 errors |
| `dotnet format --verify-no-changes` | PASS |
| Frozen specification changes | None |

No Google Docs update call, Verified State persistence, Update CLI command,
Managed Region mutation, Snapshot, or Recovery behavior was added or executed
in this phase.

## Phase 3-2B: Verified State Lifecycle Decisions

| Field | Value |
|---|---|
| Date | 2026-07-22 |
| Scope | Verified-state restoration, verification-gated promotion, state transitions, and atomic local JSON persistence |
| Frozen specification changes | None |
| Google Docs physical update and readback | Contract only; no implementation |
| CLI, Managed Region, Snapshot, Recovery | Out of scope |

### Lifecycle boundary and confirmation order

`PublishCandidate` remains unverified input. It cannot construct or replace a
`VerifiedPublishState`. A physical application adapter must instead provide a
`PublishApplicationVerification` containing the applied plan, target identity,
completion flags, applied fingerprint, and ordered read-back block identities.
`PublishResultVerifier` checks that evidence and is the only producer of the
non-publicly-constructible `VerifiedPublishResult`. Only that result can be
accepted by `VerifiedPublishStatePromoter`.

`VerifiedPublishLifecycle` enforces this order:

1. receive an already generated Candidate;
2. restore the optional verified Baseline and reject a prohibited state transition;
3. generate the logical DiffPlan;
4. call the target-neutral external apply-and-verify contract;
5. validate application and readback evidence;
6. promote the verified result to active state;
7. atomically save the complete state;
8. return success.

No success result exists before step 7 completes. Application, readback,
verification, transition, or save failure therefore cannot publish a new
Verified State. Phase 3-2B provides no Google Docs implementation of step 4.

### Persisted DocumentState and transitions

Absence is represented by no state record, not by a persisted enum value. Only
`active`, `missing`, and `archived` are serialized.

| From / To | Active | Missing | Archived |
|---|---:|---:|---:|
| No state | Allow | Deny | Deny |
| Active | Allow | Allow | Allow |
| Missing | Allow | Allow | Allow |
| Archived | Deny | Deny | Allow |

Missing-to-Active is allowed only after the normal verification-gated publish
flow. Missing-to-Archived permits terminal administrative retirement. Archived
is terminal in v1: implicit resurrection is prohibited until a future explicit
restore workflow defines its authorization and verification semantics. Invalid
transitions fail with `STATE_INVALID_TRANSITION`.

### Restore contract and version policy

The read contract uses `PublicationId` plus `DocumentId` as its logical key and
also requires the expected nullable `GoogleDocumentId`. All identity values use
ordinal comparison. Both Google IDs null is valid; a one-sided null or unequal
value fails with `STATE_DOCUMENT_IDENTITY_MISMATCH`. A missing optional baseline
returns null. Callers that require an existing record may report the reserved
`STATE_NOT_FOUND` code.

Restore validates the exact v1 field set, required values, JSON types, schema,
algorithm versions, known DocumentState token, canonical fingerprint and hash
prefixes, lowercase SHA-256 hex, Explicit ID canonical syntax and NFC form,
Explicit ID uniqueness, Generated ID uniqueness, exactly one strong identity
per block, and zero-based contiguous block order. Unknown or duplicated JSON
properties, malformed UTF-8, BOM, CR bytes, missing LF terminator, and invalid
or unknown values are corruption. No normalization, repair, fallback, or schema
migration is attempted.

The versions persisted independently are:

- Publish State Schema Version;
- Generated ID Algorithm Version;
- Content Hash Algorithm Version;
- Fingerprint Algorithm Version;
- Publisher Transformation Specification Version;
- Publisher implementation version.

Only schema version `1` is accepted by the configured v1 store. A different
schema fails with `STATE_SCHEMA_VERSION_UNSUPPORTED`; migration is deferred.
Unsupported identity, hash, or fingerprint algorithms fail with
`STATE_ALGORITHM_VERSION_UNSUPPORTED`. Transformation and Publisher versions
remain historical state metadata; a newer candidate's fingerprint determines
whether a new application is needed.

Stable lifecycle codes are `STATE_NOT_FOUND` (reserved for a required-load
caller), `STATE_CORRUPTED`, `STATE_SCHEMA_VERSION_UNSUPPORTED`,
`STATE_ALGORITHM_VERSION_UNSUPPORTED`, `STATE_DOCUMENT_IDENTITY_MISMATCH`,
`STATE_INVALID_TRANSITION`, `STATE_VERIFICATION_REQUIRED`,
`STATE_VERIFICATION_MISMATCH`, and `STATE_SAVE_FAILED`.

### Canonical local JSON and atomicity

The v1 state document uses UTF-8 without BOM, no indentation, fixed property
emission order, JSON arrays for block order, and exactly one LF terminator.
Null is emitted as JSON `null`; empty required strings are invalid. JSON string
escaping preserves the distinction between data characters and record bytes.
The stored shape is:

```json
{"format":"vmf-publisher-verified-state","schemaVersion":"1","generatedIdAlgorithmVersion":"1","contentHashAlgorithmVersion":"1","fingerprintAlgorithmVersion":"1","transformationSpecificationVersion":"1.0","publisherVersion":"1.0.0","publicationId":"publication","documentId":"document","googleDocumentId":"google-document","documentState":"active","publishFingerprint":"v1:sha256:<64 lowercase hex>","blocks":[{"order":0,"explicitId":"intro","generatedId":null,"contentHash":"ch-v1:sha256:<64 lowercase hex>"}]}
```

The file name is derived from a SHA-256 digest of length-delimited canonical
Publication ID and Document ID values, so user identifiers are never used as
paths. The root directory is supplied explicitly by the host and is not coupled
to Google credentials or local Google API configuration.

Save first serializes and revalidates the complete new record. It then writes a
uniquely named temporary file in the destination directory, flushes it to disk,
and commits by same-directory atomic rename for a new key or atomic replacement
for an existing key. If writing, flushing, or replacement fails, the temporary
file is removed and the prior state remains the committed record. The public
failure is `STATE_SAVE_FAILED`.

### Phase boundary

The Application layer owns restore/use contracts, evidence verification,
promotion, and lifecycle ordering. Domain owns persisted states, versions, and
transition rules. Infrastructure owns strict JSON and atomic file operations.
Composition registers the current algorithms and target-neutral services.

No Google Docs physical update, Google Docs readback, Update CLI command,
Managed Region mutation, Snapshot, or Recovery behavior was added in this
phase.

### Automated evidence

| Check | Result |
|---|---|
| Phase 3-2B focused Unit Tests | PASS - 76/76 |
| Publisher Unit Tests | PASS - 320/320 |
| Publisher Integration Tests | PASS - 8/8 |
| Release solution build | PASS - 0 warnings, 0 errors |
| `dotnet format --verify-no-changes` | PASS |
| `git diff --check` | PASS |
| Frozen specification changes | None |

## Phase 4-2-1: Diagnostic Logging Specification, Implementation, And Review

Architecture decision reference:
`docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md`
records the durable observability decision for Publisher structured JSON
diagnostics, stdout/stderr separation, lifecycle events, safe messages, and
redaction policy. The ADR is documentation-only and does not replace this
Phase 4-2-1 implementation record.

Status: DONE as local-only Phase 4 implementation.

Phase 4-2-1 standardized Publisher CLI diagnostic logging without changing
Frozen specifications, public APIs, persisted schemas, canonical formats,
release artifacts, Live E2E behavior, Google Docs, Google Drive, or the
Avast-pending package state.

### Implemented behavior

- Added required structured log fields: `timestampUtc`, `level`, `sessionId`,
  `command`, `phase`, `operation`, `code`, and `message`.
- Changed `SESSION_STARTED` to the command-independent message
  `Publisher diagnostic session started.`
- Added `COMMAND_STARTED`, `COMMAND_COMPLETED`, and `COMMAND_FAILED`
  lifecycle events.
- Preserved existing summary result codes and publish success
  `documentId` / `documentUrl` compatibility.
- Replaced external exception message exposure with classified safe messages
  and optional simple `exceptionType`.
- Removed local path and invalid setting value echo from diagnostic summaries.
- Normalized image warning messages while preserving existing warning codes and
  the `IPublisherLogger.Warning(string code, string message)` public contract.

### Release notes draft

Publisher Phase 4-2-1 improves local CLI diagnostic logs for troubleshooting
and evidence capture. Structured stderr logs now include command, phase, and
operation context plus command lifecycle events. Failure summaries use stable
safe messages instead of raw external exception messages, and image warning
messages are normalized without changing public APIs or live publish defaults.

This is local implementation work only. It does not approve a release, create
or update packages, execute Live E2E, mutate Google Docs or Google Drive, or
change the Avast release-gate status.

### Automated evidence

| Check | Result |
|---|---|
| Focused Publisher Unit Tests | PASS - 22/22 |
| Publisher Unit Tests | PASS - 467/467 |
| Publisher Integration Tests | PASS - 16/16, non-live |
| Release solution build | PASS - 0 warnings, 0 errors |
| `dotnet format --verify-no-changes` | PASS |
| `git diff --check` | PASS |
| Frozen specification changes | None |
| Public API changes | None |
| Live E2E / Google Docs / Google Drive mutation | Not executed |
| Release package / dist / tag / publication | Not changed |

## Phase 4-2-2: Error Handling Specification, Implementation, And Review

Status: DONE as local-only Phase 4 implementation.

Phase 4-2-2 standardized Publisher CLI error handling without changing Frozen
specifications, public APIs, persisted schemas, canonical formats, retry
policy, release artifacts, Live E2E behavior, Google Docs, Google Drive, or the
Avast-pending package state.

### Implemented behavior

- Added CLI `Verification` classification and mapped it to exit code `4`.
- Preserved exit codes `0`, `1`, `2`, `3`, `75`, and `130`.
- Kept `Input` and `Internal` on the existing generic publish failure exit code
  `1`.
- Mapped unknown, blank, or missing stable codes to `Internal`.
- Replaced publish and configuration failure summaries with
  classification-based fixed safe messages.
- Preserved publish success stdout and structured `documentId` / `documentUrl`
  compatibility.
- Preserved help / usage stderr behavior.
- Kept `OperationCanceledException` rethrown below the CLI boundary and
  converted to `CANCELED` / exit `130` at the CLI boundary.
- Preserved retry policy and delivery-state retry behavior.

### Release notes draft

Publisher Phase 4-2-2 improves local CLI failure behavior for automation and
support. Error summaries now use stable classifications and fixed safe messages,
verification and readback failures return exit code `4`, cancellation returns
exit code `130`, and unknown codes fall back to internal failure handling.
Sensitive provider, path, token, secret, and raw exception details are not
emitted in failure summaries.

This is local implementation work only. It does not approve a release, create
or update packages, execute Live E2E, mutate Google Docs or Google Drive, or
change the Avast release-gate status.

### Automated evidence

| Check | Result |
|---|---|
| Focused Publisher Unit Tests | PASS - 33/33 |
| Publisher Unit Tests | PASS - 490/490 |
| Publisher Integration Tests | PASS - 12/12, non-live |
| Release solution build | PASS - 0 warnings, 0 errors |
| `dotnet format --verify-no-changes` | PASS |
| `git diff --check` | PASS - no whitespace errors; CRLF normalization warnings only |
| Frozen specification changes | None |
| Public API changes | None |
| Live E2E / Google Docs / Google Drive mutation | Not executed |
| Release package / dist / tag / publication | Not changed |

### Final review

Phase 4-2-2 has no phase-specific release blocker. Continuing blockers remain
the release gate, Live E2E authorization, Google Docs / Google Drive mutation,
package / dist / tag / publication authorization, and the unchanged
Avast-pending package state.

vNext candidate: a separate input-specific public CLI exit code may be proposed
in a future task, but it was not adopted in Phase 4-2-2.

## Phase 4-2-3: Local Verify Report Improvement

Status: DONE as local-only Phase 4 implementation.

Phase 4-2-3 standardized the Publisher CLI `verify` report for human and
machine review without changing Frozen specifications, public APIs, persisted
schemas, canonical formats, validation logic, retry policy, release artifacts,
Live E2E behavior, Google Docs, Google Drive, or the Avast-pending package
state.

### Implemented behavior

- Added a `LOCAL_VERIFY_REPORT` JSON Lines diagnostic entry for the `verify`
  command while preserving existing lifecycle and summary diagnostics.
- Reported stable top-level `overallResult`, `exitCode`, `resultCode`,
  `safeSummary`, and `executedAtUtc` fields.
- Reported checks in stable order with `PASS`, `FAIL`, and `SKIPPED` statuses:
  `configuration`, `markdownCompilation`, `localOnlyBoundary`, `liveE2E`,
  `package`, `release`, and `publication`.
- Marked unexecuted Markdown compilation, Live E2E, package, release, and
  publication scope as `SKIPPED` instead of treating them as implicit success.
- Kept Live E2E, package, release, and publication outside the Local Verify
  success criteria.
- Included safe configuration and environment metadata such as .NET runtime,
  OS description, OS architecture, and process architecture.
- Continued to suppress raw exceptions, URIs, local paths, tokens, secrets,
  credential values, and HTTP bodies from Local Verify failure output.

### Release notes draft

Publisher Phase 4-2-3 improves local `verify` evidence for automation and
review. The CLI now emits a stable Local Verify report with explicit result,
exit code, check statuses, safe failure code and summary, execution timestamp,
configuration metadata, environment metadata, and local-only constraints.
Skipped external and release operations are reported explicitly and are not
part of the Local Verify success criteria.

This is local implementation work only. It does not approve a release, create
or update packages, execute Live E2E, mutate Google Docs or Google Drive, or
change the Avast release-gate status.

### Automated evidence

| Check | Result |
|---|---|
| Focused Publisher Unit Tests | PASS - 35/35 |
| Publisher Unit Tests | PASS - 492/492 |
| Publisher Integration Tests | PASS - 16/16, non-live |
| Release solution build | PASS - 0 warnings, 0 errors |
| `dotnet format --verify-no-changes` | PASS |
| `git diff --check` | PASS - no whitespace errors; CRLF normalization warnings only |
| Frozen specification changes | None |
| Public API changes | None |
| Live E2E / Google Docs / Google Drive mutation | Not executed |
| Release package / dist / tag / publication | Not changed |

### Final review

Phase 4-2-3 has no phase-specific release blocker. Continuing blockers remain
the release gate, Live E2E authorization, Google Docs / Google Drive mutation,
package / dist / tag / publication authorization, and the unchanged
Avast-pending package state.

## Phase 4-2-3: Retry Policy Specification Consolidation

Status: DONE as documentation-only specification consolidation.

Added `docs/development/Publisher_Phase4-2-3_RetryPolicySpecification.md` to
make Publisher retry behavior explicit without changing production code,
Frozen specifications, public APIs, persisted schemas, canonical formats,
validation logic, release artifacts, Live E2E behavior, Google Docs, Google
Drive, or the Avast-pending package state.

The specification records retryable and non-retryable failure classes, exit
code relationships, stable error-code relationships, transient failure
handling, verification failure non-retry policy, idempotency and safe retry
conditions, bounded backoff expectations, CLI behavior, structured logging
requirements, local-only test viewpoints, non-goals, and release-hold
continuation conditions.

The existing `Phase 4-2-3 Local Verify Report Improvement` implementation
record remains authoritative for the local verify report work. The retry-policy
record is a docs-only consolidation and does not renumber or replace that
implementation record.

### Explicit non-actions

This documentation update did not change production code, tests, Frozen
specifications, public APIs, persisted schemas, canonical formats, release
artifacts, packages, tags, publication state, Live E2E behavior, Google Docs,
Google Drive, token stores, credentials, temporary public hosting, or the
Avast-pending executable/package state.

## Phase 4-3: Release Readiness Review Records

Status: DONE as documentation-only Phase 4 release-readiness review.

Phase 4-3 added release-readiness review records without changing Frozen
specifications, public APIs, production design, release artifacts, Live E2E
behavior, Google Docs, Google Drive, or package state.

### Added records

- `Publisher_Phase4-3-1_ReleaseReadinessChecklist.md`
- `Publisher_Phase4-3-2_ReleaseCandidateVerification.md`
- `Publisher_Phase4-3-3_ReleaseArtifactAudit.md`
- `Publisher_Phase4-3-4_SecurityAndSupplyChainReview.md`
- `Publisher_Phase4-3-5_GoNoGoReview.md`

### Decision

The current formal state remains local-only verification complete / release
blocked. Phase 4-3 does not promote local-only verification to release
readiness.

The overall judgment is `DEFERRED` because these conditions remain unresolved:

- Avast false positive handling;
- Live E2E authorization or owner-approved N/A decision;
- release candidate artifact selection or authorized generation;
- release artifact audit;
- security and supply-chain review;
- repository-owner release approval.

### Explicit non-actions

Phase 4-3 did not release, create tags, publish artifacts, create or update
packages, execute Live E2E, mutate Google Docs or Google Drive, re-run flagged
artifacts, change Frozen specifications, change public APIs, or change
production design.

## Publisher Existing Test Classification And Resume Procedure Hardening

Status: DONE as documentation-only release-boundary hardening.

Added `docs/development/Publisher_TestClassification.md` to classify existing
Publisher verification targets while Avast false-positive handling remains
pending. The document separates documentation checks, Release build, unit
tests, focused unit tests, non-live integration tests, Live Google Docs E2E,
local CLI verification, packaged executable smoke, existing-package static
verification, package creation, and publication operations.

The classification preserves the current formal state:

`Phase 4 local-only verification complete / release blocked`.

It also records the resume order after an Avast response: record the vendor
response and artifact identity first, reopen only the explicitly authorized
gate, rerun local source verification, verify the selected package, run
packaged executable smoke only after clearance or owner exception, run Live E2E
only after separate per-run authorization, complete security review, record
go/no-go, and publish only after separate publication authorization.

### Explicit non-actions

This documentation hardening did not execute Live E2E, mutate Google Docs or
Google Drive, create or update packages, run flagged artifacts, create tags,
publish artifacts, change Frozen specifications, change public APIs, or change
production design.

## Publisher Failure Report Diagnostic Summary

Status: DONE as documentation-only diagnostic status record.

Added `docs/development/Publisher_FailureReport_DiagnosticSummary.md` to record
the current failure-report interpretation while Avast false-positive handling
remains pending. The diagnostic summary preserves the formal state:

`Phase 4 local-only verification complete / release blocked`.

The current stop is intentional and operational. It is not recorded as a
product regression. Diagnostic Logging and Error Handling remain done as
local-only Phase 4 implementation work, and neither changes the release gate.

### Decision

Current decision: Hold. Await Avast response.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update packages
or distribution artifacts, re-run flagged executables, push commits, change
Frozen specifications, change public APIs, change production code, or change
production design.

## Publisher Operator Guidance For Avast Hold

Status: DONE as documentation-only operator guidance.

Added `docs/development/Publisher_OperatorGuidance_AvastHold.md` to record the
local-only operator boundary while Avast false-positive handling remains
pending. The guidance preserves the formal state:

`Phase 4 local-only verification complete / release blocked`.

Allowed work is limited to build, unit tests, mock-backed verification, dry-run
verification that does not cross the release boundary, documentation updates,
and static existing-package inspection only. Blocked work remains Live E2E,
Google Docs or Google Drive mutation, package or distribution artifact creation
or update, release, tag, publication, flagged executable re-run, and push.

### Decision

Do not proceed to the release path before the Avast response is received and
recorded. Treat the current hold as operational, not a product regression.
Resume record synchronization in this order: Runbook, TestClassification,
CURRENT_STATUS, Voyage Log.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update packages
or distribution artifacts, re-run flagged executables, push commits, change
Frozen specifications, change public APIs, change production code, or change
production design.

## Publisher Evidence Bundle Specification

Status: DONE as documentation-only evidence bundle design.

Added `docs/development/Publisher_EvidenceBundleSpecification.md` to define the
intended structure of a redacted Publisher evidence bundle for release review,
security review, Avast false-positive appeal, internal audit, and regression
investigation.

The specification records bundle sections for build evidence, unit test
evidence, integration/mock evidence, diagnostic log samples, error handling,
retry policy, release runbook references, hold/resume conditions, prohibited
contents, naming and folder conventions, redaction review, verification, and
future automation candidates.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Explicit non-actions

This documentation update did not assemble a concrete evidence bundle, release,
create tags, publish artifacts, execute Live E2E, mutate Google Docs or Google
Drive, create or update packages or distribution artifacts, re-run flagged
executables, submit files to vendors, push commits, change Frozen
specifications, change public APIs, change production code, or change
production design.

## Publisher Preflight Hardening

Status: DONE as documentation-only / local-only release-boundary hardening.

Added `docs/development/Publisher_PreflightHardening.md` to consolidate the
Avast-pending allowed work, blocked work, preflight hard stops, resume
conditions, and reporting requirements that prevent local-only evidence from
being mistaken for release readiness.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Decision

Do not proceed from local-only verification into release, package, Live E2E,
Google Docs or Google Drive mutation, publication, or flagged executable
execution before the Avast response is recorded against the exact selected
artifact identity and the repository owner explicitly reopens the required
gate. Package work, packaged executable smoke, Live E2E, and publication remain
separate authorization gates.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update packages
or distribution artifacts, write to `dist`, re-run flagged executables, push
commits, change Frozen specifications, change public APIs, change production
code, or change production design.

## Publisher Release Approval Package

Status: DONE as documentation-only / local-only approval package organization.

Added `docs/development/Publisher_ReleaseApprovalPackage.md` to summarize the
current approval state, evidence index, ahead commits, blocked operations,
resume conditions, and approval recommendation.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Decision

Approval Recommendation = Hold.

Avast false-positive handling remains pending, vendor clearance has not been
obtained, Live E2E has not been authorized or executed for this approval
package, and repository-owner go/no-go approval has not been recorded.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update packages
or distribution artifacts, write to `dist`, re-run flagged executables, push
commits, change Frozen specifications, change public APIs, change tests,
change production code, or change production design.

## Publisher vNext Backlog

Status: DONE as documentation-only / local-only backlog record.

Added `docs/development/Publisher_vNext_Backlog.md` to record Publisher vNext
resume-gate, release-safety, hardening, and enhancement candidates while the
release remains blocked.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Decision

Avast false-positive handling remains pending. The backlog records candidate
next work only and does not change the release state.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update packages
or distribution artifacts, write to `dist`, re-run flagged executables, push
commits, change Frozen specifications, change public APIs, change tests,
change production code, or change production design.

## Publisher Avast Response Intake Template

Status: DONE as documentation-only / local-only intake template.

Added `docs/development/Publisher_AvastResponseIntakeTemplate.md` to define a
safe record structure for a future Avast false-positive response. The template
includes intake metadata, response classification, required evidence,
redaction rules, release gate decision options, resume conditions, operator
notes, and a decision log.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Decision

No Avast response has been received or recorded by this documentation update.
Avast false-positive handling remains pending. The release gate remains
blocked.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update packages
or distribution artifacts, write to `dist`, re-run flagged executables, push
commits, change Frozen specifications, change public APIs, change tests,
change production code, or change production design.

## Phase 3-2D: Update Execution Transaction and Recovery Decisions

### Scope

Phase 3-2D adds the productization execution boundary for applying an existing
`PhysicalUpdatePlan` through Google Docs `batchUpdate`. It does not recalculate
diffs, alter plan revisions, mutate the plan, update Verified State inside the
executor, or change Frozen specifications.

### Execution boundary

`PhysicalUpdateExecutor` validates the supplied plan, returns `NoChange` before
mapping or API calls, maps operation plans exactly once, and reuses the same
`PhysicalUpdateRequestBatch` for all retries. It classifies terminal outcomes
as `Applied`, `NoChange`, `RevisionConflict`, `Rejected`, `TransientFailure`,
or `IndeterminateFailure`. Result diagnostics include document ID, required
revision, applied revision when known, submitted operation and request counts,
attempt count, diagnostic code/message, and request traces.

Retry is limited to failures that are both retryable and definitely `NotSent`.
`Sent` or `Unknown` failures are treated as indeterminate and are not resent by
the executor. Retry delay uses the configured policy, honors `Retry-After`,
applies exponential backoff and max-delay clamping, and is mediated through
`IAsyncDelay` so tests do not wait in real time.

Cancellation is classified by location. Pre-send cancellation returns a safe
not-sent result, retry-delay cancellation returns a safe not-sent result, and
send-time cancellation is indeterminate. API success remains `Applied`.

### Request mapping and Google client

`GoogleDocsPhysicalUpdateRequestMapper` is a pure mapper from
`PhysicalUpdatePlan` to `PhysicalUpdateRequestBatch`. It preserves the planner's
operation order, emits delete ranges before subsequent inserts when that is the
plan order, expands insert/update/move/move-and-update payloads from the
candidate canonical block only, and creates one trace for each Google Docs
request. Trace records retain request index, source physical operation
sequence, operation reason, block identity, and request kind.

`GoogleDocsBatchUpdateClient` serializes the mapped requests with
`writeControl.requiredRevisionId` and normalizes Google/transport failures into
`GoogleDocsBatchUpdateException` with HTTP status, Google reason, Retry-After,
delivery state, and inner exception where available.

### Recovery and Verified State

`PhysicalUpdateApplicationService` sits above the executor. On `Applied` or
`NoChange`, it rereads the document, verifies the readback against the
Candidate, passes evidence through the existing publish-result verifier and
promoter, and saves Verified State only at the end. On `IndeterminateFailure`,
it rereads and invokes `PhysicalUpdateRecoveryReconciler`.

Recovery compares canonical document evidence, not revision alone:

* Candidate identity, fingerprint, block order, Explicit ID, Generated ID, and
  Content Hash match -> `Applied`
* Baseline identity, fingerprint, block order, Explicit ID, Generated ID, and
  Content Hash match -> `NotApplied`
* neither full comparison matches -> `Diverged`

`NotApplied` requests upper-layer replanning; the old plan is not resent by the
application service. `Diverged` stops without saving state.

### Automated evidence

| Check | Result |
|---|---|
| Phase 3-2D focused Unit Tests | PASS - 21/21 |
| Publisher Unit Tests | PASS - 393/393 |
| Publisher Integration Tests | PASS - 10/10 |
| Release solution build | PASS - 0 warnings, 0 errors |
| `dotnet format --verify-no-changes` | PASS |
| `git diff --check` | PASS - no whitespace errors; CRLF normalization warnings only |
| Frozen specification changes | None |

## Phase 3-2C: Physical Update Planning and Verification Decisions

Architecture decision reference: `docs/architecture/ADR-0004-verified-state-and-differential-update-safety.md`
records the durable safety decision for Verified State as the trusted
differential-update baseline, revision-conflict abort behavior, safe physical
update ordering, mandatory Readback Verification, and post-verification-only
Verified State persistence. The ADR is documentation-only and does not replace
this Phase 3-2C implementation record.

| Field | Value |
|---|---|
| Date | 2026-07-22 |
| Scope | Physical block planning, explicit managed snapshots, optimistic concurrency, dry-run, readback verification, and lifecycle connection |
| Frozen specification changes | None |
| Live Google Docs updates | Disabled by default; no credentialed live test |
| CLI, complete Managed Region design, Snapshot, Recovery, Archived restore, Schema migration | Out of scope |

### Logical and physical plan separation

`DiffPlan` remains a target-neutral statement of block identity changes.
`PhysicalUpdatePlanner` converts it into a separate `PhysicalUpdatePlan` only
after receiving a validated `ManagedDocumentSnapshot`. Every physical operation
retains its prior and candidate block indexes, logical reason, affected range,
and traced `BlockIdentity`. A Candidate built by the canonical pipeline also
retains its `DocumentModel`, because identities and hashes alone cannot supply
the payload required for a physical insertion.

NoChange generates no physical operation. Insert generates one InsertBlock.
Delete generates one DeleteRange. Update generates DeleteRange followed by an
InsertBlock carrying the Candidate payload. Move is not treated as a Google
Docs primitive: it generates DeleteRange followed by InsertBlock. A block that
is both moved and updated generates exactly one source deletion and one
Candidate-payload insertion, both marked `MoveAndUpdate`.

### Index-safe execution order

All destructive operations are emitted first in descending current document
start index. Their ranges therefore remain valid while later document content
is deleted. Overlapping, repeated, missing, or out-of-region source ranges stop
planning with a stable error.

After all destructive operations, ranges for surviving NoChange blocks are
rebased by the exact lengths of preceding deletions. Constructive operations
are emitted in descending Candidate index. Each insertion uses the first
surviving block to its right as an anchor, or the reduced managed-region end
when no such survivor exists. Multiple blocks in one gap are consequently
inserted in reverse order at the same anchor and finish in Candidate order.
This avoids guessing the post-insert length of tables or images and keeps the
plan deterministic.

### Managed document snapshot contract

Every planning and application operation requires:

- exact `DocumentIdentity`;
- `DocumentRevision` with provider ID and adapter-supplied monotonic sequence;
- managed-region start and exclusive end UTF-16 indexes;
- ordered managed blocks;
- Explicit ID, Generated ID, and Content Hash for every block;
- inclusive-start/exclusive-end block range;
- reconstructed publish fingerprint.

Block ranges must be non-empty, ordered, non-overlapping, and fully inside the
managed region. The current snapshot must match the Verified Baseline's
revision, fingerprint, identity, block identities, hashes, and order. Phase
3-2C defines this boundary contract but does not define marker discovery or the
complete Managed Region storage design.

### Revision and optimistic concurrency

Four revisions are kept distinct:

1. the revision persisted with the Verified Baseline;
2. the snapshot revision used for planning;
3. a second snapshot read immediately before apply;
4. the apply receipt and post-apply readback revision.

The Baseline and planning revisions must match exactly. The pre-apply revision
and complete topology must still equal the prepared snapshot. The adapter
receives the prepared revision as an apply precondition. A mutating apply must
return a strictly greater monotonic sequence. Readback must equal the apply
receipt and remain greater than the planning revision. NoChange does not apply
and may retain the same revision. Missing, changed, unchanged-after-mutation,
or regressed revisions stop with `UPDATE_REVISION_CONFLICT`.

Persisting the successful readback revision changes the Publish State canonical
shape. `PublishStateSchema.CurrentVersion` is therefore `2`. Schema v2 adds
`revisionId` and `revisionSequence`. Schema v1 is not migrated in this phase and
fails with `STATE_SCHEMA_VERSION_UNSUPPORTED`.

### Apply, readback, and lifecycle

`IManagedDocumentAdapter` exposes snapshot read and revision-bound physical
apply operations without Google SDK types in Application. The physical
application verifier creates the plan, repeats the pre-apply read, invokes the
adapter, reads back, and requires exact Candidate identity, fingerprint, block
count, order, Explicit ID, Generated ID, and Content Hash. It also validates
the returned managed boundary and all block ranges.

`VerifiedPublishLifecycle` now performs Baseline load, current snapshot read,
DiffPlan generation, physical planning/application, readback verification,
promotion, atomic save, and success in that order. Any physical planning,
application, revision, readback, promotion, or save error occurs before a new
state becomes durable.

### Dry-run

`DryRunAsync` uses the same Baseline load, snapshot preparation, DiffEngine, and
PhysicalUpdatePlanner as a real update. It returns logical and physical counts,
per-kind logical counts, publish-required status, revision precondition,
operation identities, affected ranges, warnings, and stable conflicts. It does
not call adapter apply and does not call State save. A conflicting dry-run
retains the logical publish-required result while omitting an unsafe physical
plan.

If identity, revision, or managed-boundary preparation fails before DiffPlan
generation, dry-run returns the stable conflict code with no logical or
physical plan; counts are zero because unsafe input is not planned.

### Infrastructure adapters and live-update boundary

`InMemoryManagedDocumentAdapter` implements deterministic revision-bound apply
and readback for Unit and Integration tests. It can inject pre-apply edits,
post-apply edits, unchanged or regressed revisions, and application failures.

`GoogleDocsPhysicalUpdateAdapter` implements the Application adapter boundary
over `IGoogleDocsManagedDocumentGateway`. Snapshot reads are available, but
live apply is rejected by default. The host must explicitly construct it with
`liveUpdatesEnabled: true`. The gateway implementation that discovers managed
markers, reconstructs canonical block metadata, and maps block payloads to
credentialed Google Docs requests is deferred with the complete Managed Region
design; it is not silently approximated in Phase 3-2C.

Stable physical-update codes are `UPDATE_REVISION_CONFLICT`,
`UPDATE_MANAGED_REGION_MISMATCH`, `UPDATE_PHYSICAL_PLAN_INVALID`,
`UPDATE_APPLICATION_FAILED`, `UPDATE_READBACK_FAILED`, and
`UPDATE_READBACK_MISMATCH`.

No CLI command, live-write default, complete Managed Region marker design,
Snapshot, Recovery, Archived restoration, or schema migration was introduced.

### Automated evidence

| Check | Result |
|---|---|
| Phase 3-2C focused Unit Tests | PASS - 52/52 |
| Publisher Unit Tests | PASS - 372/372 |
| Publisher Integration Tests | PASS - 10/10 |
| Release solution build | PASS - 0 warnings, 0 errors |
| `dotnet format --verify-no-changes` | PASS |
| `git diff --check` | PASS |
| Frozen specification changes | None |

## ADR Operating Basis

Status: DONE as documentation-only / local-only architecture decision record
process.

Added `docs/architecture/ADR_INDEX.md`,
`docs/architecture/adr-template.md`, and
`docs/architecture/ADR-0001-architecture-decision-record-process.md` to define
the VMF repository ADR operating basis.

ADR numbering starts at `ADR-0001` and continues as a zero-padded sequence.
ADR statuses are limited to Proposed, Accepted, Superseded, and Deprecated.
The ADR index tracks number, title, current status, successor ADR, and related
documents. Accepted ADR body content is stable by default; meaningful changes
are recorded by a later ADR that supersedes the earlier ADR.

### Decision

ADRs record durable architecture decision rationale only. They do not replace
Canon v2.0, VMF v1.0 Frozen Specification, Publisher implementation
specifications, public API contracts, persisted schemas, canonical formats,
release checklists, runbooks, release notes, verification evidence, or current
status records.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update packages
or distribution artifacts, write to `dist`, re-run flagged executables, push
commits, change Frozen specifications, change public APIs, change tests,
change production code, or change production design.

## ADR-0002 OAuth Desktop Authentication

Status: DONE as documentation-only / local-only authentication decision
record.

Added `docs/architecture/ADR-0002-oauth-2-0-desktop-authentication.md` and
updated `docs/architecture/ADR_INDEX.md` to record OAuth 2.0 Desktop as the
Publisher Google API authentication decision for personal Gmail and local
operator workflows.

The ADR records that Service Account authentication remains available for
automation and Shared Drive workflows where the target location is explicitly
accessible to the service identity. It records token-store persistence as local
sensitive state that must remain outside the repository, packages, logs, and
evidence records. It also records the current Documents and Drive scopes and
defers Google Picker plus `drive.file` least-privilege routing to vNext
reconsideration.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, mutate token stores,
create or update packages or distribution artifacts, write to `dist`, re-run
flagged executables, push commits, change Frozen specifications, change public
APIs, change tests, change production code, or change production design.

## ADR-0003 Release Gate and Vendor Clearance

Status: DONE as documentation-only / local-only release governance decision
record.

Added `docs/architecture/ADR-0003-release-gate-and-vendor-clearance.md` and
updated `docs/architecture/ADR_INDEX.md` to record Publisher release gate and
vendor clearance as long-term release governance boundaries.

The ADR records required verification success, vendor clearance, Avast
false-positive review resolution or formal repository-owner risk acceptance,
explicit release authorization, and final release verification success as
required release conditions. Until all gates are satisfied, release
publication, production release tag creation, production package publication,
and unauthorized Live Google Docs / Drive mutation remain prohibited.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Decision

Runbooks remain operational procedure. ADR-0003 records the durable governance
decision only and does not replace the runbook, approve release execution,
obtain vendor clearance, resolve Avast false-positive handling, or accept
antivirus risk.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, mutate token stores,
create or update packages or distribution artifacts, write to `dist`, re-run
flagged executables, push commits, change Frozen specifications, change public
APIs, change tests, change production code, or change production design. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, and release remains blocked.

## ADR-0005 Retry Policy and Failure Classification

Status: DONE as documentation-only / local-only retry decision record.

Added
`docs/architecture/ADR-0005-retry-policy-and-failure-classification.md` and
updated `docs/architecture/ADR_INDEX.md` to record Publisher failure-time retry
judgment as an accepted Architecture Decision.

ADR-0005 records that automatic retry is limited to retryable,
definitely-not-sent, idempotent operations under bounded backoff. It preserves
the existing Phase 4-2-2 exit-code relationship and Phase 4-2-3 retry policy:
exhausted safe transient retry maps to `Transient` / exit code `75`,
verification and revision-conflict failures map to verification handling,
configuration errors are not retried, unknown or blank stable codes remain
`Internal` fallback cases, and `OperationCanceled` is not retried.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Decision

ADR-0004 remains the update-safety ADR: Verified State, revision conflict hard
stops, physical update ordering, readback verification, and state promotion.
ADR-0005 records the failure-time retry decision: retry eligibility, transient
classification, exit-code relationship, idempotency requirement, bounded
backoff, and safe message policy.

Safe user-facing and evidence messages must not expose raw exception text,
stack traces, raw HTTP bodies, tokens, credentials, private URLs, temporary
public URLs, local paths, or implementation-specific exception details.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, mutate token stores,
create or update packages or distribution artifacts, write to `dist`, re-run
flagged executables, push commits, change Frozen specifications, change public
APIs, change tests, change production code, or change production design. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, and release remains blocked.

## ADR-0006 Diagnostic Logging and Safe Observability

Status: DONE as documentation-only / local-only observability decision record.

Added
`docs/architecture/ADR-0006-diagnostic-logging-and-safe-observability.md` and
updated `docs/architecture/ADR_INDEX.md` to record Publisher diagnostic
logging and safe observability as an accepted Architecture Decision.

ADR-0006 records structured JSON diagnostic logs as the standard, stdout as
the user-facing result stream, and stderr as the structured diagnostic log
stream. It records `sessionId`, stable event `code`, `level`, and
`timestampUtc` as basic fields, with command, phase, operation, and safe
message context governed by the Phase 4-2-1 Diagnostic Logging Specification.
It also records session, command, phase, operation, summary, and warning
lifecycle events.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Decision

Safe messages and redaction policy are required before diagnostic events are
serialized. Logs must not expose raw exception messages, stack traces, raw
HTTP bodies, OAuth tokens, credentials, Authorization headers, private URLs,
temporary public URLs, local paths, or secrets.

Plain text only logging, raw exception logging, and unbounded verbose logging
are not adopted. External log collection infrastructure, OpenTelemetry,
distributed tracing, and monitoring service integration are out of scope.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, mutate token stores,
create or update packages or distribution artifacts, write to `dist`, re-run
flagged executables, push commits, change Frozen specifications, change public
APIs, change tests, change production code, or change production design. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, and release remains blocked.

## ADR-0007 Error Handling and Failure Classification

Status: DONE as documentation-only / local-only error handling decision
record.

Added
`docs/architecture/ADR-0007-error-handling-and-failure-classification.md` and
updated `docs/architecture/ADR_INDEX.md` to record Publisher CLI error
handling and failure classification as an accepted Architecture Decision.

ADR-0007 records the existing Phase 4-2-2 error handling behavior:
verification failures return exit code `4`, transient failures return exit
code `75`, cancellation returns exit code `130`, and unknown, blank, or
missing stable error codes fall back to `Internal`.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Decision

Publisher CLI owns final failure classification, exit-code conversion, and
fixed safe summary messages. Stable error codes remain separate from safe
user-facing messages. Raw exception messages, stack traces, raw HTTP bodies,
provider payloads, private URLs, local paths, tokens, credentials, and
implementation-specific exception details must not be emitted to user-facing
output or structured diagnostics.

ADR-0007 keeps ADR-0005 focused on retry policy and ADR-0006 focused on
diagnostic logging. `OperationCanceledException` must be rethrown through
lower layers and handled only at the CLI boundary or an owning test harness
boundary.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, mutate token stores,
create or update packages or distribution artifacts, write to `dist`, re-run
flagged executables, push commits, change Frozen specifications, change public
APIs, change tests, change production code, or change production design. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, and release remains blocked.

## ADR-0008 Preflight Hard Stop and Release Boundary

Status: DONE as documentation-only / local-only operational gate decision
record.

Added
`docs/architecture/ADR-0008-preflight-hard-stop-and-release-boundary.md` and
updated `docs/architecture/ADR_INDEX.md` to record Publisher preflight hard
stop and release boundary enforcement as an accepted Architecture Decision.

ADR-0008 records the current formal state:

`Phase 4 local-only verification complete / release blocked`.

Avast false-positive handling remains pending, and vendor clearance has not
been obtained. While that remains true, the preflight hard stop prohibits Live
E2E, Google Docs mutation, Google Drive mutation, package or distribution
artifact update, release, tag creation, publication, and flagged executable
re-run.

### Decision

ADR-0008 fixes the operational boundary that prevents local-only evidence from
being promoted into release authorization. Allowed work during the hold remains
limited to documentation, build, unit tests, mock-backed verification,
dry-run verification that does not publish or execute the flagged package
executable, static inspection, and non-changing confirmation of package
contents when explicitly in scope.

ADR-0003 remains the release gate and vendor-clearance governance basis.
ADR-0008 records the preflight hard stop that enforces that basis before
release-path work begins. ADR-0005 remains responsible for retry and failure
classification, ADR-0006 for diagnostic logging and safe observability, and
ADR-0007 for CLI error handling and stable failure surface.

Release-path work may resume only after Avast false-positive handling is
complete, vendor clearance or another ADR-0003 owner-decision path is
explicitly recorded, the runbook resume gate is rechecked, required
re-verification is completed, and each later operation-specific gate is
separately authorized.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, mutate token stores,
create or update packages or distribution artifacts, write to `dist`, re-run
flagged executables, push commits, change Frozen specifications, change public
APIs, change tests, change production code, or change production design. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, and release remains blocked.

## ADR-0009 Evidence Bundle and Release Approval Package Boundary

Status: DONE as documentation-only / local-only evidence and approval-package
boundary decision record.

Added
`docs/architecture/ADR-0009-evidence-bundle-and-release-approval-package-boundary.md`
and updated `docs/architecture/ADR_INDEX.md` to record Publisher evidence
bundle and release approval package boundary control as an accepted
Architecture Decision.

ADR-0009 records the evidence bundle as a design, collection, validation, and
redaction boundary. It is not a release artifact, publication artifact,
package artifact, distribution artifact, release authorization, vendor
clearance, or Avast false-positive resolution.

The release approval package remains a review record. The current package
records `Approval Recommendation = Hold`. If no Avast response has been
received and recorded, the default decision is `Hold continues`.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Decision

Evidence bundle creation, evidence validation, evidence redaction, release
approval package preparation, or approval package review does not authorize
package or distribution artifact creation or update, release, tag creation,
publication, Live E2E, Google Docs mutation, Google Drive mutation,
token-store mutation, or flagged executable re-run.

ADR-0009 keeps ADR-0003 as the release gate and vendor-clearance governance
basis and ADR-0008 as the operational preflight hard stop. ADR-0005 remains
responsible for retry policy, ADR-0006 for diagnostic logging and safe
observability, and ADR-0007 for CLI error handling and stable failure surface.

Vendor clearance has not been obtained. That means no approval, no release
authorization, and no permission to publish.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, mutate token stores,
create or update packages or distribution artifacts, write to `dist`, re-run
flagged executables, push commits, change Frozen specifications, change public
APIs, change tests, change production code, or change production design. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, and release remains blocked.

## ADR-0010 vNext Backlog and Deferred Scope Boundary

Status: DONE as documentation-only / local-only backlog-boundary decision
record.

Added
`docs/architecture/ADR-0010-vnext-backlog-and-deferred-scope-boundary.md` and
updated `docs/architecture/ADR_INDEX.md` to record Publisher vNext backlog
classification and deferred scope as an accepted Architecture Decision.

ADR-0010 records that `docs/development/Publisher_vNext_Backlog.md` is a
planning, triage, sequencing, and traceability record only. Its P0, P1, P2,
Blocked, and Deferred classifications are not implementation approval, vNext
feature adoption, v1.0 release authorization, vendor clearance, Avast
false-positive resolution, Live E2E authorization, Google Docs / Drive
mutation approval, package or distribution artifact approval, tag approval, or
publication approval.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

### Decision

vNext candidates are not v1.0 release authorization. Google Picker plus
`drive.file` least-privilege routing remains a vNext reconsideration
candidate, not an adopted design decision for the current v1.0 release
boundary.

Backlog organization is allowed during the current hold only as docs-only /
local-only work. It may clarify future sequence and deferred scope, but it
must preserve Frozen Specifications, public APIs, production code, tests,
package artifacts, distribution artifacts, release records, and the
release-blocked state.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, mutate token stores,
create or update packages or distribution artifacts, write to `dist`, re-run
flagged executables, push commits, change Frozen specifications, change public
APIs, change tests, change production code, or change production design. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, and release remains blocked.

## ADR-0011 Release Authorization Record and Explicit Approval Boundary

Status: DONE as documentation-only / local-only release-authorization-boundary
decision record.

Added
`docs/architecture/ADR-0011-release-authorization-record-and-explicit-approval-boundary.md`
and updated `docs/architecture/ADR_INDEX.md` to record that release
authorization must be a separate release-governance record, not an ADR.

ADR-0011 records that Accepted ADRs document architectural and operational
decisions only. Accepted ADRs do not imply release approval, production
readiness, vendor clearance, or authorization to publish, tag, package,
distribute, re-run flagged executables, perform Live E2E, or mutate live
Google Docs / Drive resources.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

The Release Approval Package remains evidence for review, not approval itself.
The current recommendation remains `Approval Recommendation = Hold`.

### Decision

A release authorization record is a separate release-governance artifact. At
minimum, it must include vendor clearance, Avast response or explicit risk
acceptance, final verification result, explicit approver identity, approval
timestamp, approved release scope, release target/version, and confirmation
that named blocked operations are permitted.

A `Hold` recommendation cannot authorize release, package publication,
tagging, Live E2E, Google Docs / Drive mutation, distribution, or flagged
executable re-run.

If vendor clearance or an Avast response arrives later, release remains
blocked until a separate explicit release authorization record is created and
approved.

ADR-0011 keeps ADR-0003 as the release gate and vendor-clearance governance
basis, ADR-0008 as the operational preflight hard stop, and ADR-0009 as the
evidence and Release Approval Package review boundary.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, mutate token stores,
create or update packages or distribution artifacts, write to `dist`, re-run
flagged executables, push commits, change Frozen specifications, change public
APIs, change tests, change production code, or change production design. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, the current Release Approval Package recommendation remains Hold,
and release remains blocked.

## ADR-0012 Release Resumption Procedure and Final Verification Order

Status: DONE as documentation-only / local-only release-resumption-order
decision record.

Added
`docs/architecture/ADR-0012-release-resumption-procedure-and-final-verification-order.md`
and updated `docs/architecture/ADR_INDEX.md` to record the final release
resumption order as an accepted Architecture Decision.

ADR-0012 applies only after vendor clearance is obtained and Avast response /
false-positive disposition is received and reviewed. It records that vendor
clearance alone is not release authorization, Avast response alone is not
release authorization, and the Release Approval Package is not approval by
itself.

The required order is intake and preserve vendor / Avast response evidence,
validate evidence authenticity and scope, confirm no remaining release
blockers, re-run approved local verification only, review Evidence Bundle
completeness and redaction, prepare the Release Approval Package, record an
explicit release authorization decision, permit final release verification only
after authorization, and permit package/dist update, tag, publication, and
release only after final verification succeeds.

ADR-0012 keeps ADR-0003 as the release gate and vendor-clearance governance
basis, ADR-0008 as the operational preflight hard stop, ADR-0009 as the
evidence and Release Approval Package review boundary, and ADR-0011 as the
explicit release authorization boundary.

The recommendation remains `Approval Recommendation = Hold` until explicit
release authorization is recorded. Any ambiguity, mismatch, missing evidence,
remaining blocker, incomplete redaction, missing approval decision, or failed
final verification returns the state to Hold.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update package
or distribution artifacts, re-run flagged executables, change production code,
change tests, modify Frozen specifications, or change public APIs.

## ADR-0015 Release Withdrawal / Rollback Record and Incident Evidence Boundary

Status: DONE as documentation-only / local-only withdrawal, rollback, and
incident-evidence boundary decision record.

Added
`docs/architecture/ADR-0015-release-withdrawal-rollback-record-and-incident-evidence-boundary.md`
and updated `docs/architecture/ADR_INDEX.md` to record the Release Withdrawal
Record, Rollback Record, and Incident Evidence Bundle boundary as an accepted
Architecture Decision.

ADR-0015 defines the boundary between Release Withdrawal Record, Rollback
Record, Incident Evidence Bundle, Release Evidence Bundle, and Release
Approval / Authorization. It records that withdrawal or rollback records are
not release approval, release authorization, vendor clearance, Avast
false-positive resolution, risk acceptance for a future release, or permission
to republish.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

The recommendation remains `Approval Recommendation = Hold`. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, no release authorization has been granted, no Release Decision
Record has been created, no publication has occurred, no Publication Record
has been created, no Post-Release Evidence has been created, no Withdrawal
Record has been created, no Rollback Record has been created, and no Incident
Evidence Bundle has been created.

### Decision

A Withdrawal or Rollback Record must include, at minimum: trigger, affected
artifact or publication target, detection timestamp, containment action,
rollback or withdrawal action, verification performed, evidence references,
residual risk, follow-up owner, and final status.

Incident evidence must follow safe evidence rules. It must not include
credentials, tokens, private URLs, raw local paths, unredacted logs, or
sensitive Google Docs / Drive identifiers unless explicitly redacted or
approved.

Any re-release, re-publication, package replacement, tag replacement, or
publication restoration after withdrawal or rollback must re-enter the release
gate and verification order defined by ADR-0003, ADR-0008, ADR-0009,
ADR-0012, ADR-0013, and any applicable release-authorization prerequisite.

ADR-0015 keeps ADR-0003 as the release gate and vendor-clearance governance
basis, ADR-0008 as the operational preflight hard stop, ADR-0009 as the
Evidence Bundle and Release Approval Package boundary, ADR-0012 as the release
resumption and final verification order, ADR-0013 as the Release Decision
Record and post-authorization traceability boundary, and ADR-0014 as the
Release Publication Record and Post-Release Evidence boundary.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
republish, execute Live E2E, mutate Google Docs or Google Drive, create or
update package or distribution artifacts, re-run flagged executables, change
production code, change tests, modify Frozen specifications, or change public
APIs.

## ADR-0014 Release Publication Record and Post-Release Evidence Boundary

Status: DONE as documentation-only / local-only publication-record and
post-release-evidence boundary decision record.

Added
`docs/architecture/ADR-0014-release-publication-record-and-post-release-evidence-boundary.md`
and updated `docs/architecture/ADR_INDEX.md` to record the Release Publication
Record and Post-Release Evidence boundary as an accepted Architecture
Decision.

ADR-0014 applies only after actual publication has occurred. It records that a
future Release Publication Record must identify the facts actually published,
including publication date/time, operator, version, commit / tag / release
identifier, package or distribution artifact identity, publication
destination, publication command or workflow reference, linked Release
Decision Record or authorization reference, and post-publication verification
or observation references.

Post-Release Evidence is evidence collected after publication. It may record
observations, confirmations, or audit evidence, but it must not retroactively
satisfy or repair pre-release approval, release authorization, required
release gates, vendor clearance, Avast false-positive resolution, final
release verification required before publication, or Release Decision Record
completeness.

ADR-0014 keeps ADR-0003 as the release gate and vendor-clearance governance
basis, ADR-0009 as the Evidence Bundle and Release Approval Package review
boundary, ADR-0012 as the release resumption and final verification order, and
ADR-0013 as the Release Decision Record and post-authorization traceability
boundary.

The recommendation remains `Approval Recommendation = Hold`. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, no release authorization has been granted, no Release Decision
Record has been created, no publication has occurred, no Publication Record
has been created, and no Post-Release Evidence has been created.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update package
or distribution artifacts, re-run flagged executables, change production code,
change tests, modify Frozen specifications, or change public APIs.

## ADR-0013 Release Decision Record and Post-Authorization Traceability

Status: DONE as documentation-only / local-only post-authorization
traceability decision record.

Added
`docs/architecture/ADR-0013-release-decision-record-and-post-authorization-traceability.md`
and updated `docs/architecture/ADR_INDEX.md` to record the Release Decision
Record and post-authorization traceability boundary as an accepted
Architecture Decision.

ADR-0013 applies only after release authorization is granted. It records that
a future Release Decision Record must link the decision date/time, decision
owner / authorizer, authorized release scope, evidence bundle reference, final
verification reference, vendor clearance / Avast resolution reference,
explicit authorization outcome, any accepted residual risk, and the next
allowed operation boundary.

The Release Decision Record is not itself a release artifact, package,
publication, tag, deployment, or publication record. It must not be backdated
or used to imply authorization before ADR-0003, ADR-0009, ADR-0012, and any
applicable release-authorization prerequisites are satisfied.

ADR-0013 keeps ADR-0003 as the release gate and vendor-clearance governance
basis, ADR-0009 as the evidence and Release Approval Package review boundary,
and ADR-0012 as the release resumption and final verification order.

The recommendation remains `Approval Recommendation = Hold`. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, no release authorization has been granted, no Release Decision
Record has been created, and no publication record has been created.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update package
or distribution artifacts, re-run flagged executables, change production code,
change tests, modify Frozen specifications, or change public APIs.

## ADR-0017 Release Retention / Archival / Audit Trail

Status: DONE as documentation-only / local-only retention, archival, and
audit-trail boundary decision record.

Added
`docs/architecture/ADR-0017-release-retention-archival-audit-trail.md`
and updated `docs/architecture/ADR_INDEX.md` to record release retention,
archival, and audit trail responsibilities as an accepted Architecture
Decision.

ADR-0017 records that finalized release evidence, approval packages, vendor
clearance responses, final verification records, release authorization
records, release decision records, publication records, post-release evidence,
withdrawal records, rollback records, and incident evidence bundles must be
retained as immutable audit evidence.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

The recommendation remains `Approval Recommendation = Hold`. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, no release authorization has been granted, no Release Decision
Record has been created, and no publication has occurred.

### Decision

Archival is documentation and evidence preservation only. It is not release
authorization, release approval, package approval, publication approval,
vendor clearance, Avast false-positive resolution, Live E2E authorization,
Google Docs / Drive mutation authorization, tag authorization, or production
readiness.

Archived evidence must preserve traceability from release decision to final
verification, vendor clearance or Avast disposition, Release Approval Package,
Evidence Bundle, and package/release identifiers when those records exist and
are authorized to be recorded.

Until vendor clearance is obtained and recorded, archival may record only the
current Hold state: release blocked, Avast pending, vendor clearance not
obtained, and `Approval Recommendation = Hold`.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update package
or distribution artifacts, write to `dist`, re-run flagged executables, change
production code, change tests, modify Frozen specifications, change public
APIs, obtain vendor clearance, resolve Avast false-positive handling, create
archive artifacts, stage changes, commit changes, or push commits.

## ADR-0018 Emergency Release Exception Boundary

Status: DONE as documentation-only / local-only emergency-exception-boundary
decision record.

Added
`docs/architecture/ADR-0018-emergency-release-exception-boundary.md`
and updated `docs/architecture/ADR_INDEX.md` to record the emergency release
exception boundary as an accepted Architecture Decision.

ADR-0018 records that an emergency release exception is not normal release gate
reopening. It does not clear Avast pending, does not obtain vendor clearance,
does not change `Approval Recommendation = Hold`, and does not convert a
blocked release into an approved release path.

The current formal state remains:

`Phase 4 local-only verification complete / release blocked`.

The recommendation remains `Approval Recommendation = Hold`. Avast
false-positive handling remains pending, vendor clearance has not been
obtained, no release authorization has been granted, no emergency exception
approval has been granted, no Release Decision Record has been created, and no
publication has occurred.

### Decision

Emergency release exception consideration requires explicit authority, exact
scope, risk acceptance naming unresolved release-gate conditions, evidence,
rollback or withdrawal planning, operator responsibility, post-incident
review, and traceability to a later ADR or release decision record.

Unless an emergency exception is explicitly approved and recorded with the
required authority, scope, risk acceptance, evidence, rollback plan, and
post-incident review requirement, release, tag creation, publication, package
creation or update, distribution artifact update, writing to `dist`, Live E2E,
Google Docs mutation, Google Drive mutation, token-store mutation, flagged
executable re-run, and flagged-executable-dependent vendor submission remain
prohibited.

An emergency exception is not a permanent precedent. After containment, it
must be followed by post-incident review and tracked by a later ADR or release
decision record.

### Explicit non-actions

This documentation update did not release, create tags, publish artifacts,
execute Live E2E, mutate Google Docs or Google Drive, create or update package
or distribution artifacts, write to `dist`, re-run flagged executables, change
production code, change tests, modify Frozen specifications, change public
APIs, obtain vendor clearance, resolve Avast false-positive handling, accept
risk, approve emergency exception use, reopen the normal release gate, stage
changes, commit changes, or push commits.
