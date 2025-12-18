using UnityEngine;
using WebSocketSharp.Server;
using System;

public class CameraStreamer : MonoBehaviour
{
    public Camera cam; // 🔹ここに配信したいカメラを指定
    private WebSocketServer server;
    private Texture2D tex;

    void Start()
    {
        // WebSocketサーバ起動
        server = new WebSocketServer(8080);
        server.AddWebSocketService<CameraService>("/camera");
        server.Start();
        Debug.Log("📡 WebSocketサーバ起動: ws://localhost:8080/camera");

        // 一時的なテクスチャを作成
        tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    void Update()
    {
        if (cam == null || cam.targetTexture == null) return;

        // カメラ映像を読み込む
        RenderTexture rt = cam.targetTexture;
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        // JPEG化してBase64文字列に変換
        byte[] bytes = tex.EncodeToJPG(50);
        string base64 = Convert.ToBase64String(bytes);
        CameraService.latestFrame = base64;
    }

    void OnApplicationQuit()
    {
        server.Stop();
    }
}

// WebSocket通信部分
public class CameraService : WebSocketBehavior
{
    public static string latestFrame = "";

    protected override void OnMessage(WebSocketSharp.MessageEventArgs e)
    {
        Send(latestFrame);
    }
}
