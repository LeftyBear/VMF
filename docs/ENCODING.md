# Encoding

## Purpose

This document defines the character encoding standards used throughout the VMF project.

Maintaining consistent encodings prevents character corruption and ensures compatibility between VBA source files, GitHub, and development tools.

---

## Encoding Rules

| Target | Encoding |
|--------|----------|
| `src/**/*.bas` | Shift_JIS |
| `src/**/*.cls` | Shift_JIS |
| `src/**/*.frm` | Shift_JIS |
| `docs/**/*.md` | UTF-8 |
| `README.md` | UTF-8 |
| `CHANGELOG.md` | UTF-8 |

---

## Notes

- VBA source files **MUST** use **Shift_JIS**.
- Markdown documents **MUST** use **UTF-8**.
- Do not mix encodings within the same directory.
- All new documentation should follow these rules.

---

## References

- `README.md`
- `docs/AI_DEVELOPMENT_RULES.md`
