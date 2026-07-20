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
