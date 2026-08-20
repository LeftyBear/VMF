# P2-01 — Blueprint Specification v0.1 Scope Definition

## Status

COMPLETE / docs-only scope definition

## Purpose

Define the initial scope of Blueprint Specification v0.1.

P2 assumes the verified P1 baseline and starts from the principle that users should not directly maintain YAML. Instead, VMF should support a safe and reproducible path from natural-language intent to an approved Blueprint and then to generated VBA.

## Canonical Pipeline

```text
Requirement
-> AI-generated Blueprint draft
-> Human review and approval
-> Manifest generation
-> Template
-> GenerateContext
-> Generator
-> VBA
```

## Core Principle

Blueprint is the design canon and Single Source of Truth.

Manifest is generated data derived from Blueprint for code generation. It is not a human-maintained design document.

## In Scope for v0.1

Blueprint Specification v0.1 covers the minimum stable design information required to describe VBA generation targets.

Included:

- Blueprint identity
- Blueprint version
- Blueprint status
- approval state
- workbook or add-in target description
- module definitions
- procedure definitions
- procedure kind:
  - Sub
  - Function
- visibility:
  - Public
  - Private
- parameters
- return value definition for Function
- responsibility description
- dependencies
- generation policy
- Manifest derivation boundary

## Out of Scope for v0.1

The following are explicitly out of scope:

- AI implementation for converting natural language to Blueprint
- interactive review UI
- Excel runtime execution
- broad Generator rewrite
- advanced type inference
- automatic refactoring
- external API integration
- OAuth or cloud integration
- direct generation from unapproved Blueprint
- manual Manifest maintenance workflow

## Blueprint / Manifest Boundary

Blueprint:

- is reviewed by humans
- is approved before generation
- contains design intent
- is the authoritative source

Manifest:

- is derived from Blueprint
- is consumed by generation logic
- may normalize Blueprint content
- must not introduce independent design decisions
- must not become the maintained source of truth

## Approval Rule

Generation must not proceed from an unapproved Blueprint.

At minimum, Blueprint status must distinguish:

- draft
- review
- approved
- rejected
- superseded

Only `approved` Blueprint documents may be used for Manifest generation.

## v0.1 Success Criteria

P2-01 is complete when:

- Blueprint v0.1 scope is documented
- Blueprint / Manifest responsibility boundary is documented
- v0.1 in-scope and out-of-scope areas are explicit
- later implementation tasks can reference this document as the scope anchor

## Verification

Docs-only task.

Expected checks:

- Markdown added
- No VBA implementation changed
- No generator behavior changed
- No test execution required
