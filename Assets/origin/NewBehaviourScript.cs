using UnityEngine;

public class ShowAABB : MonoBehaviour
{
    void OnDrawGizmos()
    {
        // AABB（軸平行バウンディングボックス）
        Bounds b = GetComponent<Collider>().bounds;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(b.center, b.size);
    }
}
