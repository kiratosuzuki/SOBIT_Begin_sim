using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;
using System.Collections.Generic;
using System;

public class WebSocketServerUnity : MonoBehaviour
{
    private WebSocketServer wssv;

    [Header("Robot")]
    public Transform baseFootprint;
    public Camera robotCamera;
    public float maxDistance = 50f;
    public LayerMask detectLayer = ~0;

    [Header("Components")]
    public TalkGoal talkGoal;
    public HsrSmoothGripper gripper;
    public SpeechBubble bubble;
    public ArmLifter armLifter;

    [Header("Camera Topic")]
    public float cameraScanInterval = 0.5f;

    [Header("Talk Reply Delay")]
    public float replyDelay = 4f;


    // Movement parameters
    private float linearSpeed = 0.5f;
    private float angularSpeed = 60f;

    // Movement state
    private bool isExecuting = false;
    private string currentCommand = "";
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private string talkText = "";

    // Timeout
    public float timeoutMargin = 1.2f;
    private float moveTimer = 0f;
    private float moveTimeout = 0f;

    // Delay state
    private bool delayMode = false;
    private float delayTimer = 0f;
    private float delayDuration = 0f;
    private string pendingResponse = "";

    // Camera publish
    private float cameraScanTimer = 0f;

    // Command queue (written from WS thread, read from main thread)
    private readonly Queue<Command> commandQueue = new Queue<Command>();

    private struct Command
    {
        public string type;
        public float value;
        public bool hasValue;
    }


    // ======================================================
    // Lifecycle
    // ======================================================
    void Start()
    {
        wssv = new WebSocketServer(8080);

        // /unity/order  — Scratch → Unity (コマンド受信)
        wssv.AddWebSocketService<OrderService>("/unity/order", () =>
        {
            var s = new OrderService();
            s.OnCommand = EnqueueRaw;
            return s;
        });

        // /unity/camera   — Unity → Scratch (常時スキャン配信)
        wssv.AddWebSocketService<PassiveService>("/unity/camera");

        // /unity/response — Unity → Scratch (行動完了通知)
        wssv.AddWebSocketService<PassiveService>("/unity/response");

        // /unity/reply    — Unity → Scratch (talk応答テキスト)
        wssv.AddWebSocketService<PassiveService>("/unity/reply");

        wssv.Start();
        Debug.Log("🌐 WebSocket Server started: ws://localhost:8080");
        Debug.Log("  order    → ws://localhost:8080/unity/order");
        Debug.Log("  camera   → ws://localhost:8080/unity/camera");
        Debug.Log("  response → ws://localhost:8080/unity/response");
        Debug.Log("  reply    → ws://localhost:8080/unity/reply");
    }

    void OnDestroy() => wssv?.Stop();
    void OnApplicationQuit() => wssv?.Stop();

    void Update()
    {
        ProcessMovement();
        PublishCamera();
    }


    // ======================================================
    // コマンド受付 (WSスレッド → メインスレッド橋渡し)
    // ======================================================
    private void EnqueueRaw(string msg)
    {
        Command cmd = Parse(msg);
        lock (commandQueue)
            commandQueue.Enqueue(cmd);
    }

    private Command Parse(string msg)
    {
        var cmd = new Command { type = msg, value = 0f };

        if (msg.StartsWith("talk:"))
        {
            cmd.type = "talk";
            talkText = msg.Substring(5).Trim();
            return cmd;
        }

        if (msg.Contains(":"))
        {
            string[] parts = msg.Split(':');
            cmd.type = parts[0];
            if (parts.Length > 1 && float.TryParse(parts[1], out float v))
            {
                cmd.value = v;
                cmd.hasValue = true;
            }
        }

        return cmd;
    }


    // ======================================================
    // カメラトピック配信 (メインスレッド)
    // ======================================================
    private void PublishCamera()
    {
        cameraScanTimer += Time.deltaTime;
        if (cameraScanTimer < cameraScanInterval) return;
        cameraScanTimer = 0f;

        string result = DoScan();
        Broadcast("/unity/camera", result);
    }


    // ======================================================
    // 行動処理 (メインスレッド)
    // ======================================================
    private void ProcessMovement()
    {
        if (baseFootprint == null) return;

        // 遅延待ち
        if (delayMode)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer < delayDuration) return;

