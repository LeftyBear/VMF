# Build.xlam 開発正典 v1.0

Version : 1.0
Status  : Frozen
Scope   : Build.xlam
Depends : Canon_v2.0.md

---

# 第1章 目的

Build.xlam は、Blueprint・Manifest・Template に基づき、
VBAアプリケーションを自動生成するための開発基盤である。

Build.xlam は単なるコード生成ツールではない。

Build.xlam は Manifest を解釈し、
プロジェクトを構築する Generator Engine である。

---

# 第2章 基本理念

Build.xlam 自身も Canon v2.0 に従う。

Build.xlam は Canon のリファレンス実装である。

---

# 第3章 開発原則

## Rule 1 Blueprint First

設計を承認・凍結した後に実装を開始しなければならない。

実装が設計を変更してはならない。

設計変更は次期バージョンへ分離する。

---

## Rule 2 Manifest Driven

生成対象は Manifest で定義しなければならない。

Generator に生成対象を直接記述してはならない。

---

## Rule 3 Template Driven

生成コードは Template に定義しなければならない。

Generator は Template の内容を知ってはならない。

---

## Rule 4 Context Driven

生成時の可変情報は GenerateContext に集約しなければならない。

Generator の引数追加による設計変更を行ってはならない。

---

## Rule 5 Single Generate Engine

生成エンジンは一つだけ存在する。

全ての生成処理はこの Generator を利用しなければならない。

---

## Rule 6 Composition

大きな生成は小さな生成の組み合わせで構成しなければならない。

GenerateProject
    ↓
GenerateLayer
    ↓
GenerateComponent

---

## Rule 7 Single Source of Truth

Blueprint を唯一の設計情報とする。

Manifest

Template

Generated Code

は Blueprint から導かれる成果物である。

---

## Rule 8 Self Hosting

Build.xlam 自身も Build により生成可能であることを目標とする。

Build は Build を育てる。

---

# 第4章 アーキテクチャ

Presentation

↓

Application

↓

Composition Root

↓

Infrastructure

    Manifest Provider

    Template Provider

    Token Replacer

    Generator

---

# 第5章 Generator Pipeline

Blueprint

↓

Manifest

↓

Template

↓

GenerateContext

↓

Token Replace

↓

Generator

↓

VBProject

---

# 第6章 責務

Blueprint

設計のみ担当する。

Manifest

生成対象のみ定義する。

Template

コードの雛形のみ定義する。

GenerateContext

可変情報のみ保持する。

Generator

VBAProjectへの反映のみ担当する。

---

# 第7章 Version Policy

Build v1.0

Manifest Driven Generator

Build v1.1

Blueprint Parser

Manifest Auto Generation

Build v2.0

Self Hosting

Build generates Build.

---

# 第8章 v1.1 Candidate

Blueprint Parser

Blueprint → Manifest 自動生成

Template Validation

Manifest Validation

Generate Preview

Incremental Generate

Ribbon UI

Visual Designer

---

# 第9章 Design Goal

Build.xlam は

Blueprint

↓

Manifest

↓

Application

を自動生成する開発基盤である。

コードを書くことよりも

設計を書くこと

を優先する。

これを Build.xlam の最終目標とする。
