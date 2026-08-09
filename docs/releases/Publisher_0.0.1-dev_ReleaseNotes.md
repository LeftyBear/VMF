# Publisher 0.0.1-dev Release Notes

Date: 2026-08-09

## Scope

These notes record the completed Publisher `0.0.1-dev` GitHub prerelease
identity:

- Version: `0.0.1-dev`
- Tag: `vmf-publisher-v0.0.1-dev`
- Runtime: `win-x64`
- Configuration: `Release`
- Package type: framework-dependent (`selfContained=false`)
- Package:
  `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip`
- GitHub Release:
  https://github.com/LeftyBear/VMF/releases/tag/vmf-publisher-v0.0.1-dev

The package was generated and verified for this identity. The annotated tag was
pushed, remote tag readback passed, the GitHub prerelease was created, and the
release asset was uploaded.

## Identity Status

`0.0.1-dev` is adopted as the next release identity because the prior
`0.0.0-dev` identity is already used as a tag, GitHub Release, and published
asset identity. The existing `vmf-publisher-v0.0.0-dev` tag, GitHub Release,
and asset remain immutable historical records.

The target commit is fixed:

- package target / peeled commit:
  `f08eef306ba82e3ea7f031ef652666178f2f0acf`;
- evidence docs commit:
  `39df8bedd848da42a4de3cb9461ce4cc86b51197`;
- annotated tag object:
  `a962e19ba2b0a494d1158011ae823d579e41711f`.

The verified package artifact identity is fixed:

- asset name: `vmf-publisher-0.0.1-dev-win-x64.zip`;
- package path: `dist\release\Publisher\vmf-publisher-0.0.1-dev-win-x64.zip`;
- package size: 983404 bytes;
- package SHA-256:
  `73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6`.

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
`73582c24e4c3bf279aeb8fd2044b84a30a3d621eac623188dcfa4406ac32bcc6`.

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

The tag is `vmf-publisher-v0.0.1-dev`; its annotated tag object is
`a962e19ba2b0a494d1158011ae823d579e41711f`, and its peeled/package target
commit is `f08eef306ba82e3ea7f031ef652666178f2f0acf`. The GitHub Release is
published as prerelease `true` at
https://github.com/LeftyBear/VMF/releases/tag/vmf-publisher-v0.0.1-dev.
This docs-only update is not new tag/release execution authorization, staging
authorization, commit authorization, or push authorization.

Avast vendor clearance is not recorded as obtained. The release proceeded on
the ADR-0019 VMF-side residual risk acceptance basis, not on Avast safety
certification or vendor clearance.
