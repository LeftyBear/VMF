# VMF Codex Playbook vNext

## 1. 目的

本Playbookは、`AGENTS.md`で定義されたVMFのAI運用原則を、Chat / Work / Codexの日常運用へ適用するための実務ガイドである。

本Playbookは主に以下を定める。

* 推奨モデル・reasoning運用
* Route選択
* 役割分担
* Standard Instruction Packet
* 実行・検証手順
* Git手動引継ぎ
* 結果返却形式

Frozen仕様、認可、安全境界、Git禁止事項などの恒久規則は`AGENTS.md`を正典とする。
矛盾がある場合は`AGENTS.md`およびより上位の権威ある指示を優先する。

### 1.1 Codexの定義

本Playbookでいう **Codex** は、VMFにおける **repository execution role** であり、その役割をOpenAI Codex agentで実現するものを指す。

Codexは単一のsurfaceや実行場所を意味しない。delegated VMF workでは、次のCodex Execution Routeの一方を取る。

* Codex Cloud
* VS Code Codex LOCAL

`Codex`は以下を意味しない。

* underlying model名
* reasoning level
* terminal / shellそのもの
* process identityまたはUser identity
* capabilityそのもの
* authorizationそのもの
* coding可能なAI一般

したがって、**roleとしてCodexを選ぶこと**と、**どのCodex Execution Routeを使うか**は別判断である。Codex Execution Routeは、必要なrepository context、local tooling、sandbox/isolation、process identity、credentials、network capability、interactive reviewの必要性から選択する。routeを選択しただけではauthorizationは発生しない。

Codexが使用するmodelはSection 3.1で別に選択する。将来、Codex以外の製品・agentをrepository execution roleへ割り当てる場合は、能力が類似しているだけで自動代替せず、本Playbookまたは`AGENTS.md`でrole mappingを明示的に更新する。

---

## 2. 基本運用

* 1依頼1目的
* 必要なfileだけ確認する
* current valid stateだけを引き継ぐ
* 既存patternを優先する
* Focused Verificationを先に行う
* 不要な再読・再調査を避ける
* 結果報告は短く、必要なevidenceは省略しない

モデル能力やreasoning性能が向上しても、`AGENTS.md`の仕様・認可・安全境界は変わらない。

---

## 3. 推奨実行設定

### 3.1 Model

既定は、その時点でVMF作業に利用可能な高性能コーディングモデルとする。
モデル名は運用設定であり、恒久規則ではない。

### 3.2 Reasoning

必要な最小レベルから開始する。

```text
Low -> Medium -> High
```

* **Low** — routine、docs-only、小規模・決定論的変更、既存patternに従う実装
* **Medium** — 複数仕様の整合確認、境界解釈、GO / NO-GO、公開契約影響、非自明なdebugging
* **High** — crash / persistence consistency、retry safety、複雑なstate transition、conflict / partial-failure、複数安全境界

Route Cは少なくともMedium相当とする。
明示的なreasoning設定がない環境では、同等のreview depthを分析・検証で確保する。
High / maximumを既定にしない。

### 3.3 Speed

Speedは運用設定として扱い、taskの正確性・verification・安全境界を弱めない範囲で選択する。
判断密度が高い作業では速度より正確性を優先する。

### 3.4 Purpose-First Execution Selection

実行先は、次の順序で選択する。

```text
Purpose
  -> Role / Target
  -> Execution Mode / Context
  -> Reasoning Depth
  -> Token Profile
```

製品surface、reasoning、token効率から先に実行先を決めない。

#### Step 1 — Purposeからroleを決める

| Execution purpose | Primary role / target | Typical reasoning | Token profile |
|---|---|---|---|
| control / decision / authorization coordination | Chat | Medium or higher when judgment is material | Low–Medium |
| investigation / cross-document review / evidence reduction | Work | Medium; High only when materially necessary | Medium–High |
| repository implementation / repository verification | Codex | Low for routine; Medium+ when non-trivial | Low–Medium to Medium |
| exact authorized interactive technical operation | User Interactive Executor | Packet-defined | Packet-defined |
| human-authority / privileged interactive operation | User | N/A | N/A |

