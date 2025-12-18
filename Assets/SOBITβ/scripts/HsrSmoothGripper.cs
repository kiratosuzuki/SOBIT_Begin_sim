using UnityEngine;

public class HsrSmoothGripper : MonoBehaviour
{
    public Transform leftFinger;
    public Transform rightFinger;
    public Transform gripAttachPoint;

    // 開くときの角度
    public float leftOpenAngle = 30f;
    public float rightOpenAngle = -30f;

    // 動くスピード（度/秒）
    public float moveSpeed = 80f;

    private float leftTargetAngle = 0f;
    private float rightTargetAngle = 0f;

    private Transform grabbedObject;

    void Update()
    {
        UpdateFingerRotation();
    }

    // ---- 滑らかに回転する処理 ----
    void UpdateFingerRotation()
    {
        float currentLeft = leftFinger.localEulerAngles.x;
        float currentRight = rightFinger.localEulerAngles.x;

        // Unity の角度は 0〜360 になるので -180〜180 に補正
        if (currentLeft > 180) currentLeft -= 360;
        if (currentRight > 180) currentRight -= 360;

        float newLeft = Mathf.MoveTowards(currentLeft, leftTargetAngle, moveSpeed * Time.deltaTime);
        float newRight = Mathf.MoveTowards(currentRight, rightTargetAngle, moveSpeed * Time.deltaTime);

        leftFinger.localRotation = Quaternion.Euler(newLeft, 0, 0);
        rightFinger.localRotation = Quaternion.Euler(newRight, 0, 0);
    }

    // ---- 開く ----
    public void Open()
    {
        leftTargetAngle = leftOpenAngle;
        rightTargetAngle = rightOpenAngle;
    }

    // ---- 閉じる ----
    public void Close()
    {
        leftTargetAngle = 0f;
        rightTargetAngle = 0f;
    }

}
