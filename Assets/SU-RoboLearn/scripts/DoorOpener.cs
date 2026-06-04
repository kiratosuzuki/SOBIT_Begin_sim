using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [Header("Hinge")]
    public Transform pivot;
    public float openAngle = 90f;
    public float speed = 60f;

    public enum RotationAxis { X, Y, Z }
    public RotationAxis axis = RotationAxis.Y;

    private float rotated = 0f;
    private bool isOpening = false;

    public void Open()
    {
        if (isOpening) return;
        isOpening = true;
    }

    void Update()
    {
        if (!isOpening) return;

        float step = speed * Time.deltaTime;
        float remaining = openAngle - rotated;

        if (remaining <= 0f)
        {
            isOpening = false;
            return;
        }

        step = Mathf.Min(step, remaining);
        Vector3 axisVec = axis == RotationAxis.X ? pivot.right :
                          axis == RotationAxis.Z ? pivot.forward : pivot.up;
        transform.RotateAround(pivot.position, axisVec, step);
        rotated += step;
    }
}
