using UnityEngine;

public class HSRMoveSimple : MonoBehaviour
{
    public Transform baseFootprint;
    public float linearSpeed = 0.5f;
    public float angularSpeed = 60f;

    void Update()
    {
        float move = Input.GetAxis("Vertical") * linearSpeed * Time.deltaTime;
        float turn = Input.GetAxis("Horizontal") * angularSpeed * Time.deltaTime;

        // ✅ Wで前進、Sで後退（X軸が前方向、符号反転）
        baseFootprint.Translate(-Vector3.right * move, Space.Self);

        // ✅ Z軸回りで旋回（A左 / D右）
        baseFootprint.Rotate(Vector3.forward * turn);
    }
}
