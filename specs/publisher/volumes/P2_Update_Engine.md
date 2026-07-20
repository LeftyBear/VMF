# Publisher v1.0 Architecture Specification (Frozen Edition)

## Volume P2: Update Engine

Status: Frozen

Version: 1.0

## P2.1 Purpose

既存公開文書を安全・再現可能・冪等に更新する。

## P2.2 Responsibilities

Document Identity、状態判定、Diff Plan、Managed Region、Anchor、Transaction、Publish Stateを管理する。

## P2.3 Components

IdentityResolver、ManagedRegionDetector、DiffEngine、AnchorResolver、TransactionManager、SnapshotManager、PublishStateManager。

## P2.4 Document Identity

PublicationId、DocumentId、GoogleDocumentId、Stateを持ち、GoogleDocumentIdを公開先の唯一の識別子とする。

## P2.5 Document State

New、Existing、Archived、Missing。Missingの既定動作はFail。

## P2.6 Managed Region

Document Header、Managed Region、User Regionを分離し、PublisherはManaged Regionのみ更新する。

## P2.7 Diff Engine

Insert、Update、Delete、Move、NoChangeをブロック単位で算出する。MoveはDelete＋Insertへ展開可能。

## P2.8 Block Identity

ExplicitId、GeneratedId、ContentHashの順で対応付ける。

## P2.9 Anchor Architecture

AnchorId、BlockId、DocumentIdを保持し、TOCとCross Referenceが参照する。

## P2.10 Transaction and Idempotency

PublishTransactionとPublishFingerprintを導入し、同一Fingerprintでは更新を省略する。Publish Stateは検証完了後のみ確定する。

## P2.11 Conflict and Recovery

ConflictPolicyの既定値はFail。更新前Snapshotを保持し、再開の既定値はRestartとする。

## P2.12 Conformance

User Regionを変更せず、安全性を優先し、不整合時は既定ポリシーに従う。
