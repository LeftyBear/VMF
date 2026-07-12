# VMF

VMF is the repository for the VMF architecture, Build.xlam, and applications
that use VMF.

Build.xlam v1.1 and VMF v1.0 are official releases. Frozen specifications are
not modified in place; future improvements are recorded as candidate documents.

---

## Architecture

```text
Presentation
    -> Application
        -> Domain
            -> Common

Infrastructure
    -> Common
```

Dependencies are one-way. Circular dependencies are prohibited.

---

## Directory Structure

```text
src/            Build and VMF source
tests/          Unit and integration tests
specs/          Official Build and VMF specifications
candidates/     Future candidate changes
docs/           Development, build, and release documentation
templates/      Build generation templates
tools/          Build, test, and audit tools
applications/   Applications that use VMF
dist/           Generated distribution artifacts only
```

The SchoolTimetable application workspace is under
`applications/SchoolTimetable/`.

The current official Build artifact is:

```text
dist/release/Build/v1.1/Build.xlam
```

---

## Release Reports

- `docs/releases/VMF_v1.0_ReleaseReport.md`
- `docs/releases/Build_v1.0.1_ReleaseReport.md`
- `docs/releases/Build_v1.0.2_ReleaseReport.md`
- `docs/releases/Build_v1.1_ReleaseReport.md`

---

## Development Rules

- Do not modify frozen VMF v1.0 specifications in place.
- Do not modify frozen Build v1.0.x specifications in place.
- Record future improvements under `candidates/`.
- Keep specifications, implementation, tests, tools, documentation, and
  generated artifacts separated by directory responsibility.
- Treat GitHub as the single source of truth.

---

## License

See [LICENSE](LICENSE).
