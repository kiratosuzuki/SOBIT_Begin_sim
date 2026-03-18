using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 5f;
    public float verticalSpeed = 2f; // 上下移動スピード

    private float yaw;
    private float pitch;

    private bool wasDragging = false;

    void Update()
    {
        // クリック開始を検出
        if (Input.GetMouseButtonDown(1))
        {
            // 今のカメラ角度を yaw/pitch に反映する
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;

            wasDragging = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // クリック中なら視点回転
        if (Input.GetMouseButton(1))
        {
            yaw += lookSpeed * Input.GetAxis("Mouse X");
            pitch -= lookSpeed * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -90f, 90f);

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        // 水平移動（WASD）
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = (transform.right * x + transform.forward * z) * moveSpeed;

        // 上下移動は speed を乗せない（別の verticalSpeed のみ）
        if (Input.GetKey(KeyCode.Space))
            move += Vector3.up * verticalSpeed;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            move += Vector3.down * verticalSpeed;

        // まとめて反映
        transform.position += move * Time.deltaTime;
    }

}
