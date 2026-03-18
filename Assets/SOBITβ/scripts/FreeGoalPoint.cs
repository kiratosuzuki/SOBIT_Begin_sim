using UnityEngine;

public class FreeGoalPoint : MonoBehaviour
{
    public string taskName;   // Inspectorで設定するタスク名

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            Debug.Log("Goal Reached : " + taskName);

            FreeTaskManager.Instance.CompleteTask(taskName);
        }
    }
}