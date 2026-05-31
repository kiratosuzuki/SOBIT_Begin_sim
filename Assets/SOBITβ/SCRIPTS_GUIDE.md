# SOBITβ スクリプト使用ガイド

新しいシーンを作成する際のセットアップ参考資料。

---

## 必須シングルトン（1つだけ配置）

### InitialRandomSettings
**役割**: ゲーム開始時のランダム初期値を決定する。

**アタッチ先**: 空の GameObjectに1つ

**Inspector 設定**:
| フィールド | 内容 |
|-----------|------|
| person | キッチン人物オブジェクト |
| kitchenPos0 / 1 | キッチン配置候補位置（Transform） |
| tray / trayPos0 / 1 | トレイと配置位置 |
| customer | 客席判定オブジェクト（MoveConditionをアタッチしたもの） |
| cusPos0〜3 | 客席配置候補位置（Transform） |
| robot | ロボットオブジェクト |
| foodsParent | 食品オブジェクト群の親（子に7個以上必要） |
| randomizeOnStart | true=ランダム / false=forcedIndexを使用 |

**外部参照用プロパティ**:
- `wantItem` / `wantItem2` / `wantItem3` — 注文商品名
- `soldOutItem` / `soldOutItem2` / `soldOutItem3` — 売り切れ商品名
- `dummyItem` — ダミー商品名
- `kitchenSide` — 「右」or「左」
- `customerTableNo` — 「テーブル1」〜「テーブル3」

---

### CompetitionSettings
**役割**: スキップ・難易度フラグの管理。PlayerPrefsで永続化。

**アタッチ先**: 空の GameObjectに1つ

**SettingKey 一覧**:
| キー | 内容 |
|-----|------|
| SkipDoorOpen | ドアオープンをスキップ |
| SkipRecognizeKitchen | キッチン認識をスキップ（位置をPos0固定） |
| SkipRecognizeSoldOut | 売り切れ認識をスキップ |
| SkipHandleSoldOut | 売り切れ対応をスキップ（SkipRecognizeSoldOut連動） |
| SkipObstacleAvoidance | 障害物回避をスキップ |
| DoorOpenBeta | ドアβ（ロボット向きランダム） |
| MoveToCustomerBeta | 客席移動β（位置ランダム） |
| RecognizeOrderTypeBeta | 注文認識β |

---

### FreeTaskManager
**役割**: タスクの達成・スコア加算・前提タスク管理。

**アタッチ先**: 空の GameObjectに1つ

**Inspector 設定（Tasksリスト）**:
| フィールド | 内容 |
|-----------|------|
| taskName | タスク識別名（他コンポーネントと一致させる） |
| score | アルファスコア |
| scoreBeta | ベータスコア（0の場合 score を使用） |
| isCompleted | 達成済みフラグ（読み取り専用） |

**主要メソッド**:
- `CompleteTask(taskName, isBeta)` — タスク達成＋スコア加算
- `SkipTask(taskName)` — スコアなしで達成済みにする
- `IsCompleted(taskName)` — 達成済みか確認
- `CanAttempt(prerequisites[])` — 前提タスクが全完了か確認

---

### ScoreManager
**役割**: スコアの加算とUI表示。

**アタッチ先**: 空の GameObjectに1つ

**Inspector 設定**:
| フィールド | 内容 |
|-----------|------|
| scoreText | TextMeshProUGUI への参照 |

---

## タスク条件コンポーネント

### TalkCondition
**役割**: 発話テキストのキーワードマッチでタスクを達成する。

**アタッチ先**: 任意の GameObject

**Inspector 設定**:
| フィールド | 内容 |
|-----------|------|
| taskName | FreeTaskManagerのタスク名と一致させる |
| keywordGroups | グループ間AND・グループ内OR のキーワードリスト |
| replyText | タスク達成時の返答（空欄なら返答なし） |
| hasDifficulty | β難易度対応を有効化 |
| difficultyKey | 対応するSettingKey |
| prerequisites | 前提タスク名リスト |
| onCompleted | 達成時に呼ぶ処理 |

**プレースホルダー（keywordGroups / replyText で使用可）**:
`{wantItem}` `{wantItem2}` `{wantItem3}`
`{soldOutItem}` `{soldOutItem2}` `{soldOutItem3}`
`{dummyItem}` `{kitchenSide}` `{customerTable}`

**例（売り切れ質問）**:
```
Group0: ["うりきれ", "売り切れ"]
Group1: ["何", "なに", "教えて"]
```

---

### MoveCondition
**役割**: ロボットがトリガーに入るとタスクを達成する。

**アタッチ先**: Collider(IsTrigger=ON) を持つ GameObject

**Inspector 設定**:
| フィールド | 内容 |
|-----------|------|
| taskName | タスク名 |
| robotTag | ロボットのタグ（デフォルト "Robot"） |
| hasDifficulty | β対応 |
| difficultyKey | SettingKey |
| prerequisites | 前提タスク名 |
| onCompleted | 達成時イベント |

---

### TaskTalkRouter
**役割**: WebSocketから受信したテキストをシーン内の全TalkConditionに流す。

**アタッチ先**: WebSocketServerUnity と同じ GameObject（または任意）

**Inspector 設定**:
- WebSocketServerUnity の `talkGoal` フィールドにドラッグ

---

## ランダム対応コンポーネント

### QRSwapper
**役割**: InitialRandomSettingsの食品名と一致する子オブジェクトをアクティブにする。

