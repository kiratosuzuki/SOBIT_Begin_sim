using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera overrideCamera;

    void LateUpdate()
    {
        Camera cam = ResolveCamera();
        if (cam == null) return;

        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;

        if (camForward.sqrMagnitude < 0.001f) return;

        transform.rotation = Quaternion.LookRotation(camForward);
    }

    Camera ResolveCamera()
    {
        if (overrideCamera != null) return overrideCamera;

        var mcv = MultiCameraView.Instance;
        if (mcv != null && mcv.IsActive && mcv.activeCamera != null)
            return mcv.activeCamera;

        return Camera.main;
    }
}
