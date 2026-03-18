using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [System.Serializable]
    public class TaskData
    {
        public GameObject goal;   // ゴール地点
        public int score;         // スコア
    }

    public List<TaskData> tasks = new List<TaskData>();

    private int currentIndex = 0;
    // 現在のタスクのインデックスを外部から取得できるようにする
    public int CurrentIndex
    {
        get { return currentIndex; }
    }


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ActivateTask(0);
    }

    public void ActivateTask(int index)
    {
        // すべて無効化
        foreach (var t in tasks)
            t.goal.SetActive(false);

        // 指定タスクだけを有効化
        tasks[index].goal.SetActive(true);

        currentIndex = index;
    }
    
    public void CompleteTask(GameObject goalObject)
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            if (tasks[i].goal == goalObject)
            {
                // ⭐ そのタスク固有のスコアを加算
                ScoreManager.Instance.AddScore(tasks[i].score);

                tasks[i].goal.SetActive(false);

                Debug.Log("Clear : " + tasks[i].goal.name);
                return;
            }
        }
    }

    public void CompleteCurrentTask()
    {
        // スコア加算
        ScoreManager.Instance.AddScore(tasks[currentIndex].score);

        // 次のタスクへ
        int next = currentIndex + 1;

        if (next < tasks.Count)
        {
            ActivateTask(next);
        }
        else
        {
            Debug.Log("ALL TASKS COMPLETED!");
        }
    }
}
