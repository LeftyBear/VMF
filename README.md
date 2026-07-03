# VMF

Build.xlam の実装プロジェクトです。

本プロジェクトは、**保守性・拡張性・テスト容易性**を重視したアーキテクチャを採用し、GitHub を唯一の正本（Single Source of Truth）として管理します。

---

## Architecture

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

依存関係は一方向のみとし、循環参照を禁止します。

---

## Development Environment

| Item            | Value                  |
| --------------- | ---------------------- |
| Language        | VBA (Excel)            |
| IDE             | Visual Studio Code     |
| Version Control | Git / GitHub           |
| AI Assistant    | ChatGPT / OpenAI Codex |

---

## Directory Structure

```text
src/
spec/
docs/
tests/
prompt/
```

---

## Development Rules

* VMF v1.0 の設計は凍結し、設計変更は行いません。
* 改善案は **VMF v1.1 Candidate** として管理します。
* GitHub を唯一の正本（Single Source of Truth）として運用します。
* 原則として **1ファイル完成 = 1コミット** とします。

---

## Documentation

| Directory | Description |
| --------- | ----------- |
| `spec/`   | 仕様書         |
| `docs/`   | 開発ドキュメント    |
| `prompt/` | AI プロンプト    |
| `tests/`  | テスト         |

---

## License

ライセンスは、プロジェクト方針に従って決定します。
