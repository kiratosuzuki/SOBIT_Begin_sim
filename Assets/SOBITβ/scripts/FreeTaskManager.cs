using System.Collections.Generic;
using UnityEngine;

public class FreeTaskManager : MonoBehaviour
{
    public static FreeTaskManager Instance;

    [System.Serializable]
    public class TaskData
    {
        public string taskName;     // タスク識別名
        public int score;           // 得点
        public bool isCompleted;    // 達成済みか
    }

    public List<TaskData> tasks = new List<TaskData>();

    void Awake()
    {
        Instance = this;
    }

    public void CompleteTask(string taskName)
    {
        foreach (var t in tasks)
        {
            if (t.taskName == taskName)
            {
                if (t.isCompleted)
                {
                    Debug.Log("Already completed : " + taskName);
                    return;
                }

                t.isCompleted = true;
                ScoreManager.Instance.AddScore(t.score);

                Debug.Log("Task Clear : " + taskName + " +" + t.score);
                return;
            }
        }

        Debug.LogWarning("Task not found : " + taskName);
    }

    public bool IsCompleted(string taskName)
    {
        foreach (var t in tasks)
        {
            if (t.taskName == taskName)
                return t.isCompleted;
        }
        return false;
    }
}