# VMF Studio Specification

# VMF Studio v2.0 Specification v1.0 (Frozen Edition)

## Volume 0 — Master Specification (Foundation Documents)

Document ID: VMF-STUDIO-SPEC-V0  
Version: 1.0.0  
Status: Frozen  
Edition: Frozen Edition  
Authority: VMF Constitution  
Language: Japanese  
Established: 2026-07-19

[V0.1 Purpose](#v0.1-purpose)

[V0.2 Scope](#v0.2-scope)

[V0.2.1 Included Scope](#v0.2.1-included-scope)

[V0.2.2 Excluded Scope](#v0.2.2-excluded-scope)

[V0.3 Authority and Hierarchy](#v0.3-authority-and-hierarchy)

[V0.3.1 Authority](#v0.3.1-authority)

[V0.3.2 Document Hierarchy](#v0.3.2-document-hierarchy)

[V0.3.3 Single Source of Truth](#v0.3.3-single-source-of-truth)

[V0.4 Design Principles](#v0.4-design-principles)

[V0.5 Specification Structure](#v0.5-specification-structure)

[V0.5.1 Master Documents](#v0.5.1-master-documents)

[V0.5.2 Architecture Sections](#v0.5.2-architecture-sections)

[V0.5.3 Supporting Documents](#v0.5.3-supporting-documents)

[V0.5.4 Implementation Documents](#v0.5.4-implementation-documents)

[V0.6 Document Relationships](#v0.6-document-relationships)

[V0.6.1 Dependency Rules](#v0.6.1-dependency-rules)

[V0.6.2 Responsibilities](#v0.6.2-responsibilities)

[V0.6.3 Traceability](#v0.6.3-traceability)

[V0.7 Document Lifecycle and Versioning](#v0.7-document-lifecycle-and-versioning)

[V0.7.1 Lifecycle States](#v0.7.1-lifecycle-states)

[V0.7.2 Frozen Edition](#v0.7.2-frozen-edition)

[V0.7.3 Version Policy](#v0.7.3-version-policy)

[V0.7.4 Change Management](#v0.7.4-change-management)

[V0.8 Normative Language and Conformance](#v0.8-normative-language-and-conformance)

[V0.8.1 Normative Keywords](#v0.8.1-normative-keywords)

[V0.8.2 Conformance Requirements](#v0.8.2-conformance-requirements)

[V0.8.3 Compliance Verification](#v0.8.3-compliance-verification)

[V0.8.4 Exceptions](#v0.8.4-exceptions)

[V0.9 Publication and Maintenance](#v0.9-publication-and-maintenance)

[V0.9.1 Publication Requirements](#v0.9.1-publication-requirements)

[V0.9.2 Canonical Source](#v0.9.2-canonical-source)

[V0.9.3 Maintenance](#v0.9.3-maintenance)

[Revision History](#revision-history)

[Declaration](#declaration)

[Architecture Vision](#architecture-vision)

[Design Philosophy](#design-philosophy)

[A0 — Architecture Overview](#a0-—-architecture-overview)

[Architecture Map](#architecture-map)

[A1 — Architecture Principles](#a1-—-architecture-principles)

[A2 — Architecture Constraints](#a2-—-architecture-constraints)

[A3 — Documentation Conventions](#a3-—-documentation-conventions)

[S01 — Architecture Overview](#s01-—-architecture-overview)

[S02 — Component Architecture](#s02-—-component-architecture)

[S03 — Layer Architecture](#s03-—-layer-architecture)

[S04 — Module Architecture](#s04-—-module-architecture)

[S05 — Dependency Architecture](#s05-—-dependency-architecture)

[S06 — Workspace Architecture](#s06-—-workspace-architecture)

[S07 — Project Architecture](#s07-—-project-architecture)

[S08 — Package Architecture](#s08-—-package-architecture)

[S09 — Plugin Architecture](#s09-—-plugin-architecture)

[S10 — Service Architecture](#s10-—-service-architecture)

[S11 — Event Architecture](#s11-—-event-architecture)

[S12 — Workflow Architecture](#s12-—-workflow-architecture)

[S13 — Security Architecture](#s13-—-security-architecture)

[S14 — Deployment Architecture](#s14-—-deployment-architecture)

[S15 — Observability Architecture](#s15-—-observability-architecture)

[S16 — Runtime Architecture](#s16-—-runtime-architecture)

[S17 — UI Architecture](#s17-—-ui-architecture)

[S18 — Document Architecture](#s18-—-document-architecture)

[S19 — AI Integration Architecture](#s19-—-ai-integration-architecture)

[S20 — SDK Architecture](#s20-—-sdk-architecture)

[S21 — Testing Architecture](#s21-—-testing-architecture)

[S22 — Governance Architecture](#s22-—-governance-architecture)

[Version History](#version-history)

[Implementation Vision](#implementation-vision)

[I00 — Implementation Overview](#i00-—-implementation-overview)

[I01 — Coding Standards](#i01-—-coding-standards)

[I02 — Naming and Structure](#i02-—-naming-and-structure)

[I03 — Configuration and Environment](#i03-—-configuration-and-environment)

[I04 — Build and Generation](#i04-—-build-and-generation)

[I05 — Testing and Quality Gates](#i05-—-testing-and-quality-gates)

[I06 — Packaging and Versioning](#i06-—-packaging-and-versioning)

[I07 — Release and Deployment](#i07-—-release-and-deployment)

[I08 — Operations and Maintenance](#i08-—-operations-and-maintenance)

[Implementation Compliance](#implementation-compliance)

[Version History](#version-history-1)

[R00 — Reference Overview](#r00-—-reference-overview)

[R01 — RFC System](#r01-—-rfc-system)

[R02 — ADR System](#r02-—-adr-system)

[R03 — Lexicon](#r03-—-lexicon)

[R04 — Design Knowledge](#r04-—-design-knowledge)

[R05 — Glossary and Index](#r05-—-glossary-and-index)

[Reference Governance](#reference-governance)

[Version History](#version-history-2)

# V0.1 Purpose {#v0.1-purpose}

本書は、VMF Studio v2.0 Specification v1.0 (Frozen Edition) のマスター仕様書である。

本書の目的は、次のとおりである。

1. VMF Studio v2.0 に関する全仕様書の構成、責務および相互関係を定義する。  
2. 各仕様書の優先順位、参照規則、版管理および公開条件を統一する。  
3. VMF Constitution を最上位規範とする公式仕様体系を確立する。  
4. 仕様、設計、実装および運用の追跡可能性を保証する。  
5. Frozen Edition を将来の変更から保護し、安定した基準点として維持する。

本書は、仕様内容そのものを重複して定義する文書ではない。各領域の詳細は、対応する正式文書および各 Architecture Section に委譲する。

# V0.2 Scope {#v0.2-scope}

## V0.2.1 Included Scope {#v0.2.1-included-scope}

本仕様体系は、VMF Framework、VMF Studio、VMF Applications に共通する基盤契約、および Architecture、Implementation、Workspace、Repository、Configuration、Plugin、Service、Event、Workflow、Security、Deployment、Observability、Runtime、UI、Document、AI Integration、SDK、Testing、Governance、Publication、Compilation を対象とする。

## V0.2.2 Excluded Scope {#v0.2.2-excluded-scope}

本書は、個別 VMF Application の業務要件および画面仕様、プロジェクト固有の設定値、未承認の設計メモおよび実験記録、Draft 状態の RFC および ADR、一時的な実装詳細、外部製品固有の操作手順を直接の規定対象としない。

対象外事項であっても、VMF Studio の公開契約またはアーキテクチャへ影響する場合は、RFC または ADR を通じて本仕様体系へ接続しなければならない。

# V0.3 Authority and Hierarchy {#v0.3-authority-and-hierarchy}

## V0.3.1 Authority {#v0.3.1-authority}

VMF Constitution は、VMF Studio v2.0 Specification の最上位規範である。本書およびすべての下位文書は、Constitution に適合しなければならない。

## V0.3.2 Document Hierarchy {#v0.3.2-document-hierarchy}

文書の優先順位は、次のとおりとする。

1. VMF Constitution  
2. Volume 0 — Master Specification (Foundation Documents)  
3. Architecture Specification v1.0  
4. Implementation Specification v1.0  
5. 各 Architecture Section（S01〜S22）  
6. Publication Policy / Compilation Policy  
7. 承認済み RFC  
8. 承認済み ADR  
9. Lexicon  
10. Design Knowledge  
11. 実装、テスト、運用資料

上位文書と下位文書に矛盾がある場合、上位文書を正とする。矛盾が判明した下位文書は、修正、廃止または新版への移行対象としなければならない。

## V0.3.3 Single Source of Truth {#v0.3.3-single-source-of-truth}

一つの規範事項は、一つの正式文書のみで管理する。他の文書は同一内容を複製せず、正式な識別子を用いて参照する。

# V0.4 Design Principles {#v0.4-design-principles}

すべての仕様、設計および実装は、次の原則に従う。

* Single Source of Truth  
* Separation of Concerns  
* Layered Architecture  
* Explicit Dependency  
* Stable Public Contract  
* Evolvability  
* Backward Compatibility  
* Traceability  
* Testability  
* Observability  
* Security by Design  
* Documentation as Architecture

仕様は実装より先に定義されることを原則とする。実装は仕様へ適合しなければならず、実装上の都合のみを理由として上位仕様を暗黙に変更してはならない。

# V0.5 Specification Structure {#v0.5-specification-structure}

## V0.5.1 Master Documents {#v0.5.1-master-documents}

本仕様体系は、VMF Constitution、Volume 0 — Master Specification (Foundation Documents)、Architecture Specification v1.0、Implementation Specification v1.0、Publication Policy / Compilation Policy によって構成される。

## V0.5.2 Architecture Sections {#v0.5.2-architecture-sections}

Architecture Specification は、次の22セクションで構成される。

S01 Architecture Overview  
S02 Component Architecture  
S03 Layer Architecture  
S04 Module Architecture  
S05 Dependency Architecture  
S06 Workspace Architecture  
S07 Project Architecture  
S08 Package Architecture  
S09 Plugin Architecture  
S10 Service Architecture  
S11 Event Architecture  
S12 Workflow Architecture  
S13 Security Architecture  
S14 Deployment Architecture  
S15 Observability Architecture  
S16 Runtime Architecture  
S17 UI Architecture  
S18 Document Architecture  
S19 AI Integration Architecture  
S20 SDK Architecture  
S21 Testing Architecture  
S22 Governance Architecture

## V0.5.3 Supporting Documents {#v0.5.3-supporting-documents}

本仕様体系を補完する正式文書は、RFC Series、ADR Series、Lexicon、Design Knowledge、Manifest Registry、CHANGELOG、Voyage Log とする。

## V0.5.4 Implementation Documents {#v0.5.4-implementation-documents}

実装を具体化する文書には、Coding Standards、Naming Conventions、Project Structure、Build System、Packaging Rules、Distribution Rules、Extension Rules を含む。

# V0.6 Document Relationships {#v0.6-document-relationships}

## V0.6.1 Dependency Rules {#v0.6.1-dependency-rules}

1. 下位文書は上位文書を参照できる。  
2. 上位文書は下位文書へ規範的に依存してはならない。  
3. 文書間の循環参照を禁止する。  
4. 参照は、文書 ID、章番号または正式名称によって特定する。  
5. 廃止済み文書を現行規範として参照してはならない。

## V0.6.2 Responsibilities {#v0.6.2-responsibilities}

Constitution は理念、目的および統治原則を定義する。Volume 0 は仕様体系全体を統括する。Architecture Specification は構造および設計契約を定義する。Implementation Specification は実装上の規則を定義する。RFC は変更提案および合意形成を管理する。ADR は採用された設計判断と根拠を記録する。Design Knowledge は背景知識、原則および再利用可能な知見を保持する。Publication Policy は公開、配布および版管理を規定する。Compilation Policy は公開成果物の構成および生成を規定する。

## V0.6.3 Traceability {#v0.6.3-traceability}

すべての正式な変更は、RFC、ADR、CHANGELOG、Voyage Log、Git History のうち一つ以上に紐付かなければならない。仕様、設計判断、実装変更、テストおよび公開成果物は、相互に追跡可能でなければならない。

# V0.7 Document Lifecycle and Versioning {#v0.7-document-lifecycle-and-versioning}

## V0.7.1 Lifecycle States {#v0.7.1-lifecycle-states}

正式文書は、Draft、Review、Approved、Frozen、Published、Deprecated、Archived の状態を持つ。

Draft は作成中、Review はレビュー中、Approved は承認済み、Frozen は内容が固定された正式基準版、Published は正式公開版、Deprecated は新版への移行推奨版、Archived は保守対象外の保存版を示す。

標準的な状態遷移は、Draft → Review → Approved → Frozen → Published とする。Deprecated および Archived への移行は、Governance Architecture および Publication Policy に従う。

## V0.7.2 Frozen Edition {#v0.7.2-frozen-edition}

1. 規範内容を直接変更してはならない。  
2. 誤字、脱字、リンク切れなど、意味を変更しない修正のみを許可する。  
3. 規範的変更は、新しい版として制定する。  
4. 変更理由および影響は、RFC、ADR または CHANGELOG に記録する。  
5. 旧 Frozen Edition は削除せず、参照可能な状態で保存する。

## V0.7.3 Version Policy {#v0.7.3-version-policy}

文書版番号は、原則として MAJOR.MINOR.PATCH 形式で管理する。

MAJOR は非互換な規範変更、MINOR は後方互換性を維持した追加または拡張、PATCH は意味を変更しない訂正を示す。版表示を v1.0 と省略する場合、その正式な解釈は v1.0.0 とする。

## V0.7.4 Change Management {#v0.7.4-change-management}

規範的変更は、RFC の提出、レビュー、承認または却下、必要に応じた ADR の制定、新版の作成、CHANGELOG の更新、公開および移行案内の手順を経なければならない。

# V0.8 Normative Language and Conformance {#v0.8-normative-language-and-conformance}

## V0.8.1 Normative Keywords {#v0.8.1-normative-keywords}

MUST / REQUIRED / SHALL は必須要件、MUST NOT / SHALL NOT は禁止事項、SHOULD / RECOMMENDED は原則として従うべき推奨事項、SHOULD NOT は原則として避けるべき事項、MAY / OPTIONAL は実装者または運用者が選択できる事項を示す。

これらのキーワードは、規範的な意味で使用される場合、大文字で表記する。

## V0.8.2 Conformance Requirements {#v0.8.2-conformance-requirements}

VMF Studio の正式な実装、拡張、文書および公開成果物は、次を満たさなければならない。

1. Constitution に適合すること。  
2. 本書に登録された仕様体系へ属すること。  
3. Architecture Specification に準拠すること。  
4. Implementation Specification に従うこと。  
5. 必要な Quality Gate を通過すること。  
6. 対象版を明記して適合性を宣言すること。

## V0.8.3 Compliance Verification {#v0.8.3-compliance-verification}

適合性は、Architecture Compliance Review、Design Review、Code Review、Static Analysis、Unit Test、Integration Test、Contract Test、Documentation Review によって検証する。詳細は、S21 Testing Architecture および S22 Governance Architecture に委譲する。

## V0.8.4 Exceptions {#v0.8.4-exceptions}

仕様からの逸脱は、技術的または運用上の根拠が明確であり、影響範囲が評価され、RFC または ADR に記録され、正式なレビューおよび承認を受け、解消、再評価または恒久化の条件が定義されている場合に限り認める。

# V0.9 Publication and Maintenance {#v0.9-publication-and-maintenance}

## V0.9.1 Publication Requirements {#v0.9.1-publication-requirements}

正式公開される文書は、Constitution に適合し、Master Specification に登録され、文書 ID、版、状態および権限を明記し、必要なレビューおよび承認を完了し、CHANGELOG に記録され、参照先が有効であり、公開成果物が Compilation Policy に適合していなければならない。

## V0.9.2 Canonical Source {#v0.9.2-canonical-source}

正式な正本は、Publication Policy で指定された管理場所に保持する。Google Docs、Markdown および公開成果物が併存する場合、どの形式を編集正本とするかを明示しなければならない。内容の差異が発生した場合は、指定された編集正本を基準として同期する。

## V0.9.3 Maintenance {#v0.9.3-maintenance}

文書の保守では、Correctness、Consistency、Completeness、Traceability、Maintainability、Extensibility、Reusability を維持する。

# Revision History {#revision-history}

Version: 1.0.0  
Date: 2026-07-19  
Status: Frozen  
Summary: Volume 0 — Master Specification (Foundation Documents) 初版制定

# Declaration {#declaration}

本書を、VMF Studio v2.0 Specification v1.0 (Frozen Edition)、Volume 0 — Master Specification (Foundation Documents) の正式版として制定する。

本書は VMF Constitution の下位規範として、VMF Studio v2.0 に関する仕様体系、文書階層、ライフサイクル、適合性および公開管理の基準点となる。

# Volume 1 — Architecture Specification v1.0

Document ID: VMF-AS-1.0  
Version: 1.0.0  
Status: Frozen  
Authority: VMF Constitution v1.0  
Applies To: VMF / VMF Studio / VMF Applications  
Edition: Frozen Edition  
Established: 2026-07-19  
Revision: 1.0.0

# Architecture Vision {#architecture-vision}

VMF Studio is an Architecture-Centric Development Platform.

VMF Studio は、アーキテクチャを開発の中心に据える統合開発プラットフォームである。設計、実装、文書、テスト、AI 支援および運用は、共通仕様を基準として連携する。

## Design Philosophy {#design-philosophy}

Architecture First  
Specification Driven  
Implementation Follows Architecture  
Documentation as an Asset  
Human–AI Collaboration  
Continuous Evolution with Governance

# A0 — Architecture Overview {#a0-—-architecture-overview}

本巻は、VMF Studio v2.0 の構造、責務、境界、相互作用および統治規則を定義する。Architecture Specification は実装詳細ではなく、実装が適合すべき設計契約を規定する。

## Architecture Map {#architecture-map}

Foundation: S01–S05  
Development: S06–S09  
Execution: S10–S16  
Experience and Governance: S17–S22

# A1 — Architecture Principles {#a1-—-architecture-principles}

各構成要素は単一責務を持たなければならない。関心事は明確に分離し、依存は安定した抽象へ向ける。コンポーネント間連携は公開契約を介し、設計判断は追跡可能でなければならない。

# A2 — Architecture Constraints {#a2-—-architecture-constraints}

循環依存を導入してはならない。  
非公開 API を利用してはならない。  
レイヤー規則を越えて直接参照してはならない。  
例外は ADR に記録しなければならない。  
互換性を破る変更は、原則としてメジャーバージョンで扱わなければならない。

# A3 — Documentation Conventions {#a3-—-documentation-conventions}

規範要件には MUST、MUST NOT、SHOULD、SHOULD NOT、MAY を使用する。各章は Purpose、Scope、Design Principles、Architecture、Components、Interfaces、Lifecycle、Extension Points、Compliance、References を基本構成とする。各章は Document ID、Version、Status、Decision Traceability、Version History を保持する。

# S01 — Architecture Overview {#s01-—-architecture-overview}

VMF Studio v2.0 のアーキテクチャ全体像と適合原則を定義する。すべての設計は Constitution に従い、Architecture Specification と Implementation Specification の間で追跡可能でなければならない。

# S02 — Component Architecture {#s02-—-component-architecture}

VMF Studio は Workspace、Document、Project、Extension、Service、Event Bus、Workflow Engine、AI Engine、Runtime Platform により構成される。各コンポーネントは明確な責務と公開契約を持ち、内部状態を直接共有してはならない。

# S03 — Layer Architecture {#s03-—-layer-architecture}

標準レイヤーは Common、Core、Domain、Application、Studio、Applications とする。依存は Common に向かう安定方向を基本とし、循環依存および下位層から上位層への逆参照を禁止する。

# S04 — Module Architecture {#s04-—-module-architecture}

モジュールは公開インターフェース、内部実装、設定、資源、テスト、文書から構成される。公開契約と内部実装を分離し、独立して検証・配布・交換できる単位とする。

# S05 — Dependency Architecture {#s05-—-dependency-architecture}

依存関係は明示的な契約を介し、より安定した抽象へ向ける。許可されない依存、レイヤー越え参照、循環依存、内部実装への直接依存を禁止する。

# S06 — Workspace Architecture {#s06-—-workspace-architecture}

Workspace は Projects、Documents、Packages、Configuration、Extensions、Build、Metadata を管理する開発境界である。Workspace は独立性、再現性、移植性および永続性を備えなければならない。

# S07 — Project Architecture {#s07-—-project-architecture}

Project は Metadata、Source、Documents、Resources、Packages、Tests、Build、Output を集約する開発単位である。Project は独立して構築、検証、パッケージ化、リリースおよび保守できなければならない。

# S08 — Package Architecture {#s08-—-package-architecture}

Package は、再利用・配布・依存解決の単位である。各 Package は一意な識別子、版、公開契約、依存宣言、互換性条件、完全性情報およびライフサイクル状態を持たなければならない。

# S09 — Plugin Architecture {#s09-—-plugin-architecture}

Plugin は、VMF Studio の中核を変更せずに機能を拡張するための隔離された拡張単位である。Plugin は Manifest、公開 Extension Point、権限宣言、依存関係、起動・停止処理および障害境界を持たなければならない。

# S10 — Service Architecture {#s10-—-service-architecture}

Service は、明示的な契約を通じて機能を提供する長寿命コンポーネントである。登録、解決、ライフサイクル、スコープ、依存注入および障害分離を定義し、呼出側が内部実装へ依存することを禁止する。

# S11 — Event Architecture {#s11-—-event-architecture}

Event Architecture は、発行者と購読者を疎結合に連携させる。Event は不変の識別子、版、発生時刻、発生元、Payload 契約および相関情報を持ち、配送失敗、重複、順序および再試行方針を明示しなければならない。

# S12 — Workflow Architecture {#s12-—-workflow-architecture}

Workflow は、複数の処理を状態遷移と制御規則によって編成する実行単位である。Step、Condition、Context、Result、Cancellation、Retry、Compensation および監査証跡を定義し、再実行可能性を確保しなければならない。

# S13 — Security Architecture {#s13-—-security-architecture}

Security Architecture は、認証、認可、秘密情報、権限境界、入力検証、監査および最小権限を規定する。すべての外部入力は信頼せず、機密情報をコード・ログ・公開文書へ埋め込んではならない。

# S14 — Deployment Architecture {#s14-—-deployment-architecture}

Deployment Architecture は、成果物の構築、署名、配布、更新、互換性確認、ロールバックおよび廃止を定義する。配備は再現可能で、対象環境、版、依存関係および検証結果を追跡できなければならない。

# S15 — Observability Architecture {#s15-—-observability-architecture}

Observability Architecture は、Logging、Metrics、Tracing、Telemetry および診断コンテキストを統合する。観測情報は相関可能で、機密情報を含まず、障害解析と品質判断に必要な粒度を持たなければならない。

# S16 — Runtime Architecture {#s16-—-runtime-architecture}

Runtime Architecture は、起動、構成読込、サービス初期化、実行コンテキスト、リソース管理、終了および障害回復を規定する。初期化順序と所有権を明示し、予測可能で安全な停止を保証しなければならない。

# S17 — UI Architecture {#s17-—-ui-architecture}

UI Architecture は、View、ViewModel、Command、Navigation、Dialog および User Interaction を分離する。表示層は業務ロジックを保持せず、状態変更は明示的なコマンドと通知を通じて実行しなければならない。

# S18 — Document Architecture {#s18-—-document-architecture}

Document Architecture は、Document Model、Editor、Persistence、Undo/Redo、Versioning および競合処理を定義する。文書状態は一貫性を保ち、保存形式と編集モデルを分離し、操作履歴を追跡可能にしなければならない。

# S19 — AI Integration Architecture {#s19-—-ai-integration-architecture}

AI Integration Architecture は、AI Provider、Prompt Engine、Context Builder、Tool Calling、Knowledge Retrieval および Agent Framework を統治する。AI 出力は検証可能で、人間の承認境界、権限、機密性、再現性および監査性を明示しなければならない。

# S20 — SDK Architecture {#s20-—-sdk-architecture}

SDK Architecture は、公開 API、型、契約、Extension Point、サンプル、互換性および Developer Experience を定義する。SDK は内部実装を露出せず、版管理された安定契約と明確な移行手段を提供しなければならない。

# S21 — Testing Architecture {#s21-—-testing-architecture}

Testing Architecture は、Unit、Integration、Contract、System、Regression および AI-assisted Testing を統合する。テストは仕様要件へ追跡可能で、決定的、独立、再実行可能であり、Quality Gate の客観的証拠を提供しなければならない。

# S22 — Governance Architecture {#s22-—-governance-architecture}

Governance Architecture は、ADR、RFC Lifecycle、Quality Gates、Architecture Compliance および Design Review を定義する。重要な変更は正式な審議、承認、記録および適合性確認を経なければならず、例外には期限と再評価条件を設定しなければならない。

# Version History {#version-history}

v1.0.0 — Initial Frozen Edition

# Volume 2 — Implementation Specification v1.0

Document ID: VMF-IS-1.0  
Version: 1.0.0  
Status: Frozen  
Authority: VMF Constitution v1.0 / Architecture Specification v1.0  
Applies To: VMF / VMF Studio / VMF Applications  
Edition: Frozen Edition  
Established: 2026-07-19  
Revision: 1.0.0

# Implementation Vision {#implementation-vision}

Implementation follows Architecture.  
本巻は、Architecture Specification を実装へ変換するための規範を定義する。実装は設計契約、依存方向、公開境界、品質ゲートおよび変更管理に適合しなければならない。

# I00 — Implementation Overview {#i00-—-implementation-overview}

本仕様は、コード、設定、生成物、ビルド、テスト、パッケージ、リリースおよび運用準備に適用する。実装判断は Architecture Specification、ADR、RFC およびテスト証跡と追跡可能でなければならない。

# I01 — Coding Standards {#i01-—-coding-standards}

コードは明確な責務、明示的な契約、予測可能な制御フローを持たなければならない。暗黙の共有状態、未処理エラー、循環参照、未文書化の公開 API を禁止する。

# I02 — Naming and Structure {#i02-—-naming-and-structure}

名称は役割、レイヤー、責務を表現しなければならない。ディレクトリ、モジュール、クラス、インターフェース、設定および成果物は、一貫した命名規則と配置規則に従う。

# I03 — Configuration and Environment {#i03-—-configuration-and-environment}

設定は実装から分離し、環境差分を明示的に管理する。秘密情報をソースコードへ埋め込んではならない。既定値、上書き規則、検証規則を定義する。

# I04 — Build and Generation {#i04-—-build-and-generation}

ビルドは再現可能、決定的、検証可能でなければならない。生成物は入力仕様、テンプレート、マニフェストおよびツールバージョンから追跡可能でなければならない。

# I05 — Testing and Quality Gates {#i05-—-testing-and-quality-gates}

Unit、Integration、Contract、System および AI-assisted Testing を適切に組み合わせる。必須テスト、静的検証、依存検査、仕様適合性確認を通過しない変更はリリースしてはならない。

# I06 — Packaging and Versioning {#i06-—-packaging-and-versioning}

配布単位、依存関係、互換性、署名、メタデータおよびロールバック手順を定義する。バージョンは Semantic Versioning の原則に従い、互換性を破る変更はメジャーバージョンで扱う。

# I07 — Release and Deployment {#i07-—-release-and-deployment}

リリースは承認済み成果物のみを対象とし、検証、パッケージ化、公開、配備、確認、ロールバックの順に実行する。各段階はログと証跡を残さなければならない。

# I08 — Operations and Maintenance {#i08-—-operations-and-maintenance}

運用時のログ、監視、障害対応、変更管理、廃止およびアーカイブを定義する。実装から得られた知識は Design Knowledge、ADR、RFC および次版仕様へ還元する。

# Implementation Compliance {#implementation-compliance}

すべての実装成果物は、Architecture Specification と本仕様の両方に適合しなければならない。例外は ADR で承認し、必要に応じて RFC により仕様変更として管理する。

# Version History {#version-history-1}

v1.0.0 — Initial Frozen Edition

# Volume 3 — Reference

Document ID: VMF-REF-1.0  
Version: 1.0.0  
Status: Frozen  
Authority: VMF Constitution v1.0 / Architecture Specification v1.0 / Implementation Specification v1.0  
Applies To: VMF / VMF Studio / VMF Applications  
Edition: Frozen Edition  
Established: 2026-07-19  
Revision: 1.0.0

# R00 — Reference Overview {#r00-—-reference-overview}

本巻は、VMF の仕様体系を補完する参照文書を収録する。RFC、ADR、Lexicon、Design Knowledge、Glossary は、仕様変更、設計判断、用語、知識および相互参照を管理するための公式基盤である。

# R01 — RFC System {#r01-—-rfc-system}

RFC は、仕様・プロセス・アーキテクチャに対する重要な変更提案を審議し、採否と履歴を記録する文書である。RFC-0000 は RFC の提出、レビュー、承認、凍結、廃止のライフサイクルを定義する。

# R02 — ADR System {#r02-—-adr-system}

Architecture Decision Record は、重要な設計判断、その背景、選択肢、決定、結果および影響を記録する。ADR は採用済み設計の根拠を保持し、将来の変更時に判断経緯を追跡可能にする。

# R03 — Lexicon {#r03-—-lexicon}

Lexicon は、VMF における公式用語、定義、許容表記、禁止表記および関連概念を管理する。仕様、コード、文書、UI、AI 支援では Lexicon に定義された用語を一貫して使用しなければならない。

# R04 — Design Knowledge {#r04-—-design-knowledge}

Design Knowledge は、設計上の暗黙知、形式知、価値観、パターン、失敗例および実践知を蓄積する。Architecture、Implementation、Operation から得られた知識を整理し、次の設計判断へ還元する。

# R05 — Glossary and Index {#r05-—-glossary-and-index}

Glossary は本仕様書で使用する主要語の簡潔な説明を提供する。Index は文書 ID、章、RFC、ADR、用語および関連仕様への検索導線を提供する。

# Reference Governance {#reference-governance}

Reference 文書は Constitution、Publication Policy、Compilation Policy、RFC Process および Governance Architecture に従って管理する。参照文書が凍結仕様と矛盾する場合、上位規範を優先し、必要に応じて RFC または ADR を起票する。

# Version History {#version-history-2}

v1.0.0 — Initial Frozen Edition  
