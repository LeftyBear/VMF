# VMF v1.1 Candidates

Version : 1.1 Candidate
Status  : Candidate
Scope   : VMF
Depends : VMF_v1.0.md, Canon_v2.0.md

---

# 1. Policy

This document records VMF v1.1 Candidate items.

Candidate items are not part of VMF v1.0 until formally adopted.

VMF v1.0 is frozen and SHALL NOT be modified in place.

---

# 2. Candidates

### V001 Native Core Layer Generation

Status : Proposed
Priority : High

Build v1.0.1 validates generation layers against a fixed layer set and does not accept `Core` as a native generation layer. VMF v1.0 project construction records `Core` in `specs/vmf/manifest.yaml`, while future VMF tooling SHOULD support `Core` as a first-class generation target without changing VMF v1.0.

### V002 Manifest YAML Native Reader

Status : Proposed
Priority : Medium

VMF project generation currently uses a PowerShell wrapper to read `specs/vmf/manifest.yaml`. A future VMF version SHOULD define an official manifest reader contract for YAML-based project manifests.

### V003 Native Enum Generation

Status : Proposed
Priority : Medium

Build v1.0.2 generates Class Modules and Standard Modules but does not provide a native template or generation pipeline for Enum definitions. VMF v1.0 therefore excludes Enum generation from the official generation scope while preserving Enum definitions in the design. A future VMF version SHOULD provide native Enum templates, generation support, and manifest validation for Enum components without changing VMF v1.0.

### V004 Manifest Generation Contract

Status : Proposed
Priority : Medium

VMF v1.0 defines generated components under a `Modules` section in `specs/vmf/manifest.yaml`. This structure is sufficient for documentation and auditing but is not optimized as a direct generation contract. A future VMF version SHOULD define a native generation contract that explicitly describes generation targets, component types, layers, and generation options, allowing Build tooling to consume the manifest directly without changing VMF v1.0.
