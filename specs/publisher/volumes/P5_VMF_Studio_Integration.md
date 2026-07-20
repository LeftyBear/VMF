# Publisher v1.0 Architecture Specification (Frozen Edition)

## Volume P5: VMF Studio Integration

Status: Frozen

Version: 1.0

## P5.1 Purpose

PublisherをVMF Studioの標準サービスとして統合する。

## P5.2 Architecture

Workspace、Command System、Workflow Engine、Job Manager、Diagnostics、Publisher Serviceを連携し、Publisher CoreはStudio UIへ依存しない。

## P5.3 Studio Commands

Publish、Publish Document、Publish Volume、Publish All、Compile、Validate、PreviewをIStudioCommandとして提供する。

## P5.4 Command Context

Workspace、Publish Manifest、Logger、Services、Progress、CancellationTokenを保持し、処理はPublisher Serviceへ委譲する。

## P5.5 Workspace Integration

標準構造はdocuments、assets、publish/publish.yaml、publish/state、output、logs。Manifest未検出時はPublisher機能のみ無効化する。ManifestとStateを分離し、出力はStaging経由で確定する。

## P5.6 Publish Workflow

Workflow Engine上でPrepare、Validate、Compile、Render、Publish、Verify、Commit Stateの7段階を実行する。Verify完了後のみStateを確定する。

## P5.7 Job Management

PublishJob、JobManager、ProgressReport、Job Event群を用いる。同一Workspaceおよび同一Google Documentへの同時Publishを禁止する。

## P5.8 Cancellation

Cancellation Tokenを全Stepへ伝播し、補償処理を行う。State Commit開始後はキャンセル不可とする。

## P5.9 Logging and Diagnostics

PublishLogEntry、PublishDiagnostic、PublishReportを標準モデルとする。エラーコードはPUB-{Category}-{Number}形式。秘密情報と本文全体をログへ出力しない。

## P5.10 Telemetry

任意かつ既定無効。処理時間、文書数、出力数、Renderer種別、エラーコードに限定する。

## P5.11 Conformance

Commandを唯一の実行入口、Workspaceを唯一の作業単位とし、Studio共通基盤を利用する。