1 execution stepの`Target Executor`には、Work、Codex、User Interactive Executorのいずれか一つだけを指定する。Chatはcontrol plane、Userはhuman authority planeとして扱う。

User Interactive Technical Executionは、`Target Executor: User Interactive Executor`のstepで、Userが指定されたinteractive execution contextを通じてexact authorized technical operationを実行するmechanismである。新しいRouteではなく、Route A / B / Cを変更しない。

このmechanismは、packetに記載されたexact operation、execution context、Authorization Basis、authorized use count、consumption boundaryで制御する。利用可能性、interactiveであること、prior successから、別operation、別context、別use、別stepへのauthorization portabilityを推定しない。不明または曖昧な場合はexecution前にSAFE-STOPする。

#### Step 2 — Codexを選んだ場合にCodex Execution Routeを決める

| Codex Execution Route | 選択目的 | 典型的に重視するもの |
|---|---|---|
| **Codex Cloud** | self-containedなrepository taskを隔離・委任して実行したい | sandbox/isolation、再現可能な環境、非対話的な委任 |
| **VS Code Codex LOCAL** | 現在のlocal workspaceとVS Code Codex contextを使い、Userが近接してreviewしながら進めたい | local files/tooling、IDE context、interactive supervision |

`Target Executor: Codex`のpacketは、`Codex Execution Route: VS Code Codex LOCAL | Codex Cloud`からexactly oneを必須指定する。genericな`Codex`だけの指定はincompleteとしてdelegation前にSAFE-STOPする。この選択はauthorizationを発生させず、実際のcapabilityはexecution contextごとに確認する。

#### Step 3 — Reasoningを独立して選ぶ

* Routine repository work: **Codex / Low**
* Non-trivial repository work: **Codex / Medium**
* Cross-document investigation: **Work / Medium**
* Complex cross-document or safety analysis: **Work / High only when necessary**
* Governance / authorization decision: **Chat / Medium or higher**

#### Step 4 — Token効率を最適化する

* Prefer direct **Chat -> Codex** when Work would not materially improve correctness or safety.
* Use Work when cross-document reading, evidence reduction, or independent analysis materially improves the next decision or instruction packet.
* Use the Codex execution mode that provides the required context with the least unnecessary transfer or duplication.
* Do not choose Work, Codex Cloud, VS Code Codex LOCAL, User Interactive Executor, or a higher reasoning level merely because more capability is available.
* `Token profile` is a relative planning heuristic, not a quota or completion criterion.

---

## 4. Route

### Route A — Normal Development

対象:

* 小規模実装
* bug fix
* Focused Test追加
* docs-only更新
* 既存patternに従う変更

原則: 最小探索、Codex単独委任を基本、Focused Verification優先。

### Route B — Investigation / Cross-Document Review

対象:

* 大規模調査
* 複数文書の整合確認
* repository横断分析
* 実装前evidence収集

原則: 1 execution stepにつきTarget ExecutorはWork、Codex、User Interactive Executorのいずれか一つ。WorkからCodexへ直接hand-offしない。

### Route C — Governance / Authorization / SAFE-STOP

対象:

* GO / NO-GO
* SAFE-STOP
* security / release / evidence判断
* authorization確認
* credential / execution-context境界

原則: Medium以上。Current Valid StateとAuthorization Basisを明示し、不明・期限切れ・消費済み・再利用不能なevidenceはfail closed。

Route選択は既存のNO-GO / SAFE-STOPを解除しない。

---

## 5. 役割

### Chat

* Route選択
* current valid state圧縮
* instruction packet作成
* Work / Codex結果評価
* authorization gate管理
* Userへのmanual Git handoff