**アタッチ先**: 子オブジェクトに食品名と同名のオブジェクトを持つ GameObject

**子オブジェクト要件**:
- 名前 = foodsParentの子と完全一致
- tag = "scan"（DoScanで検出させる場合）
- 初期状態 = inactive

**主要メソッド（onCompletedから呼ぶ）**:
- `ShowOrder()` / `ShowOrder2()` / `ShowOrder3()`
- `ShowSoldOut()` / `ShowSoldOut2()` / `ShowSoldOut3()`
- `ShowCurrentTable()`
- `HideAll()`

**Inspector 設定**:
| フィールド | 内容 |
|-----------|------|
| delayMin / delayMax | 表示までのランダム遅延（秒） |

---

### TableManager
**役割**: シーン上の静的テーブルQRオブジェクトをcusidxに応じて表示/非表示。

**アタッチ先**: 任意の GameObject（シングルトン）

**Inspector 設定**:
| フィールド | 内容 |
|-----------|------|
| table1 / table2 / table3 | 各テーブルのGameObject |

**主要メソッド**:
- `HideCurrentTable()` — 現在の客席テーブルだけ非表示
- `ShowCurrentTable()` — 現在の客席テーブルだけ表示
- `ShowAll()` / `HideAll()`
- `ShowTable1~3()` / `HideTable1~3()`

---

### MoveOnTable4
**役割**: cusidx==3（テーブル4）のときだけオブジェクトをcusPos3に移動する。

**アタッチ先**: 移動させたいオブジェクト本体

**設定不要**（Inspector フィールドなし）

---

### SettingSkipper
**役割**: 指定SettingKeyがtrueのとき、タスクをスキップしオブジェクトを無効化する。

**アタッチ先**: 任意の GameObject

**Inspector 設定**:
| フィールド | 内容 |
|-----------|------|
| key | 対象のSettingKey |
| taskNamesToSkip | スキップするタスク名リスト |
| objectsToDisable | 無効化するオブジェクトリスト |

---

## WebSocket通信

### WebSocketServerUnity
**役割**: Scratchからのコマンドを受信してロボットを操作する。

**アタッチ先**: 任意の GameObject（シーンに1つ）

**Inspector 設定**:
| フィールド | 内容 |
|-----------|------|
| baseFootprint | ロボットのベースTransform |
| robotCamera | ロボットのカメラ |
| talkGoal | TaskTalkRouterをドラッグ |
| gripper | HsrSmoothGripper |
| bubble | SpeechBubble |

**受信コマンド**:
| コマンド | 動作 |
|---------|------|
| `move:100` | 100cm前進 |
| `turn:90` | 右90度回転（負数で左） |
| `talk:テキスト` | 発話・タスク判定 |
| `open` / `close` | グリッパー開閉 |

**WebSocketポート**: 8080

---

## UI関連

### SettingToggleBinder
**役割**: UIトグルとCompetitionSettingsのフラグを双方向バインド。

**アタッチ先**: Toggle コンポーネントと同じ GameObject

**Inspector 設定**:
- `key` → 対応するSettingKey をドロップダウンで選択

---

### PageNavigator
**役割**: 複数ページのUI切替（ループ対応）。

**アタッチ先**: パネル等の親オブジェクト

**Inspector 設定**:
- `pages` → Page1〜3のGameObjectをリストに追加

**ボタン配線**:
- Next ボタン → `PageNavigator.NextPage()`
- Prev ボタン → `PageNavigator.PrevPage()`

---

### SettingsPanel
**役割**: 設定パネルの開閉。

**アタッチ先**: 任意の GameObject

---

### CameraSpeedSettings
**役割**: FreeCameraControllerの速度をスライダーで設定。PlayerPrefsで保存。

**アタッチ先**: 任意の GameObject

**Inspector 設定**:
- `cameraController` → FreeCameraController（未設定なら自動検索）
- `moveSpeedSlider` / `lookSpeedSlider` → UI Slider

---

## ロボット関連

### HsrSmoothGripper
**役割**: グリッパーのOpen/Closeアニメーション。

**アタッチ先**: グリッパーオブジェクト

---

### SpeechBubble
**役割**: 発話テキストの吹き出し表示（タイプライター演出）。

**アタッチ先**: 吹き出しUI GameObject

**Inspector 設定**:
| フィールド | 内容 |
|-----------|------|
| text | TextMeshProUGUI |
| displayTime | 表示時間（秒） |
| typeInterval | 1文字あたりの表示間隔 |
| maxCharsPerLine | 自動改行の文字数 |

---

### TTSManager
**役割**: Windows/Mac対応のテキスト読み上げ。

**アタッチ先**: 任意の GameObject（シングルトン）

**Inspector 設定**:
- `macVoice` — Mac の音声名（デフォルト "Kyoko"）

**呼び出し**:
```csharp
TTSManager.Instance.Speak("テキスト");
```

---

## カメラ

### FreeCameraController
**役割**: 自由視点カメラ操作。

**アタッチ先**: カメラ GameObject

**操作**:
| 入力 | 動作 |
|-----|------|
| 右クリック＋ドラッグ | 視点回転 |
| WASD | 水平移動 |
| Space | 上昇 |
| Ctrl | 下降 |

---

## その他

### ResetButton
**役割**: シーンをリロードしてリセット。

**アタッチ先**: 任意の GameObject

### LookAtCamera
**役割**: 常にカメラに向くビルボード効果。

**アタッチ先**: 吹き出し等のUI GameObject
