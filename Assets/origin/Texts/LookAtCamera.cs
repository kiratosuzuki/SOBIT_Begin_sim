using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;                 // ← 縦成分を無視（ロール・ピッチ防止）

        if (camForward.sqrMagnitude < 0.001f) return;

        transform.rotation = Quaternion.LookRotation(camForward);
    }
}
