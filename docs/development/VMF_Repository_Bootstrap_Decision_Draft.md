# VMF Repository Bootstrap Decision Record

## 1. Document Status

| Field | Value |
|---|---|
| Decision ID | `BOOTSTRAP-001` |
| Status | Temporary Draft |
| Type | Temporary Bootstrap Decision Record |
| Normative Authority | None |
| Repository Change Authority | None |
| Repository Path | `docs/development/VMF_Repository_Bootstrap_Decision_Draft.md` |
| Base Repository | VMF |
| Repository URL | `https://github.com/LeftyBear/VMF.git` |
| Base Branch | `main` |
| Base Commit | `f535d67d38bcc635149ebcfad8be2a67e1216b7c` |
| Base Working Tree | Clean |
| Date | `2026-07-18` |
| Owner | Human Repository Owner |
| Retention | Open Decision |
| Supersede Condition | Open Decision |

This record is non-normative. It does not amend, replace, reinterpret, or supersede Canon, Architecture, Specification Hierarchy, Manifest, Glossary, VMF specifications, or any Frozen document.

この記録は、確認済みのSSOT不整合を段階的に扱うための実行順序、対象、対象外、Change Gates、停止条件を保存するTemporary Draftである。正式RFC、正式Governance、Canon Amendment、またはRepository変更の包括的承認ではない。

## 2. Purpose

最初のRepository改善候補を、この非規範Bootstrap Decision Record 1件に限定し、その後の調査、Decision、変更を小さく検証可能な単位で順序付ける。

この記録は、新しい規範を制定せず、既存文書の権威を変更せず、将来GovernanceまたはCandidate/RFC制度を先取りしない。

## 3. Repository Facts and References

この章はBase Commitで確認した事実の所在を参照し、既存文書の規範内容を複製しない。

* Canon: [`../../specs/build/Canon_v2.0.md`](../../specs/build/Canon_v2.0.md)
* Architecture: [`../../specs/build/Architecture_v2.0.md`](../../specs/build/Architecture_v2.0.md)
* Specification Hierarchy: [`../../specs/build/SpecificationHierarchy_v2.0.md`](../../specs/build/SpecificationHierarchy_v2.0.md)
* Specification registry: [`../../specs/build/Manifest_v2.0.md`](../../specs/build/Manifest_v2.0.md)
* Official terminology: [`../../specs/build/Glossary_v2.0.md`](../../specs/build/Glossary_v2.0.md)
* Frozen VMF implementation contract: [`../../specs/vmf/VMF_v1.0.md`](../../specs/vmf/VMF_v1.0.md)
* AI instructions: [`../../AGENTS.md`](../../AGENTS.md)
* Vision: [`../../Vision.md`](../../Vision.md)
* Repository orientation: [`../../README.md`](../../README.md)
* Candidate records: [`../../candidates/`](../../candidates/)
* Change history: [`../../CHANGELOG.md`](../../CHANGELOG.md)
* Release records: [`../releases/`](../releases/)
* Development records: [`./`](./)

Base Commitの調査では、Hierarchy表現の不一致、不完全なManifest登録、独立した正式Governance/RFC制度の未整備、および既存recordsによる関連責務の部分的充足が確認された。詳細な定義と権威は上記owner文書に残し、この記録では再定義しない。

## 4. Adopted Bootstrap Decisions

1. 最初のRepository変更候補を、この非規範Bootstrap Decision Record 1件に限定する。
2. 配置先を `docs/development/VMF_Repository_Bootstrap_Decision_Draft.md` とする。
3. 新しい `governance/` ディレクトリを現時点で作成しない。
4. 規範文書を変更しない。
5. 後続Phaseの順序、対象、対象外、Change Gates、停止条件だけを記録する。
6. この記録を正式RFCとは呼ばず、規範権限またはRepository Change Authorityを持たせない。
7. Repository Owner Authorization、Commit Authorization、Push Authorizationを独立して扱う。

Adopted Bootstrap Decisionsは、このTemporary Draftの計画境界を記録するものであり、Canon、Specification、Governance、RFC制度その他の規範文書の採用を意味しない。また、この記録自体は後続Phaseの実装を承認しない。

## 5. Backlog Traceability

Repository Improvement BacklogのB01における旧表現 `SSOT Recovery RFC` は、RFC制度確立前の暫定名称であり、正式RFCとして採用しない。

B01は、この非規範Bootstrap Decision Recordとして扱う。BacklogはRepository外の計画成果物であり、この記録への言及によって規範文書にはならない。

## 6. Migration Sequence

