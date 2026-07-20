# Publisher v1.0 Architecture Specification (Frozen Edition)

## Volume P6: Extension Points

Status: Frozen

Version: 1.0

## P6.1 Purpose

Publisher v1.0の公式な拡張ポイントを定義し、Coreを変更せず機能を追加できるようにする。

## P6.2 Principles

公開Contractのみを利用し、Publication ASTを破壊せず、Determinism、Idempotency、Validationを維持する。

## P6.3 Extension Categories

Renderer、Generated Content、Validator、Workflow Step、Output Target、Asset Provider、Diagnostics Providerを正式拡張点とする。

## P6.4 Renderer Extension

IRendererはValidate、Render、Verify、Publishを提供し、Publication ASTを変更してはならない。

## P6.5 Generated Content Extension

IGeneratedContentProviderはCanGenerateとGenerateを提供し、生成物をGeneratedContentNodeとして追加する。

## P6.6 Validator Extension

IValidator&lt;T&gt;により用語、文書規約、リンク、Markdownなどの品質ルールを追加する。

## P6.7 Workflow Step Extension

IPublishWorkflowStepはValidate、ExecuteAsync、VerifyAsync、CompensateAsyncを提供する。

## P6.8 Output and Asset Extensions

新しいOutput Targetは対応Rendererを提供する。IAssetProviderはResolve、Validate、Publishを提供する。

## P6.9 Diagnostics Extension

IDiagnosticsProviderはAnalyzeとReportを提供し、Studio Diagnosticsへ統合する。

## P6.10 Registration and Compatibility

Plugin ArchitectureとPlugin Registryを利用し、Manifest Schema、Publication AST、Renderer Contract、Validation Contractとの互換性を確認する。

## P6.11 Versioning and Security

ExtensionMetadataはId、Name、Version、PublisherVersion、Capabilitiesを持つ。Workspace外への無断アクセス、認証情報保存、Manifest改変、Job Context外処理を禁止する。

## P6.12 Conformance

拡張はCoreを置換せず、Contractを通じて機能を追加する。