            delayMode = false;
            isExecuting = false;
            if (!string.IsNullOrEmpty(pendingResponse))
            {
                Broadcast("/unity/response", pendingResponse);
                pendingResponse = "";
            }
            return;
        }

        // 次のコマンドをデキュー
        if (!isExecuting)
        {
            lock (commandQueue)
            {
                if (commandQueue.Count == 0) return;

                Command cmd = commandQueue.Dequeue();
                currentCommand = cmd.type;

                if (currentCommand == "move")
                {
                    float dist = cmd.hasValue ? Mathf.Abs(cmd.value) / 100f : 1.0f;
                    targetPosition = baseFootprint.position + (-baseFootprint.right * dist);
                    moveTimeout = dist / linearSpeed * timeoutMargin;
                }
                else if (currentCommand == "turn")
                {
                    float angle = cmd.hasValue ? cmd.value : 90f;
                    targetRotation = baseFootprint.rotation * Quaternion.AngleAxis(angle, Vector3.forward);
                    moveTimeout = Mathf.Abs(angle) / angularSpeed * timeoutMargin;
                }

                isExecuting = true;
                moveTimer = 0f;
            }
        }

        if (!isExecuting) return;

        switch (currentCommand)
        {
            case "move":
            {
                moveTimer += Time.deltaTime;
                baseFootprint.position = Vector3.MoveTowards(
                    baseFootprint.position, targetPosition, linearSpeed * Time.deltaTime);
                if (baseFootprint.position == targetPosition || moveTimer >= moveTimeout)
                {
                    isExecuting = false;
                    Broadcast("/unity/response", "done:move");
                }
                break;
            }

            case "turn":
            {
                moveTimer += Time.deltaTime;
                baseFootprint.rotation = Quaternion.RotateTowards(
                    baseFootprint.rotation, targetRotation, angularSpeed * Time.deltaTime);
                if (baseFootprint.rotation == targetRotation || moveTimer >= moveTimeout)
                {
                    isExecuting = false;
                    Broadcast("/unity/response", "done:turn");
                }
                break;
            }

            case "talk":
            {
                bubble.Say(talkText);
                TTSManager.Instance?.Speak(talkText);
                string reply = talkGoal.HandleUserText(talkText);
                StartCoroutine(BroadcastDelayed("/unity/reply", reply, replyDelay));
                StartDelay(4f, "done:talk");
                break;
            }

            case "open":
                gripper?.Open();
                StartDelay(2f, "done:open");
                break;

            case "close":
                gripper?.Close();
                StartDelay(2f, "done:close");
                break;

            case "arm_up":
                armLifter?.MoveUp();
                StartDelay(1f, "done:arm_up");
                break;

            case "arm_down":
                armLifter?.MoveDown();
                StartDelay(1f, "done:arm_down");
                break;

            default:
                isExecuting = false;
                break;
        }
    }

    private void StartDelay(float duration, string responseAfter)
    {
        delayDuration = duration;
        delayTimer = 0f;
        delayMode = true;
        pendingResponse = responseAfter;
        isExecuting = false;
    }


    // ======================================================
    // スキャン
    // ======================================================
    private string DoScan()
    {
        if (robotCamera == null) return "none";

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(robotCamera);
        Collider[] hits = Physics.OverlapSphere(robotCamera.transform.position, maxDistance, detectLayer);

        var detected = new List<string>();
        foreach (var hit in hits)
        {
            if (GeometryUtility.TestPlanesAABB(planes, hit.bounds) && hit.CompareTag("scan"))
                detected.Add(hit.gameObject.name);
        }

        return detected.Count > 0 ? string.Join(",", detected) : "none";
    }


    // ======================================================
    // ブロードキャスト
    // ======================================================
    private System.Collections.IEnumerator BroadcastDelayed(string path, string message, float delay)
    {
        yield return new WaitForSeconds(delay);
        Broadcast(path, message);
    }

    private void Broadcast(string path, string message)
    {
        try
        {
            wssv.WebSocketServices[path].Sessions.Broadcast(message);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Broadcast failed [{path}]: {e.Message}");
        }
    }


    // ======================================================
    // Gizmos
    // ======================================================
    private void OnDrawGizmos()
    {
        if (robotCamera == null) return;
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawWireSphere(robotCamera.transform.position, maxDistance);
    }
}


// ======================================================
// OrderService — /unity/order
// Scratch からコマンドを受け取るだけ
// ======================================================
public class OrderService : WebSocketBehavior
{
    public Action<string> OnCommand;

    protected override void OnMessage(MessageEventArgs e)
    {
        OnCommand?.Invoke(e.Data);
    }
}


// ======================================================
// PassiveService — /unity/camera, /unity/response
// クライアントが接続してブロードキャストを受け取るだけ
// ======================================================
public class PassiveService : WebSocketBehavior
{
    protected override void OnMessage(MessageEventArgs e) { }
}
