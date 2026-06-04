using UnityEngine;

public class ArmLifter : MonoBehaviour
{
    [Header("Range")]
    public float minHeight = 0f;
    public float maxHeight = 1f;
    public float speed = 0.5f;

    private enum Direction { None, Up, Down }
    private Direction current = Direction.None;

    public void MoveUp()   => current = Direction.Up;
    public void MoveDown() => current = Direction.Down;
    public void Stop()     => current = Direction.None;

    void Update()
    {
        if (current == Direction.None) return;

        float delta = speed * Time.deltaTime * (current == Direction.Up ? 1f : -1f);
        Vector3 pos = transform.localPosition;
        pos.z = Mathf.Clamp(pos.z + delta, minHeight, maxHeight);
        transform.localPosition = pos;
    }
}
