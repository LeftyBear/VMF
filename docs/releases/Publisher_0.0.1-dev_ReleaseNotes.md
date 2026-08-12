# Publisher 0.0.1-dev Release Notes

Date: 2026-08-09

## Scope

These notes record the completed Publisher `0.0.1-dev` GitHub prerelease
identity:

- Version: `0.0.1-dev`
- Tag: `publisher-v0.0.1-dev`
- Runtime: `win-x64`
- Configuration: `Release`
- Package type: framework-dependent (`selfContained=false`)
- Package:
  `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip`
- GitHub Release:
  https://github.com/LeftyBear/VMF/releases/tag/publisher-v0.0.1-dev

The canonical current published identity is recorded by
`docs/development/Publisher_ReleaseIdentityReconciliationRecord_2026-08-12.md`.
Older `vmf-publisher-v0.0.1-dev` / 983404 byte / `73582c...` release-note
facts are retained only as historical / superseded / non-canonical identity
evidence.

## Identity Status

`0.0.1-dev` is adopted as the next release identity because the prior
`0.0.0-dev` identity is already used as a tag, GitHub Release, and published
asset identity. The existing `vmf-publisher-v0.0.0-dev` tag, GitHub Release,
and asset remain immutable historical records.

The canonical current target commit and artifact identity are fixed:

- package target / peeled commit:
  `382bd715d8307930d0aeb8bd48116dac3f57af5c`;
- reconciliation record commit:
  `3759e2c4fc0d6438729ab8dffc62bc8d62abf753`;
- annotated tag object:
  `04a101729dbab431f9e67e1b7e43e6b9a94dd6e0`.

The verified package artifact identity is fixed:

- asset name: `vmf-publisher-0.0.1-dev-win-x64.zip`;
- package path: `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip`;
- package size: 983422 bytes;
- package SHA-256:
  `0174810d21c6072b8206acf2fed90b72c2e6be499c65b231d7d72d71fd69cb76`.

Package generation and package verification are recorded as `PASS`.
Package manifest identity records `version=0.0.1-dev`,
`runtimeIdentifier=win-x64`, `configuration=Release`,
`selfContained=false`, and 14 manifest files. Secret/static package
inspection is recorded as `PASS`.

Final verification, Live E2E 4/4 `PASS`, result review, package generation,
package verification, tag push/readback, GitHub prerelease creation, asset
upload, and remote asset digest readback form the recorded identity chain for
this release. Remote asset digest and the local verified package SHA-256
matched:
`0174810d21c6072b8206acf2fed90b72c2e6be499c65b231d7d72d71fd69cb76`.

## Release Boundary

This release-note file is documentation only. It records the completed release
publication result. It does not authorize or perform:

- package generation;
- `dist` updates;
- tag creation, deletion, movement, retargeting, or replacement;
- GitHub Release creation, update, deletion, or replacement;
- asset replacement;
- new publication or release announcement;
- Live E2E;
- Google Docs or Google Drive mutation;
- production code, test, public API, schema, or Frozen specification changes;
- staging, commit, or push.

ADR-0016 traceability is preserved by keeping the next identity separate from
the historical `0.0.0-dev` release and by recording the fixed package target
commit, evidence docs commit, annotated tag object, verified package path,
asset name, size, SHA-256, package verification, final verification, Live E2E,
result review, tag push/readback, GitHub prerelease creation, asset upload, and
remote digest match.

The tag is `publisher-v0.0.1-dev`; its annotated tag object is
`04a101729dbab431f9e67e1b7e43e6b9a94dd6e0`, and its peeled/package target
commit is `382bd715d8307930d0aeb8bd48116dac3f57af5c`. The GitHub Release is
published as prerelease `true` at
https://github.com/LeftyBear/VMF/releases/tag/publisher-v0.0.1-dev.
This docs-only update is not new tag/release execution authorization, staging
authorization, commit authorization, or push authorization.

Avast vendor clearance is not recorded as obtained. The release proceeded on
the ADR-0019 VMF-side residual risk acceptance basis, not on Avast safety
certification or vendor clearance.
