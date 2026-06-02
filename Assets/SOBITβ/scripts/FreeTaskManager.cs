using System.Collections.Generic;
using UnityEngine;

public class FreeTaskManager : MonoBehaviour
{
    public static FreeTaskManager Instance;

    [System.Serializable]
    public class TaskData
    {
        public string taskName;     // タスク識別名
        public int score;           // アルファスコア（共通の場合もこちら）
        public int scoreBeta;       // ベータスコア（0なら score を使用）
        public bool isCompleted;    // 達成済みか
    }

    public List<TaskData> tasks = new List<TaskData>();

    void Awake()
    {
        Instance = this;
    }

    public void CompleteTask(string taskName, bool isBeta = false)
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
                int s = (isBeta && t.scoreBeta > 0) ? t.scoreBeta : t.score;
                ScoreManager.Instance.AddScore(s);

                Debug.Log($"Task Clear : {taskName} +{s}{(isBeta ? " (Beta)" : "")}");
                return;
            }
        }

        Debug.LogWarning("Task not found : " + taskName);
    }

    public void CompleteTaskAlpha(string taskName) => CompleteTask(taskName, false);
    public void CompleteTaskBeta(string taskName)  => CompleteTask(taskName, true);

    public bool IsCompleted(string taskName)
    {
        foreach (var t in tasks)
        {
            if (t.taskName == taskName)
                return t.isCompleted;
        }
        return false;
    }

    // スコア加算なしでタスクを完了済みにする（スキップ用）
    public void SkipTask(string taskName)
    {
        foreach (var t in tasks)
        {
            if (t.taskName == taskName)
            {
                t.isCompleted = true;
                Debug.Log($"Task Skipped : {taskName}");
                return;
            }
        }
        Debug.LogWarning("Task not found : " + taskName);
    }

    // 前提タスクがすべて達成済みかチェック
    public bool CanAttempt(string[] prerequisites)
    {
        foreach (var p in prerequisites)
            if (!IsCompleted(p)) return false;
        return true;
    }
}