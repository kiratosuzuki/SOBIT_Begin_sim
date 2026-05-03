// using UnityEngine;
// using WebSocketSharp.Server;
// using WebSocketSharp;
// using System.Collections.Generic;

// public class WebServer : MonoBehaviour
// {
//     private WebSocketServer wssv;
//     private OriginUnity originUnity;

//     public Transform baseFootprint;
//     public Camera camera;
//     public float maxDistance = 50f;
//     public LayerMask detectLayer = ~0;

//     public TalkGoal talkGoal;
//     public HsrSmoothGripper gripper;
//     public SpeechBubble bubble;

//     void Start()
//     {
//         wssv = new WebSocketServer(8080);

//         wssv.AddWebSocketService<OriginUnity>("/unity", () =>
//         {
//             var service = new OriginUnity();

//             service.SetBaseFootprint(baseFootprint);
//             service.SetCamera(camera, maxDistance, detectLayer);
//             service.talkGoal = talkGoal;
//             service.bubble = bubble;
//             service.gripper = gripper;

//             originUnity = service;
//             return service;
//         });

//         wssv.Start();
//         Debug.Log("🌐 WebSocket Server started: ws://localhost:8080/unity");
//     }

//     void OnDestroy()
//     {
//         // Unity の Play/Stop の「Stop」時に必ず呼ばれる
//         wssv?.Stop();
//         Debug.Log("🛑 WebSocket server stopped (OnDestroy)");
//     }

//     void Update()
//     {
//         originUnity?.UpdateMovement();
//     }

//     void OnApplicationQuit()
//     {
//         wssv?.Stop();
//         Debug.Log("🛑 WebSocket server stopped");
//     }

//     private void OnDrawGizmos()
//     {
//         if (camera == null) return;

//         Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
//         Gizmos.DrawWireSphere(camera.transform.position, maxDistance);
//     }
// }



// // ======================================================
// // OriginUnity
// // ======================================================
// public class OriginUnity : WebSocketBehavior
// {
//     public TalkGoal talkGoal;
//     public SpeechBubble bubble;

//     private Transform baseFootprint;
//     private Camera camera;
//     private float maxDistance;
//     private LayerMask detectLayer;

//     private Queue<Command> commandQueue = new Queue<Command>();

//     private float linearSpeed = 0.5f;
//     private float angularSpeed = 60f;
//     private float targetDistance = 1.0f;
//     private float targetAngle = 90f;

//     private bool isExecuting = false;
//     private string currentCommand = "";
//     private float movedDistance = 0f;
//     private float turnedAngle = 0f;
//     private float turnDirection = 1f;

//     private string talkText = "";

//     // ★ 遅延処理用
//     private bool delayMode = false;
//     private float delayTimer = 0f;
//     private float delayDuration = 5f;   // ← 1秒待ち

//     private struct Command
//     {
//         public string type;
//         public float value;
//     }

//     public HsrSmoothGripper gripper;

//     //★特別枠
//     bool continuousScan = true;
//     float scanInterval = 0.5f;
//     float scanTimer = 0f;


//     // -------- setter --------
//     public void SetBaseFootprint(Transform t) => baseFootprint = t;
//     public void SetCamera(Camera cam, float distance, LayerMask layer)
//     {
//         camera = cam;
//         maxDistance = distance;
//         detectLayer = layer;
//     }


//     // ======================================================
//     // WebSocket 受信
//     // ======================================================
//     protected override void OnMessage(MessageEventArgs e)
//     {
//         //Debug.Log($"📩 受信: {e.Data}");

//         Command cmd = ParseCommand(e.Data);

//         lock (commandQueue)
//         {
//             commandQueue.Enqueue(cmd);
//         }
//     }


//     // ======================================================
//     // コマンド解析
//     // ======================================================
//     private Command ParseCommand(string msg)
//     {
//         Command cmd = new Command { type = msg, value = 0f };

//         if (msg.StartsWith("talk:"))
//         {
//             cmd.type = "talk";
//             talkText = msg.Substring(5).Trim();
//             return cmd;
//         }

//         if (msg.Contains(":"))
//         {
//             string[] parts = msg.Split(':');
//             cmd.type = parts[0];

