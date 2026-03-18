using UnityEngine;

public class ConditionalGoal : MonoBehaviour
{
    public string taskName;
    public string requireCompletedTask; // 前提タスク

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Robot")) return;

        if (requireCompletedTask != "")
        {
            if (!FreeTaskManager.Instance.IsCompleted(requireCompletedTask))
                return;
        }

        FreeTaskManager.Instance.CompleteTask(taskName);
    }
}