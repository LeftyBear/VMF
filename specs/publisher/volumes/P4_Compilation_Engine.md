# Publisher v1.0 Architecture Specification (Frozen Edition)

## Volume P4: Compilation Engine

Status: Frozen

Version: 1.0

## P4.1 Purpose

複数のDocument ASTを統合し、出力形式に依存しないPublication ASTを生成する。

## P4.2 Responsibilities

Volume Composition、Publication AST、Cross Reference、Generated Content、Publication Validation、Compilation Resultを担当する。

## P4.3 Architecture

Document ASTs → Volume Composition → Publication AST → Cross Reference Resolver → Generated Content Builder → Publication Validation → Compilation Result

## P4.4 Compilation Model

CompilationはId、PublicationId、Documents、Options、Metadataを持ち、Manifestで構成される。

## P4.5 Publication AST

Generated Content、Front Matter、Volumes、Back Matterで構成する。

## P4.6 Volume Composition

Manifest順序、Volume境界、Document ID、Block Identityを保持する。見出しレベル補正とVolume単位のAnchor名前空間を適用する。

## P4.7 Generated Content

Cover、TOC、将来の図表一覧・Index・Glossary・CustomをGeneratedContentNodeとして管理し、Managed Regionに属させる。

## P4.8 TOC

Publication ASTのHeadingNodeから生成し、Anchor IDを参照する。既定配置はAfterFrontMatter。毎回単一管理ブロックとして再生成する。

## P4.9 Cross Reference

同一文書、Volume間、外部文書、外部URLをDocument ID優先で解決する。未解決時の既定値はWarn、Strict ModeではError。

## P4.10 Pipeline Contract

各段階はICompilationStage&lt;TInput,TOutput&gt;に従い、Validate、Execute、Verifyを提供する。

## P4.11 Compilation Result

Publication AST、Reference Map、Generated Content、Validation Report、Statistics、Publish Contextを含み、Rendererへの唯一の入力とする。

## P4.12 Conformance

Rendererへ依存せず、Volume境界とIdentityを維持し、Publication Validationを最終品質ゲートとする。
