# Versioning

## Purpose

This document defines the versioning policy used throughout the VMF project.

Its purpose is to ensure consistent version management for source code, documentation, releases, and add-ins while maintaining long-term compatibility.

---

## Scope

This policy applies to:

- VMF
- Build.xlam
- All project documentation
- GitHub releases

---

## Versioning Policy

The VMF project follows semantic versioning.

| Version | Meaning |
|---------|---------|
| Major | Breaking architectural changes |
| Minor | Backward-compatible feature additions |
| Patch | Backward-compatible fixes and documentation improvements |

Example:

```text
v1.0.0
v1.1.0
v1.1.1
```

---

## VMF v1.0

VMF v1.0 is frozen.

No architectural changes are permitted after the design freeze.

Any improvement proposals shall be managed separately as **VMF v1.1 Candidate**.

---

## Documentation Versioning

Documentation should remain synchronized with the project version.

Documentation quality improvements do not require architectural version changes.

---

## Release Policy

A release should be created only after:

- implementation is complete
- documentation is updated
- testing is complete

---

## Compatibility

Minor and Patch releases should maintain backward compatibility whenever possible.

Breaking changes require a new Major version.

---

## References

- README.md
- docs/build/ROADMAP.md
- docs/build/AI_DEVELOPMENT_RULES.md
- specs/
