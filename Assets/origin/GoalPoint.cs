using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    // このゴールが何番目のタスクかを設定
    public int taskIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            // 現在のタスクと一致する場合だけ処理
            if (TaskManager.Instance.CurrentIndex == taskIndex)
            {
                Debug.Log("Goal Reached:" + gameObject.name);
                TaskManager.Instance.CompleteCurrentTask();
            }
        }
    }
}
