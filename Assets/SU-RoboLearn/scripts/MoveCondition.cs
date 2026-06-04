using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// robotTag を持つコライダーがトリガーに入ったときにタスクを達成する条件コンポーネント。
/// Collider (IsTrigger=true) と一緒にアタッチして taskName を設定するだけでタスクを追加できる。
/// </summary>
[RequireComponent(typeof(Collider))]
public class MoveCondition : MonoBehaviour
{
    [Header("Task")]
    public string taskName;

    [Header("Condition")]
    public string robotTag = "Robot";

    [Header("Difficulty (アルファ/ベータでスコアを変える場合)")]
    public bool hasDifficulty = false;
    public SettingKey difficultyKey;

    [Header("Prerequisites (前提タスク名、任意)")]
    public string[] prerequisites;

    [Header("On Completed")]
    public UnityEvent onCompleted;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(robotTag)) return;
        if (FreeTaskManager.Instance.IsCompleted(taskName)) return;
        if (!FreeTaskManager.Instance.CanAttempt(prerequisites)) return;

        bool isBeta = hasDifficulty
            && CompetitionSettings.Instance != null
            && CompetitionSettings.Instance.Get(difficultyKey);

        FreeTaskManager.Instance.CompleteTask(taskName, isBeta);
        onCompleted.Invoke();
    }
}
