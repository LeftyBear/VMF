# AI Development Rules

**Project:** Build.xlam
**Architecture:** VMF v1.0
**Development Standard:** VBAアプリケーション開発正典 v2.0

---

# Purpose

本プロジェクトでは、ChatGPT を開発パートナーとして利用し、一貫した品質で Build.xlam を開発・保守することを目的とする。

GitHub 上のソースコードを唯一の正本（Single Source of Truth）とし、AI が生成するコードも同じルールに従って管理する。

---

# Development Principles

## 1. VMF v1.0 は設計凍結

VMF v1.0 の設計は凍結済みとする。

実装中に改善案が見つかった場合でも、VMF v1.0 は変更しない。

改善案は **VMF v1.1 Candidate** として管理する。

---

## 2. 正典を最優先

すべての実装は

**VBAアプリケーション開発正典 v2.0**

に従う。

設計より実装を優先しない。

---

## 3. 単一責任原則

モジュール・クラス・メソッドは

**1つの責務のみ**

を持つ。

---

## 4. 一方向依存

依存関係は

Common

↓

Infrastructure

↓

VMF

↓

Build

↓

UI

の一方向のみとする。

循環参照は禁止する。

---

## 5. 契約ベース設計

公開 API のみを利用する。

内部実装へ直接依存しない。

---

# Coding Rules

## Option Explicit

すべてのモジュールで必須とする。

---

## Variant

必要最小限のみ使用する。

---

## Com_Result

Com_Result は

**Com_ResultFactory**

からのみ生成する。

New Com_Result を禁止する。

---

## Validator

Com_Validator は

* Validate 系（Boolean）
* Require 系（Com_Result）

の二層構成とする。

---

## Error

エラー番号は

ComErrNum

に従う。

---

## Member Pattern

クラスは標準として

Private Type TMember

Private This As TMember

を採用する。

---

# Git Rules

## Repository

GitHub を正本とする。

---

## Commit

1ファイル完成

=

1コミット

を原則とする。

例

* Add Com_Result
* Update Com_Path
* Fix Com_File

---

## Branch

通常開発

main

機能追加

feature/xxxx

---

## Push

コミット後は速やかに Push する。

ローカルのみで長期間保持しない。

---

# ChatGPT Rules

## Code Generation

ChatGPT は

**完成版コード**

のみ生成する。

未完成コードは禁止。

---

## Unit

生成単位

1ファイル

---

## Review

コードレビュー後に GitHub へ反映する。

---

## Explanation

コード生成時は必要最小限の説明とする。

---

## Improvement

設計変更は禁止。

改善案は

VMF v1.1 Candidate

として分離する。

---

# Folder Structure

```text
docs/
src/
tests/
prompt/
```

---

# Prompt Management

ChatGPT へ入力した主要プロンプトは

prompt/

フォルダで管理する。

必要に応じて履歴を残す。

---

# Documentation

設計変更・重要な判断は

docs/

へ記録する。

---

# Goal

Build.xlam を

* 保守しやすい
* 拡張しやすい
* テストしやすい
* 長期間維持できる

アーキテクチャで完成させる。