| Order | Phase | Limited Target | Explicit Non-Target |
|---|---|---|---|
| 0 | Investigation | Snapshot、Mapping、Backlog、Temporary Draftによるread-only調査 | Repository変更、規範採用 |
| 1 | Bootstrap Decision | 順序、対象、対象外、Change Gates、停止条件の記録 | 規範文書変更、正式RFC化 |
| 2 | Canon Scope Decision | Canon適用範囲に関するHuman Decision | 現行Canonのin-place変更 |
| 3 | Hierarchy Recovery | 正式Hierarchyの一意化方法のDecision | AGENTS、Vision、Manifestとの同時変更 |
| 4 | Registry Recovery | Manifest登録対象、inventory、用語区別のDecision | 未承認のManifest変更、dataの自動分離 |
| 5 | AI Instruction Alignment | AGENTSを正式Hierarchy参照型へ移行する候補 | AGENTSによるHierarchy再定義 |
| 6 | Governance Establishment | 承認主体、変更、例外、採否、廃止の正式化候補 | 未制定Governanceの遡及適用 |
| 7 | Candidate/RFC Establishment | `candidates/` を活用する提案制度候補 | 過去Candidateの一括変換 |
| 8 | Knowledge and Journal Indexing | 既存情報への非規範index | 規範内容の複製 |
| 9 | Automated Validation | Hierarchy、Manifest、reference、IDのread-only検査 | 自動修正、初期blocking運用 |

各Phaseの開始には、それぞれ別のHuman Decisionおよび必要なRepository Owner Authorizationを要する。この記録だけではPhase 2以降の実装を開始できない。

## 7. Scope and Explicit Exclusions

この記録の追加だけをScopeとする。`docs/development/VMF_Repository_Bootstrap_Decision_Draft.md` 以外のすべてのRepository contentを除外する。

新しい `governance/` ディレクトリの作成、規範文書の変更、既存recordsの修正、commit、pushは対象外である。

## 8. Change Gates

1. Repository Identityを確認する。
2. HEADが承認されたBase Commitと一致する。
3. working treeおよびindexがcleanである。
4. Files To Modifyが新規1件だけである。
5. Files Explicitly Excludedに差分がない。
6. Repository変更前にRepository Owner Authorizationを得る。
7. commit前に完全なdiffとValidation resultsをHuman Reviewする。
8. Commit AuthorizationとPush Authorizationを別々に得る。
9. commit後に新しいBase CommitとSnapshotを確認する。

## 9. Stop Conditions

次のいずれかに該当した場合は変更を開始しない、または停止してHuman Decisionを求める。

1. Repository Identity、Branch、HEAD、Working Treeが承認済みBase Stateと一致しない。
2. Repository Owner Authorizationがない。
3. Scope外ファイル、既存ファイル、`governance/` ディレクトリの変更が必要になる。
4. Frozen文書または規範文書の変更が必要になる。
5. Facts、Decisions、Proposalsを分離できない。
6. Relative link、UTF-8、Markdown、diff reviewのいずれかが失敗する。
7. 新しい未解決矛盾が検出される。

## 10. Prohibited Actions

* 新Constitutionを既存Canonと並立させない。
* 新Lexiconを既存Glossaryと並立させない。
* 新しい `governance/` ディレクトリを作成しない。
* Frozen文書を承認されたversion revisionなしに直接変更しない。
* 未確立のGovernanceまたはRFC制度を遡及適用しない。
* 歴史文書を現在形式へ一括変換しない。
* 複数のSSOT責務を一つの変更で同時修正しない。
* Base Commit不一致またはdirty working treeで変更を開始しない。
* 会話履歴、調査Draft、提案をRepositoryの権威文書より優先しない。
* Canon、Hierarchy、Manifest、Glossaryの規範内容をこの記録へ複製しない。

## 11. External Temporary Draft References

次のRepository外Temporary Draftを作成時の参考資料とした。

* VMF Repository Migration Strategy v1.0 Draft。
* VMF Decision Package Standard v1.0 Draft。
* VMF Canonical Synchronization Protocol v1.0 Draft。
* BOOTSTRAP-001 Review Report。

これらはRepository外のTemporary Draftまたは会話成果物であり、規範権限を持たない。Repository内には、これらの一時ファイルへのrelative linkまたは絶対pathを作成しない。

## 12. Open Decisions

1. この記録のretention期間。
2. この記録のsupersede、reject、archive条件。
3. Canonの適用範囲。
4. FrozenなSpecificationHierarchyを回復する正式versionとadoption method。
5. Manifest登録対象の判定基準と完全inventory。
6. Governance specificationのscope、placement、approval authority。
7. CandidateとRFCの制度上の関係。
8. Design KnowledgeおよびJournal indexのplacementとowner。
9. Read-only validatorの実装と運用条件。

Open Decisionを推測で解決しない。各事項は該当Phaseの別Decisionで扱う。

## 13. Source Declaration

この記録は、Base Branch `main`、Base Commit `f535d67d38bcc635149ebcfad8be2a67e1216b7c` のRepository調査、およびRepository外のSnapshot、Mapping、Backlog、Migration Strategy Draft、Decision Package Standard Draft、Canonical Synchronization Protocol Draft、Review Reportに基づく。

Repository外の各成果物とこの記録は、Repositoryのnormative sourceではない。Repositoryとの不一致がある場合はRepositoryを優先し、文書間のauthority conflictは正式Hierarchyに従う。
