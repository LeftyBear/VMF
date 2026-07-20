# Publisher v1.0 Architecture Specification (Frozen Edition)

## Volume P0: Overview

Status: Frozen

Version: 1.0

Component: VMF Studio Publisher

## P0.1 Purpose

Publisherは、VMF Studioにおける文書公開基盤である。Markdown文書を解析・コンパイル・検証し、Google Docs、PDFおよび将来の出力形式へ一貫して公開する。Publisherは単なる変換ツールではなく、宣言的なドキュメントコンパイラとして設計する。

## P0.2 Scope

Markdown解析、Document AST、Publication AST、Compilation、Cross Reference、Generated Content、Publish Manifest、Update Engine、Google Docs公開、PDF生成、VMF Studio統合を対象とする。

## P0.3 Goals

Deterministic、Idempotent、Declarative、Extensible、Output Independent、Workspace Nativeを設計目標とする。

## P0.4 Non-Goals

WYSIWYG Editor、Markdown編集機能、Google Docs手動編集支援、Git管理、EPUB・HTML出力、AI文書生成はv1.0の対象外とする。

## P0.5 Design Principles

Parse Once, Render Many／AST First／Manifest Driven／Separation of Responsibilities／Managed Region／Stable Identity／Validation Before Publishを基本原則とする。

## P0.6 Architecture Overview

Workspace → Publish Manifest → Markdown Parser → Document AST → Compilation Engine → Publication AST → Renderer Factory → Publish Result

## P0.7 Core Components

Markdown Engine、Update Engine、Publish Manifest、Compilation Engine、Renderer Factory、Publisher Serviceで構成する。

## P0.8 Terminology

Workspace、Document、Volume、Publication、Document AST、Publication AST、Generated Content、Renderer、Publish Manifest、Managed Regionを標準用語とする。

## P0.9 References

VMF Constitution、VMF Studio v2.0 Architecture Specification、Document／Workflow／Service／Event／Runtime／Plugin／Observability Architectureを参照する。

## P0.10 Conformance

実装は本仕様に適合しなければならない。変更はVMFのADRおよびRFCプロセスに従う。
