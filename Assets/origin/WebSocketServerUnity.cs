using UnityEngine;
using WebSocketSharp.Server;
using WebSocketSharp;
using System.Collections.Generic;

public class WebSocketServerUnity : MonoBehaviour
{
    private WebSocketServer wssv;
    private OriginUnity originUnity;

    public Transform baseFootprint;
    public Camera camera;
    public float maxDistance = 50f;
    public LayerMask detectLayer = ~0;

    public ClerkRobot clerkRobot;   // ← 追加（Inspector で指定）
    public TalkGoal talkGoal;


    void Start()
    {
        wssv = new WebSocketServer(8080);

        wssv.AddWebSocketService<OriginUnity>("/unity", () =>
        {
            var service = new OriginUnity();

            service.SetBaseFootprint(baseFootprint);
            service.SetCamera(camera, maxDistance, detectLayer);
            service.clerk = clerkRobot;   // ← 店員ロボを渡す
            service.talkGoal = talkGoal; // orderManager は WebSocketServerUnity のフィールドとして Inspector で指定


            originUnity = service;
            return service;
        });

        wssv.Start();
        Debug.Log("🌐 WebSocket Server started: ws://localhost:8080/unity");
    }

    void Update()
    {
        originUnity?.UpdateMovement();
    }

    void OnApplicationQuit()
    {
        wssv?.Stop();
        Debug.Log("🛑 WebSocket server stopped");
    }
    private void OnDrawGizmos()
    {
        if (camera == null) return;

        Gizmos.color = new Color(0f, 1f, 1f, 0.25f); // 半透明のシアン
        Gizmos.DrawWireSphere(camera.transform.position, maxDistance);
    }
    
}
// ======================================================

public class OriginUnity : WebSocketBehavior
{
    public ClerkRobot clerk;
    public TalkGoal talkGoal;

    private Transform baseFootprint;
    private Camera camera;
    private float maxDistance;
    private LayerMask detectLayer;

    private Queue<Command> commandQueue = new Queue<Command>();

    private float linearSpeed = 0.5f;
    private float angularSpeed = 60f;
    private float targetDistance = 1.0f;
    private float targetAngle = 90f;

    private bool isExecuting = false;
    private string currentCommand = "";
    private float movedDistance = 0f;
    private float turnedAngle = 0f;
    private float turnDirection = 1f;

    // ★ 追加：took の会話テキスト用
    private string talkText = "";

    private struct Command
    {
        public string type;
        public float value;
    }

    public void SetBaseFootprint(Transform t)
    {
        baseFootprint = t;
    }

    public void SetCamera(Camera cam, float distance, LayerMask layer)
    {
        camera = cam;
        maxDistance = distance;
        detectLayer = layer;
    }


    // ======================================================
    // WebSocket 受信
    // ======================================================
    protected override void OnMessage(MessageEventArgs e)
    {
        Debug.Log($"📩 受信: {e.Data}");

        Command cmd = ParseCommand(e.Data);

        lock (commandQueue)
        {
            commandQueue.Enqueue(cmd);
        }
    }



    // ======================================================
    // コマンド解析
    // ======================================================
    private Command ParseCommand(string msg)
    {
        Command cmd = new Command { type = msg, value = 0f };

        // --- took は特別処理 ---
        if (msg.StartsWith("took:"))
        {
            cmd.type = "took";
            talkText = msg.Substring(5).Trim();
            return cmd;
        }

        // move / turn
        if (msg.Contains(":"))
        {
            string[] parts = msg.Split(':');
            cmd.type = parts[0];

            if (parts.Length > 1 && float.TryParse(parts[1], out float val))
                cmd.value = val;
        }

        return cmd;
    }




    // ======================================================
    // UpdateMovement（コマンド実行）
    // ======================================================
    public void UpdateMovement()
    {
        if (baseFootprint == null) return;

        if (!isExecuting)
        {
            lock (commandQueue)
            {
                if (commandQueue.Count > 0)
                {
                    Command cmd = commandQueue.Dequeue();
                    currentCommand = cmd.type;

                    if (currentCommand == "move")
                        targetDistance = (cmd.value != 0f) ? Mathf.Abs(cmd.value) : 1.0f;

                    else if (currentCommand == "turn")
                    {
                        targetAngle = Mathf.Abs(cmd.value != 0f ? cmd.value : 90f);
                        turnDirection = (cmd.value >= 0f) ? 1f : -1f;
                    }

                    isExecuting = true;
                    movedDistance = 0f;
                    turnedAngle = 0f;
                }
            }
        }

        if (!isExecuting) return;


        // ---------- move ----------
        if (currentCommand == "move")
        {
            float step = linearSpeed * Time.deltaTime;
            baseFootprint.Translate(-Vector3.right * step, Space.Self);
            movedDistance += step;

            if (movedDistance >= targetDistance)
                isExecuting = false;
        }

        // ---------- turn ----------
        else if (currentCommand == "turn")
        {
            float step = angularSpeed * Time.deltaTime;
            baseFootprint.Rotate(Vector3.forward * step * turnDirection, Space.Self);
            turnedAngle += step;

            if (turnedAngle >= targetAngle)
                isExecuting = false;
        }

        // ---------- scan ----------
        else if (currentCommand == "scan")
        {
            DoScan();
            isExecuting = false;
        }

        // ---------- took（会話） ----------
        else if (currentCommand == "took")
        {
            DoTalk(talkText);
            isExecuting = false;
        }

        else
        {
            isExecuting = false;
        }
    }





    // ======================================================
    // スキャン
    // ======================================================
    private void DoScan()
    {
        if (camera == null) return;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        Collider[] hits = Physics.OverlapSphere(camera.transform.position, maxDistance, detectLayer);

        List<string> detectedNames = new List<string>();

        foreach (var hit in hits)
        {
            if (GeometryUtility.TestPlanesAABB(planes, hit.bounds) && hit.CompareTag("scan"))
            {
                detectedNames.Add(hit.gameObject.name);
                Debug.Log($"🔍 検知: {hit.gameObject.name}");
            }
        }

        Send(detectedNames.Count > 0 ? string.Join(",", detectedNames) : "none");
    }






    // ======================================================
    // 会話処理
    // ======================================================
    private void DoTalk(string userText)
    {
        Debug.Log($"Robot: {userText}");
        string reply = talkGoal.HandleUserText(userText);
        if (reply != null)
        {
            Send(reply);
        }
    }

}
