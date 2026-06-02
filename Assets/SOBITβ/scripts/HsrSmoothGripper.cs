using UnityEngine;

public class HsrSmoothGripper : MonoBehaviour
{
    [Header("Left Finger")]
    public Transform leftFinger;
    public Transform leftPivot;
    public float leftOpenAngle = 30f;

    [Header("Right Finger")]
    public Transform rightFinger;
    public Transform rightPivot;
    public float rightOpenAngle = -30f;

    public enum RotationAxis { X, Y, Z }

    [Header("Axis")]
    public RotationAxis axis = RotationAxis.X;

    [Header("Speed")]
    public float moveSpeed = 80f;

    [Header("Grasp")]
    public Transform gripAttachPoint;
    public float gripRadius = 0.15f;
    public string graspTag = "Graspable";

    private float leftRotated = 0f;
    private float rightRotated = 0f;
    private bool isOpen = false;

    private Transform grabbedObject;
    private Transform originalParent;

    void Update()
    {
        float leftTarget  = isOpen ? leftOpenAngle  : 0f;
        float rightTarget = isOpen ? rightOpenAngle : 0f;
        UpdateFinger(leftFinger,  leftPivot,  leftTarget,  ref leftRotated);
        UpdateFinger(rightFinger, rightPivot, rightTarget, ref rightRotated);
    }

    void UpdateFinger(Transform finger, Transform pivot, float targetAngle, ref float rotated)
    {
        if (finger == null || pivot == null) return;

        float remaining = targetAngle - rotated;
        if (Mathf.Abs(remaining) < 0.01f) return;

        float step = Mathf.MoveTowards(0f, remaining, moveSpeed * Time.deltaTime);
        Vector3 axisVec = axis == RotationAxis.Y ? pivot.up :
                          axis == RotationAxis.Z ? pivot.forward : pivot.right;

        finger.RotateAround(pivot.position, axisVec, step);
        rotated += step;
    }

    public void Open()
    {
        isOpen = true;
        Release();
    }

    public void Close()
    {
        isOpen = false;
        TryGrasp();
    }

    void TryGrasp()
    {
        if (gripAttachPoint == null || grabbedObject != null) return;

        Collider[] hits = Physics.OverlapSphere(gripAttachPoint.position, gripRadius);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag(graspTag)) continue;

            grabbedObject = hit.transform;
            originalParent = grabbedObject.parent;

            var rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            grabbedObject.SetParent(gripAttachPoint);
            Debug.Log($"[Gripper] Grasped: {grabbedObject.name}");
            break;
        }
    }

    void Release()
    {
        if (grabbedObject == null) return;

        var rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        grabbedObject.SetParent(originalParent);
        Debug.Log($"[Gripper] Released: {grabbedObject.name}");

        grabbedObject = null;
        originalParent = null;
    }
}
