using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 5f;
    public float verticalSpeed = 2f; // 上下移動スピード

    private float yaw;
    private float pitch;

    void Update()
    {
        // ✅ マウス左クリックで視点操作
        if (Input.GetMouseButton(0))
        {
            yaw += lookSpeed * Input.GetAxis("Mouse X");
            pitch -= lookSpeed * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -90f, 90f);
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // ✅ 水平移動（WASD）
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        // ✅ 上下移動（スペース=上昇, Ctrl=下降）
        if (Input.GetKey(KeyCode.Space))
        {
            move += Vector3.up * verticalSpeed;
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            move += Vector3.down * verticalSpeed;
        }

        transform.position += move * Time.deltaTime;
    }
}
