# ezFFmpeg アーキテクチャ

本ドキュメントでは、ezFFmpeg の内部構造・責務分離・設計意図について説明します。  
ezFFmpeg は **FFmpeg コマンド生成と実行を安全に扱うためのレイヤードアーキテクチャ**を採用しています。

---

## 全体構成

ezFFmpeg は大きく以下のレイヤーで構成されています。

```text
UI (View / ViewModel)
 ↓
Application / Helper
 ↓
Domain Model (Codec / Encoder / Format)
 ↓
Argument Builder
 ↓
FFmpeg Service
 ↓
FFmpeg / FFprobe (外部プロセス)
```

各レイヤーは **単一責務**を持ち、  
上位レイヤーは下位レイヤーの詳細を意識しません。

---

## レイヤー責務一覧

| レイヤー | 主な役割 |
|--------|--------|
| UI | ユーザー入力・表示 |
| ViewModel | 状態管理・選択制御 |
| Helper | UI とドメインの橋渡し |
| Domain Model | FFmpeg 知識のモデル化 |
| Argument Builder | コマンド生成 |
| Service | プロセス実行 |

---

## Domain Model 層

### 目的

FFmpeg の概念（Codec / Encoder / Format）を  
**型安全かつ検証可能なオブジェクトとして表現**することが目的です。

---

### Codec

#### 概要

Codec は「データ形式」を表します。

- H.264
- HEVC
- AAC
- MP3

#### 実装例（概念）

- `VideoCodec`
- `AudioCodec`
- `ICodec`

#### 責務

- コーデックの識別
- 対応フォーマット判定
- COPY 可否判定の基準情報

> Codec 自体は **実装方法を知らない**点が重要です。

---

### Encoder

#### 概要

Encoder は「実装方法」を表します。

- libx264
- h264_nvenc
- aac
- copy

#### 実装例

- `VideoEncoder`
- `AudioEncoder`
- `IEncoder`

#### 保持情報

- 対応 Codec
- GPU 使用有無
- 利用可能条件
- 推奨度

#### 設計ポイント

- COPY は特別な Encoder として定義
- GPU エンコーダも Encoder として同一視

---

### OutputFormat

#### 概要

出力コンテナ（mp4 / mkv / mp3 など）を表します。

#### 責務

- 対応 VideoCodec / AudioCodec の定義
- COPY 可否条件
- 推奨コーデック情報

OutputFormat は **「何が許可されるか」** を判断する中心的存在です。

---

## Helper 層

### 役割

UI で扱いやすい形に  
Domain Model を加工・制御します。

### 主な責務

- 使用可能 Encoder の絞り込み
- 推奨 Encoder の自動選択
- ComboBox / ListBox 用データ生成

### 例

- `SetVideoEncoders`
- `SetAudioEncoders`

> Helper 層は **UI 専用ロジック**を隔離するために存在します。

---

## Argument Builder 層

### 目的

FFmpeg コマンド文字列を  
**安全かつ再利用可能な形で構築**します。

---

### 設計方針

- 文字列連結を直接行わない
- 各オプションは小さな責務に分割
- FFmpeg / FFprobe を明確に区別

---

### 主なクラス

- `FFmpegArgumentBuilder`
- `FFprobeArgumentBuilder`

### 特徴

- Domain Model を入力として受け取る
- 不正な組み合わせは生成段階で防止
- テストが容易

---

## FFmpeg Service 層

### 役割

FFmpeg / FFprobe の **実行責務のみ**を担当します。

### 主な機能

- プロセス起動
- 標準出力 / エラー出力取得
- 非同期実行
- キャンセル対応

### 設計ポイント

- コマンド内容を解釈しない
- 実行結果を上位に返すだけ

---

## UI / ViewModel 層

### MVVM 採用理由

- 設定項目が多い
- 状態遷移が複雑
- 再利用性が高い

### ViewModel の責務

- 選択状態の保持
- Helper 呼び出し
- 実行トリガー制御

### View の責務

- 表示のみ
- ロジックを持たない

---

## COPY に関する設計

COPY は以下の理由で特別扱いされています。

- フォーマット依存
- 入力依存
- トラック構成依存

### ezFFmpeg の対応

- Encoder として明示的に定義
- 使用不可な場合は UI に表示しない
- 実行前に判定

---

## 拡張性

ezFFmpeg は以下の拡張を想定しています。

- 新 Codec の追加
- 新 GPU Encoder の追加
- 新 OutputFormat の追加
- CLI / Web API への展開

Domain Model を追加するだけで  
他レイヤーへの影響を最小化できます。

---

## 依存関係ルール

- UI → Helper → Domain
- Argument Builder → Domain
- Service は Domain を知らない

逆方向の依存は禁止されています。

---

## まとめ

ezFFmpeg は、

- FFmpeg の知識を **モデル化**
- コマンド生成を **責務分離**
- UI 利用を **前提設計**

とすることで、  
「壊れにくく、拡張しやすい FFmpeg 利用」を実現しています。
