# Publisher v1.0 Architecture Specification (Frozen Edition)

## Volume P1: Markdown Engine

Status: Frozen

Version: 1.0

## P1.1 Purpose

Markdown EngineはMarkdown文書を出力形式に依存しないDocument ASTへ変換する。

## P1.2 Responsibilities

字句解析、構文解析、Document AST生成、インライン解析、構文検証、診断生成を担当する。Google Docs API、PDF生成、相互参照解決、TOC生成は担当しない。

## P1.3 Architecture

Markdown → Tokenizer → Parser → Document AST

## P1.4 Components

MarkdownTokenizer、MarkdownParser、DocumentAstBuilder、InlineParser、MarkdownValidator、DiagnosticCollector。

## P1.5 Document AST

HeadingNode、ParagraphNode、ListNode、TableNode、ImageNode、CodeBlockNode、Metadataで構成する。

## P1.6 Inline Model

TextNode、StrongNode、EmphasisNode、InlineCodeNode、LinkNodeを共通インラインモデルとする。

## P1.7 Supported Markdown

見出し、段落、番号付き・箇条書き・ネストリスト、表、画像、コードブロック、太字、斜体、インラインコード、リンクを正式対応とする。

## P1.8 Parser Principles

Output Independent、Stable AST、Lossless Structure、Extensibleを満たす。

## P1.9 Diagnostics

MarkdownDiagnosticはCode、Severity、Location、Messageを持つ。Warning以上をPublish Reportへ含める。

## P1.10 Renderer Boundary

Markdown EngineはRendererを持たず、Document ASTのみを成果物とする。

## P1.11 Design Constraints

WorkspaceおよびManifestを変更せず、Google Docs APIを呼ばず、診断以外の副作用を持たない。

## P1.12 Conformance

新構文追加時はAST仕様を拡張し、既存構文との互換性を維持する。
