using UnityEngine;

public class CameraFrustumVisualizer : MonoBehaviour
{
    public Camera targetCamera;
    public float maxDistance = 10f;

    void OnDrawGizmos()
    {
        if (targetCamera == null) return;

        Gizmos.color = Color.cyan;

        // カメラの位置と向き
        Transform camT = targetCamera.transform;

        // Near / Far を上書きして任意距離の視錐台を可視化する
        float near = targetCamera.nearClipPlane;
        float far = maxDistance;   // ← 奥行制限

        // 視錐台の四隅のワールド座標を取得
        Vector3[] corners = new Vector3[8];

        // Near 平面
        corners[0] = camT.position + camT.forward * near + camT.right * targetCamera.nearClipPlane * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) +
                     camT.up * targetCamera.nearClipPlane * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

        // UnityのAPIを使うともっと楽
        targetCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), near, Camera.MonoOrStereoscopicEye.Mono, corners);
        for (int i = 0; i < 4; i++) corners[i] = camT.TransformPoint(corners[i]);

        // Far 平面
        targetCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), far, Camera.MonoOrStereoscopicEye.Mono, corners);
        for (int i = 4; i < 8; i++) corners[i] = camT.TransformPoint(corners[i - 4]);

        // Near 平面の枠
        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        Gizmos.DrawLine(corners[3], corners[0]);

        // Far 平面の枠
        Gizmos.DrawLine(corners[4], corners[5]);
        Gizmos.DrawLine(corners[5], corners[6]);
        Gizmos.DrawLine(corners[6], corners[7]);
        Gizmos.DrawLine(corners[7], corners[4]);

        // Near → Far のつなぎ
        Gizmos.DrawLine(corners[0], corners[4]);
        Gizmos.DrawLine(corners[1], corners[5]);
        Gizmos.DrawLine(corners[2], corners[6]);
        Gizmos.DrawLine(corners[3], corners[7]);
    }
}
