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
