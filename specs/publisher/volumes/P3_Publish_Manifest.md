# Publisher v1.0 Architecture Specification (Frozen Edition)

## Volume P3: Publish Manifest

Status: Frozen

Version: 1.0

## P3.1 Purpose

Publisherの動作を宣言的に定義する唯一の設定モデルとする。

## P3.2 Structure

Schema、Publication、Documents、Outputs、Assets、Update、Compilation、Validation、Metadataで構成する。正式形式はYAML、標準名はpublish.yaml。

## P3.3 Schema

schemaVersionを必須とし、未対応Major Versionは拒否する。

## P3.4 Publication Profile

Id、Title、Subtitle、Version、Language、Status、Authors、PublishedAtを持つ。publication.idは永続識別子とする。

## P3.5 Document Mapping

documentsはIDをキーとするマッピング形式とし、Workspace上のSourceと公開先を対応付ける。

## P3.6 Output Targets

Google Docs、PDF、将来のHTML・EPUB、Customを扱う。Parse Once, Render Manyを基本原則とし、IRendererとRenderer Factoryを使用する。

## P3.7 Update Policy

Mode=Differential、ConflictPolicy=Fail、MissingPolicy=Failを既定値とする。

## P3.8 Compilation

出版順序はCompilation側で一元管理し、TOC、Generated Content、統合オプションを定義する。

## P3.9 Validation

Manifest、Workspace、Document、Asset、Targetを段階的に検証する。ErrorはPublish禁止、Strict ModeではWarningをErrorへ昇格できる。

## P3.10 Assets

Root、FailurePolicy、Defaults、Overridesを持ち、Workspace外参照を禁止する。

## P3.11 Principles

宣言的、人間可読、Git差分可読、秘密情報非保持、Publisher設定の唯一の正本であることを保証する。

## P3.12 Conformance

独自拡張は互換性を維持し、本仕様の解釈を優先する。
