# WebSocket 通信ガイド

Scratch と Unity の通信仕様まとめ。

---

## 接続先

| 用途 | URL |
|------|-----|
| コマンド送信（Scratch → Unity） | `ws://localhost:8080/unity/order` |
| 動作完了受信（Unity → Scratch） | `ws://localhost:8080/unity/response` |
| カメラスキャン受信（Unity → Scratch） | `ws://localhost:8080/unity/camera` |
| 発話返答受信（Unity → Scratch） | `ws://localhost:8080/unity/reply` |

---

## 送信コマンド一覧（Scratch → Unity）

接続先: `ws://localhost:8080/unity/order`

| コマンド | 内容 | 例 |
|---------|------|----|
| `move:値` | ロボットを前進（cm単位） | `move:100` → 1m前進 |
| `turn:値` | ロボットを回転（度、負数で左回転） | `turn:90` → 右90度 / `turn:-90` → 左90度 |
| `talk:テキスト` | ロボットが発話・タスク判定 | `talk:こんにちは` |
| `open` | グリッパーを開く | `open` |
| `close` | グリッパーを閉じる（把持） | `close` |
| `arm_up` | アームを上昇 | `arm_up` |
| `arm_down` | アームを下降 | `arm_down` |

---

## 受信メッセージ一覧（Unity → Scratch）

### `/unity/response` — 動作完了通知

各コマンドの実行完了後に送られる。次のコマンドを送る前にこれを待つ。

| メッセージ | 対応コマンド |
|-----------|------------|
| `done:move` | move |
| `done:turn` | turn |
| `done:talk` | talk |
| `done:open` | open |
| `done:close` | close |
| `done:arm_up` | arm_up |
| `done:arm_down` | arm_down |

### `/unity/camera` — カメラスキャン結果

約0.5秒ごとに自動送信される。

| メッセージ | 内容 |
|-----------|------|
| `オブジェクト名` | 認識したオブジェクト名（複数の場合カンマ区切り） |
| `none` | 何も認識していない |

例: `ポテトチップス,テーブル1`

### `/unity/reply` — 発話返答テキスト

`talk:` コマンド送信後、NPCの返答テキストが送られる。

| メッセージ | 内容 |
|-----------|------|
| テキスト | NPCの返答文字列 |
| `none` | 返答なし |

---

## 基本的なScratchの処理フロー

```
1. ws://localhost:8080/unity/order に接続
2. ws://localhost:8080/unity/response に接続（受信用）
3. ws://localhost:8080/unity/camera に接続（受信用）
4. ws://localhost:8080/unity/reply に接続（受信用）

5. コマンド送信 → done:xxx を受信するまで待機 → 次のコマンド送信
```

---

## 注意事項

- Unityをゲームモードで起動してから接続すること
- ポート番号は `8080`
- コマンドはすべて半角文字で送信