//             if (parts.Length > 1 && float.TryParse(parts[1], out float val))
//                 cmd.value = val;
//         }

//         return cmd;
//     }


//     // ======================================================
//     // UpdateMovement
//     // ======================================================
//     public void UpdateMovement()
//     {
//         //★ 常時スキャン
//         if (continuousScan)
//         {
//             scanTimer += Time.deltaTime;

//             if (scanTimer >= scanInterval)
//             {
//                 DoScan();
//                 scanTimer = 0f;
//             }
//         }

//         if (baseFootprint == null) return;

//         // ---------- ★ 遅延中なら待つ ----------
//         if (delayMode)
//         {
//             delayTimer += Time.deltaTime;
//             if (delayTimer >= delayDuration)
//             {
//                 delayMode = false;
//                 isExecuting = false;
//             }
//             return;
//         }


//         if (!isExecuting)
//         {
//             lock (commandQueue)
//             {
//                 if (commandQueue.Count > 0)
//                 {
//                     Command cmd = commandQueue.Dequeue();
//                     currentCommand = cmd.type;

//                     if (currentCommand == "move")
//                         targetDistance = (cmd.value != 0f) ? Mathf.Abs(cmd.value) : 1.0f;

//                     else if (currentCommand == "turn")
//                     {
//                         targetAngle = Mathf.Abs(cmd.value != 0f ? cmd.value : 90f);
//                         turnDirection = (cmd.value >= 0f) ? 1f : -1f;
//                     }

//                     isExecuting = true;
//                     movedDistance = 0f;
//                     turnedAngle = 0f;
//                 }
//             }
//         }

//         if (!isExecuting) return;


//         // ---------- move ----------
//         if (currentCommand == "move")
//         {
//             float step = linearSpeed * Time.deltaTime;
//             baseFootprint.Translate(-Vector3.right * step, Space.Self);
//             movedDistance += step;

//             if (movedDistance >= targetDistance)
//                 isExecuting = false;
//         }

//         // ---------- turn ----------
//         else if (currentCommand == "turn")
//         {
//             float step = angularSpeed * Time.deltaTime;
//             baseFootprint.Rotate(Vector3.forward * step * turnDirection, Space.Self);
//             turnedAngle += step;

//             if (turnedAngle >= targetAngle)
//                 isExecuting = false;
//         }

//         // ---------- scan ----------
//         else if (currentCommand == "scan")
//         {
//             DoScan();
//             isExecuting = false;
//         }

//         // ---------- talk ----------
//         else if (currentCommand == "talk")
//         {
//             Debug.Log($"Robot: {talkText}");
//             bubble.Say(talkText);

//             delayDuration = 4f;
//             delayMode = true;
//             delayTimer = 0f;

//             string reply = talkGoal.HandleUserText(talkText);

//             Send(reply);
//             isExecuting = false;
//         }

//         // ---------- open ----------
//         else if (currentCommand == "open")
//         {
//             Debug.Log("🟦 Gripper OPEN command received");
//             gripper?.Open();

//             delayDuration = 2f;
//             delayMode = true;
//             delayTimer = 0f;
//         }

//         // ---------- close ----------
//         else if (currentCommand == "close")
//         {
//             Debug.Log("🟥 Gripper CLOSE command received");
//             gripper?.Close();

//             delayDuration = 2f;
//             delayMode = true;
//             delayTimer = 0f;
//         }

//         else
//         {
//             isExecuting = false;
//         }
//     }


//     // ======================================================
//     // スキャン
//     // ======================================================
//     private void DoScan()
//     {
//         if (camera == null) return;

//         Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
//         Collider[] hits = Physics.OverlapSphere(camera.transform.position, maxDistance, detectLayer);

//         List<string> detected = new List<string>();

//         foreach (var hit in hits)
//         {
//             if (GeometryUtility.TestPlanesAABB(planes, hit.bounds) && hit.CompareTag("scan"))
//             {
//                 detected.Add(hit.gameObject.name);
//                 Debug.Log($"🔍 検知: {hit.gameObject.name}");
//             }
//         }

//         Send(detected.Count > 0 ? string.Join(",", detected) : "none");
//     }


//     // ======================================================
//     // 会話
//     // ======================================================
//     private void DoTalk(string userText)
//     {

//     }
// }
