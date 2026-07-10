# VMF

VMF、Build.xlam、およびVMFを利用するアプリケーションを管理するリポジトリです。

Build v1.0.2とVMF v1.0は正式リリース済みであり、設計変更は候補文書として分離します。

## Architecture

```text
UI
↓
Build
↓
VMF
↓
Infrastructure
↓
Common
```

依存関係は一方向とし、循環参照を禁止します。

## Directory Structure

```text
src/            共通基盤、VMF、Build、UIのソース
tests/          unit / integration テスト
specs/          Build / VMFの正式仕様
candidates/     将来版の変更候補
docs/           Build、開発、リリース文書
templates/      Buildが参照する生成テンプレート
tools/          ビルド、テスト、監査ツール
prompts/        AIプロンプト
assets/         静的資産
applications/   VMFを利用する実アプリケーション
dist/           生成成果物のみ
```

SchoolTimetableは `applications/SchoolTimetable/` で、アプリケーション固有のソース、テスト、文書を管理します。

正式版Build成果物は `dist/release/Build_v1.0.2/Build.xlam` に配置します。

## Development Rules

- VMF v1.0およびBuild v1.0.2の設計は変更しません。
- 将来の改善は `candidates/` に記録します。
- 仕様、実装、テスト、生成成果物を混在させません。
- GitHubをSingle Source of Truthとして運用します。

## License

[LICENSE](LICENSE)を参照してください。