### Work

* 委任された調査
* 文書横断review
* evidence整理
* docs-only分析

Workは次stepを承認せず、Codexへ直接hand-offしない。

### Codex

目的: **repository execution**。

* 委任されたrepository作業
* 実装・test追加
* authorized verification
* authorized read-only Git inspection
* 結果返却

CodexをTarget Executorとするpacketは、`Codex Execution Route`に`VS Code Codex LOCAL`または`Codex Cloud`の一方を必ず明示する。指定がなければdelegation前にSAFE-STOPする。Codexは設計・認可を推測で補わない。

### User Interactive Executor

目的: packetで厳密に限定された **User Interactive Technical Execution**。

* exact authorized technical operationのみ実行
* packet指定のinteractive execution contextのみ使用
* authorized use countとconsumption boundaryを遵守
* required evidenceをChatへ返却
* authorization判断、scope拡張、authorization portabilityの推定を行わない
* Git inspection、Git mutation、authenticated Git operationsを行わない

User Interactive Executorは既存のexecution-context safety rejectionを迂回しない。特にSchema-4 Experiment executionに対するCodex LOCAL safety rejectionとNO-GOは、別途解決・認可されない限り維持する。

### User

* 人間判断を要する承認
* Git mutation
* authenticated Git operations
* 必要なinteractive-context操作

---

## 6. Standard Instruction Packet

```text
Task:
Route:
Target Executor:
Codex Execution Route:  # Target Executor = Codexの場合: VS Code Codex LOCAL | Codex Cloud

Current Valid State:
Authorization Basis:

Execution Route:
  Chat:
  Work:
  Codex:
  User Interactive Executor:
  User:

Execution Context:
Scope In:
Scope Out:
Requirements:
Authorized Actions:
Prohibited Actions:
Verification:
Stop Conditions:
Return Format:
```

記述ルール:

* `Task` — 1 execution stepで達成する1目的
* `Target Executor` — Work、Codex、User Interactive Executorのいずれか一つのみ
* `Codex Execution Route` — Target ExecutorがCodexなら`VS Code Codex LOCAL`または`Codex Cloud`の一方を必須指定。genericな`Codex`のみはincompleteとしてSAFE-STOP
* `Current Valid State` — 次stepに必要な現行状態のみ。superseded / stale stateを含めない
* `Authorization Basis` — authorizer、exact operation、scope、validity、authorized use count、consumption boundary、unused / consumed / expired / non-reusableを必要に応じて明示。不要なら`Not required for this step`
* `Execution Route` — 未使用roleは`Not used`
* `Execution Context` — authorized operationのexecution contextを特定し、identity / repository / profile / sandboxが結果へ影響するときは明示。不要なら`Not applicable`
* `Scope Out` — 関連するNO-GO / SAFE-STOPを必要に応じて再掲
* `Authorized Actions` — 狭義に列挙。未記載operationを推定しない
* `Prohibited Actions` — task固有で誤実行riskが高い事項を明示。`AGENTS.md`全文を複製しない
* `Verification` — 必要checkのみ。verification authorityは別operationのauthorizationではない
* `Stop Conditions` — scope逸脱、contract変更、authorization不足などのreturn-to-Chat条件

---

## 7. Packetの簡略化

Standard headingsは維持するが、内容量はRouteに合わせる。

* **Route A** — 各fieldを必要最小限にする
* **Route B** — Current Valid State / Scope / Verification / Return Formatを具体化
* **Route C** — Current Valid State / Authorization Basis / applicable Execution Context / Scope Out / Prohibited Actions / Stop Conditionsを省略しない

Route Cでは簡潔さより認可境界を優先する。

---

## 8. Codex標準手順

1. Task / Scope / Authorized Actions / Stop Conditions確認
2. 必要なauthoritative sourceとaffected filesのみ確認
3. 必要ならauthorized read-only Git inspection
4. Focused change
5. Focused Verification
6. 必要なbroader verification
7. diff確認
8. 結果返却

