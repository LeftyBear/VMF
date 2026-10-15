# VMF Local Work Playbook

## 1. 目的

本Playbookは、`AGENTS.md`の安全原則を日常のVMF開発へ適用するための簡潔な運用手順である。

標準運用は次の3者で構成する。

* **Chat** — 判断、方針決定、scope確認、結果評価
* **Local Work** — ローカル環境での調査、設計、実装、test、build、文書編集、検証
* **VS Code / User** — 差分確認およびGit変更操作

Codexは標準運用から外す。旧来のRoute A / B / C、Target Executor、Codex Execution Route、User Interactive Executor、承認消費回数管理は日常運用では使用しない。

恒久的な安全境界は`AGENTS.md`を正典とし、矛盾時は`AGENTS.md`を優先する。

## 2. 基本方針

* 1依頼1目的を基本とする
* 必要なauthoritative sourceとaffected filesを先に確認する
* Frozen仕様と公開contractを維持する
* requested scope内で最小変更を行う
* userの既存変更を保護する
* Focused Verificationから開始する
* testを弱めず、失敗や未確認事項を隠さない
* credential、外部service、実データを安全に扱う
* 変更はuncommittedのままUserへ返す
* 結果報告は短く、判断に必要なevidenceは省略しない

## 3. 標準ワークフロー

### 3.1 Chat

Chatは依頼を判断し、必要に応じて次を明確化する。

* Task / purpose
* Scope In / Scope Out
* authoritative source
* acceptance conditions
* verification
* stop conditions
* P9 SAFE-STOPまたは高リスク操作との接触有無

通常開発では、簡潔な依頼文で十分である。固定headingをすべて埋めるinstruction packetやexecutor選択は不要とする。

### 3.2 Local Work

Local Workはrequested scope内で次を一連の作業として実施できる。

1. 必要な調査と設計
2. 最小限の実装または文書編集
3. Focused Test / static check
4. 影響範囲に応じたUnit / Integration Test
5. 必要なbuild / format / lint
6. workspace diffの確認
7. 結果、変更、検証、残課題の返却

調査、編集、test、build、文書更新へ進むたびにUser承認を取り直さない。通常開発の依頼は、この一連のLocal Workを包括承認する。

ただし、scope拡張、Frozen仕様変更、materialなarchitecture判断、高リスク操作、またはP9 SAFE-STOP対象への接触が必要になった時点で停止してChatへ戻す。

### 3.3 VS Code / User

UserはVS Codeで変更差分を確認し、必要なGit操作を行う。

Local WorkはGit変更操作を実行しない。必要な場合は、対象file、operation、precondition、expected result、stop conditionを短いhandoffとして提示する。

## 4. 承認境界

### 4.1 包括承認される通常開発

依頼scope内では、次を都度承認なしで実行できる。

* read-only調査
* 設計と分析
* source / test / local fixture / documentationの編集
* focused test、unit test、必要なintegration test
* local build、format、lint、非破壊的な検証
* read-only Git inspectionと差分確認

### 4.2 個別承認が必要な操作

次は実行前にUserの個別承認を必要とする。

* stage、unstage、commit、amend、merge、rebase、reset、tag、push等のGit mutation
* authenticated Git operation
* release、publish、deploy、`dist/`またはrelease packageの変更
* 破壊的または復旧困難な操作
* 外部repository、live service、production data、shared remote documentの変更
* credential、authentication、account、ACL、ownership、sandbox、security controlの変更
* flagged / quarantined executableの実行
* NO-GO / SAFE-STOP対象のtechnical execution

個別承認はoperationとscopeを特定する。通常開発には承認のuse countやconsumption trackingを設けない。Userが明示的にsingle-useまたは期限を指定した場合だけ、その条件を記録する。

## 5. P9 SAFE-STOP

未解決のP9 security対象は通常開発から切り分け、**NO-GO / SAFE-STOP**を維持する。

影響対象を実行、変更、package化、公開、または依存しない通常開発は継続できる。通常開発の包括承認、他領域のtest成功、static review、文書作成、一般的なowner承認はP9 HOLDを解除しない。

次は独立した判断として扱う。

* design / document acceptance
* evidence sufficiency
* security-risk disposition
* SAFE-STOP release
* specific technical execution authorization
* release / publication authorization

P9対象またはcontainmentへ接触する場合はtechnical execution前に停止する。解除には、必要evidenceに基づく明示的なUser判断が必要である。

## 6. 変更と検証

Local Workは、authoritative source、既存architecture、naming、determinism、serialization、persistence、error contract、atomicity、retry safetyを維持する。

避ける事項:

* unrelated refactoring
* broad formatting
* speculative cleanup
* unnecessary abstraction
* future functionality
* dependency追加の推測実施
* test削除、Skip追加、assertion弱体化、fixtureによるdefect隠蔽

検証はriskと影響範囲に比例させる。

```text
Focused Test / Static Check
  -> Unit Test
  -> relevant Integration Test
  -> required Build / Format / Lint
  -> Diff Review
```

全taskで全項目を機械的に要求しない。docs-only taskでは無関係なbuildを要求しない。実施check、結果、warning / error、未実施checkと理由を報告する。

## 7. Git handoff

Git mutationとauthenticated Git operationはUserがVS Codeで行う。

必要なhandoffは次の最小形式とする。

```text
Operation:
Repository:
Files / Scope:
Preconditions:
Expected Result:
Stop Conditions:
```

対象fileは可能な限りliteral pathで示し、`git add .`を標準手順にしない。stage、cached diff review、commit、authentication、push、tag、releaseは、Userが明示的にまとめて承認しない限り別操作として扱う。

## 8. Credentialと外部service

Capabilityはauthorizationではない。credential依存操作では実process identity、profile、credential store、network、sandboxを確認する。

identity不明またはsplit contextが疑われる場合、credential creation / update / retrieval、browser authentication、credential-backed pushを停止する。別profileへの書込み、secret埋込み、別account選択、credential store変更、security control弱体化で回避しない。

外部integrationは既定で無効とし、個別承認がない限りFake、Stub、in-memory、dry-run、temporary directory、local fixtureを使用する。

## 9. 結果返却

通常は次の項目だけを返す。

```text
Result:
Changes:
Verification:
Remaining Issues:
Git / High-Risk Actions:
```

空の項目は省略できる。proposal、draft、requested approval、partial evidenceをacceptedまたはcompleteとして扱わない。

Chatは、達成状況、scope逸脱、verification、uncertainty、P9または高リスク境界を確認する。次の作業が通常開発scope内ならそのままLocal Workへ続行できる。新しい判断、高リスク操作、またはSAFE-STOP解除が必要ならUserへ戻す。

## 10. 保守方針

本Playbookは運用文書として更新できるが、モデルや製品機能の向上だけを理由にFrozen protection、最小変更、test integrity、user change protection、credential safety、Git境界、P9 SAFE-STOPを弱めない。

目的は、通常開発ではLocal Workへ十分な自律性を与え、Gitと高リスク操作ではUserの明示的統制を維持することである。
