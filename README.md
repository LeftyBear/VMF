# Build.xlam

Build.xlam は、**VMF v1.0** および **VBAアプリケーション開発正典 v2.0** に基づいて開発する Excel VBA アドインプロジェクトです。

本プロジェクトは、保守性・拡張性・テスト容易性を重視したアーキテクチャを採用し、GitHub を唯一の正本（Single Source of Truth）として管理します。

---

## アーキテクチャ

```text
UI
↑
Build
↑
VMF
↑
Infrastructure
↑
Common
```

依存関係は一方向のみとし、循環参照は禁止します。

---

## 開発環境

* Language : VBA (Excel)
* IDE : Visual Studio Code
* Version Control : Git / GitHub
* AI Assistant : ChatGPT / OpenAI Codex

---

## ディレクトリ構成

```text
src/
spec/
docs/
tests/
prompt/
```

---

## 開発ルール

* VMF v1.0 の設計は凍結し、設計変更は行いません。
* 改善案は **VMF v1.1 Candidate** として管理します。
* GitHub を唯一の正本として運用します。
* 原則として **1ファイル完成 = 1コミット** とします。

---

## ドキュメント

* `spec/` : 仕様書
* `docs/` : 開発ドキュメント
* `prompt/` : AI プロンプト
* `tests/` : テスト

---

## ライセンス

プロジェクト方針に従って決定します。