同じauthoritative fileを理由なく繰り返し読まない。
repository-wide explorationは明示的に必要な場合に限定する。

---

## 9. Verification

標準順序:

1. Focused Test / Focused static check
2. Unit Test
3. 必要なIntegration Test
4. Release Build
5. Format Check
6. Diff Check

全taskで全項目を機械的に要求しない。
docs-onlyでは無関係なbuild未実行を理由に未完了としない。
task-specificに指定されたverificationは省略しない。

代表例:

```powershell
dotnet test <Solution> --configuration Release --filter "<Filter>"
dotnet test <Solution> --configuration Release
dotnet build <Solution> --configuration Release
dotnet format <Solution> --verify-no-changes
git diff --check
```

---

## 10. Git Manual Handoff

Git mutationとauthenticated Git operationsはUser routeへ渡す。
Work / Codexはpacket-authorized read-only inspectionのみ実行する。
User Interactive ExecutorにはGit authorityを付与せず、read-only Git inspectionも委任しない。

ChatからUserへの標準形式:

```text
Operation:
Repository:
Required Identity:
Preconditions:
Command:
Expected Result:
Stop Conditions:
Return Evidence:
```

`git add .`は標準使用せず、対象fileをliteral pathで列挙する。
stage、cached verification、commit、authentication、pushは独立gateとする。
前step成功を次stepのauthorizationとして再利用しない。

VMF固有のidentity、credential、split-context、`.git`境界の詳細は`AGENTS.md`を参照する。
既知境界を自動workaroundしない。

---

## 11. Work標準手順

Workは調査・文書横断分析・evidence整理に使用する。

Packetでは次を明確にする。

* 調査対象
* authoritative source
* Scope Out
* uncertaintyの扱い
* Return Format

Workはfact / inference / uncertaintyを区別して返す。
Chatは結果から次stepに必要なcurrent valid stateだけを抽出する。

---

## 12. Standard Return Format

```text
Result:
Evidence:
Changes:
Verification:
Uncertainties:
Stop Conditions Triggered:
Recommended Next Step:
```

`Recommended Next Step`はadvisoryでありauthorizationではない。
RoutineなRoute Aでは空sectionを省略してよい。

---

## 13. Chat Result Review

Chatはreturned resultをそのまま次stepへ転送せず、次を確認する。

* Task達成
* Scope逸脱
* Verification
* Uncertainty / Stop Conditions
* Authorization消費状況
* 次stepへ引き継ぐCurrent Valid State

確認後、必要情報だけを圧縮して新しいpacketを作成する。

---

## 14. モデル能力向上時の見直し

モデルまたはthinking機能が向上した場合、まず本Playbookを見直す。

主な見直し対象:

* 推奨model
* reasoning既定値
* Route別review depth
* Packetの冗長性
* 探索量
* Verification効率
* Return Format

モデル能力向上だけを理由に、Frozen protection、authorization、Git、credential、SAFE-STOP、external-service safetyを変更しない。
恒久的な安全境界変更が必要な場合のみ`AGENTS.md`改訂を検討する。

---

## 15. 保守方針

本Playbookは運用文書として、モデル・製品機能・実行環境に応じて更新してよい。

更新時は:

* `AGENTS.md`と矛盾させない
* 恒久規則を重複再定義しない
* obsoleteなモデル名・機能を残さない
* 不要になった手順を削除する
* safetyを維持しながらinstruction overheadを減らす

目的はAIを細かく縛ることではなく、現在のモデル能力を安全かつ効率的にVMFへ適用することである。

VMF Ownerの承認権限、兼務条件および独立レビューの取扱いは、AGENTS.md第10章の『VMF Owner Authorization and Independent Review Policy』節に従う。上位規程、P9 HOLDおよび技術的実行禁止条件を優先する。
