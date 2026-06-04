using System.Collections;
using UnityEngine;

/// <summary>
/// 指定した SettingKey が true のとき、タスクをスキップしオブジェクトを無効化する。
/// FreeTaskManager.Start() より後に実行するため 1 フレーム待つ。
/// </summary>
public class SettingSkipper : MonoBehaviour
{
    public SettingKey key;

    [Header("スキップするタスク名")]
    public string[] taskNamesToSkip;

    [Header("無効化するオブジェクト")]
    public GameObject[] objectsToDisable;

    IEnumerator Start()
    {
        yield return null;

        if (CompetitionSettings.Instance == null) yield break;
        if (!CompetitionSettings.Instance.Get(key)) yield break;

        foreach (var taskName in taskNamesToSkip)
            FreeTaskManager.Instance.SkipTask(taskName);

        foreach (var obj in objectsToDisable)
            if (obj != null) obj.SetActive(false);
    }
}
