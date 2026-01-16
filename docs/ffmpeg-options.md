# FFmpeg オプション設計（ffmpeg-options）

本ドキュメントでは、**現在 ezFFmpeg に実装されている FFmpeg / FFprobe 引数生成処理**を基に、
どのオプションを・どの責務で・どのような設計思想で扱っているかを整理します。

⚠️ **ここに記載されていない機能・オプションは「未実装（今後の予定）」として扱います。**

---

## 対象クラス

- `FFmpegArgumentBuilder`
- `FFprobeArgumentBuilder`

これらは **FFmpeg / FFprobe の引数文字列のみを生成する責務**を持ち、
実行や UI には依存しません。

---

## 設計方針（実装ベース）

### 基本原則

- 引数生成に専念し、副作用を持たない
- Domain Model（Profile / Encoder / Quality）を入力とする
- 不正な組み合わせは「生成しない」ことで防ぐ
- FFmpeg と FFprobe は完全に分離する

---

## Global オプション

### 実装されているオプション

| オプション | 用途 |
|---|---|
| `-hide_banner` | 起動時バナー非表示 |
| `-loglevel quiet / error` | ログ抑制 |
| `-y` / `-n` | 出力上書き制御 |

### 設計意図

- ユーザー選択の価値が低いため UI からは操作不可
- Builder が暗黙的に付与

---

## Input オプション

### 実装されているオプション

| オプション | 用途 |
|---|---|
| `-i` | 入力ファイル指定 |

### 実装例

```text
-i "input.mp4"
```

### 設計意図

- 現在は **単一入力のみ対応**
- マルチ入力は今後の拡張対象

---

## Video ストリーム関連

### Mapping

| オプション | 用途 |
|---|---|
| `-map 0:v:0` | 最初の映像ストリームを使用 |
| `-vn` | 映像無効 |

### Encoder 指定

```text
-c:v {VideoEncoder}
```

- `VideoEncoders.GetEncoder()` により能力を取得
- COPY は `IsCopy` フラグで判定

---

### Encoder 動作モード

| エンコーダ種別 | オプション |
|---|---|
| AMF | `-quality` |
| その他 | `-preset` |

COPY の場合は **付与されない**。

---

### 品質制御（実装済）

| QualityType | オプション |
|---|---|
| CRF | `-crf` |
| CQ (QSV) | `-global_quality` |
| CQ (その他) | `-cq` |

※ CBR / VBR / maxrate 等は **未実装（今後対応予定）**

---

### 解像度制御

#### 実装内容

- ソースとターゲットのアスペクト比を GCD で比較
- 同一比率 → `-s WxH`
- 異なる比率 → `-vf scale=...,setsar=1`

#### 使用オプション

| オプション | 用途 |
|---|---|
| `-s` | 単純リサイズ |
| `-vf scale` | アスペクト保持スケール |
| `setsar=1` | SAR 正規化 |

---

### フレームレート制御

#### 実装されているモード

| モード | オプション |
|---|---|
| Source | 変更なし |
| CFR | `-r` + `-vsync cfr` |
| VFR | `-vsync vfr` |

※ フレーム補間・間引き制御は未実装

---

## Audio ストリーム関連

### Mapping

| オプション | 用途 |
|---|---|
| `-map 0:a:0` | 最初の音声ストリーム |
| `-an` | 音声無効 |

### Encoder 指定

```text
-c:a {AudioEncoder}
```

### ビットレート制御

| 条件 | オプション |
|---|---|
| 非 COPY & Source 以外 | `-b:a` |

サンプルレート・チャンネル数指定は **未実装**。

---

## Output オプション

| オプション | 用途 |
|---|---|
| `-y` | 上書き許可 |
| `-n` | 上書き禁止 |

出力フォーマット指定（`-f`）は現在 **未使用**。

---

## テスト用引数生成

### Audio エンコーダテスト

```text
-f lavfi -i anullsrc -t 0.1 -c:a {encoder} -f null NUL
```

### Video エンコーダテスト

```text
-f lavfi -i nullsrc -t 1 -c:v {encoder} -f null -
```

### 設計意図

- 実ファイル不要
- エンコーダ対応可否の事前検証

---

## サムネイル生成

### 使用オプション

| オプション | 用途 |
|---|---|
| `-ss 00:00:01` | 取得位置 |
| `-vframes 1` | 1 フレーム |
| `-vf scale` | サイズ調整 |
| `-q:v 2` | JPEG 品質 |
| `-update 1` | 上書き |

---

## FFprobe 引数設計

### 実装されているオプション

```text
-hide_banner -v error -i "file" -show_streams -of json
```

### 目的

- ストリーム構成取得
- COPY 可否判定
- 解像度・コーデック情報取得

`-show_format` は **未実装（今後予定）**。

---

## 未実装（今後の予定）

- マルチ入力対応
- 複数ストリーム Mapping
- Filter の Model 化
- CBR / VBR / maxrate 制御
- OutputFormat 依存オプション
- HDR / 色空間指定

---

## まとめ

この ffmpeg-options 設計は、

- **実装されている事実ベース**で整理され
- 未実装機能を明確に区別し
- 将来の拡張指針としても利用可能

なドキュメントであることを目的としています。
