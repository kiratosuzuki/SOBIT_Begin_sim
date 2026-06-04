using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーン内の全 TalkCondition にテキストを流すディスパッチャー。
/// ループ前にスナップショットを取り、同一メッセージ内での前提タスク連鎖を防ぐ。
/// </summary>
public class TaskTalkRouter : TalkGoal
{
    TalkCondition[] conditions;

    void Awake()
    {
        conditions = FindObjectsOfType<TalkCondition>();
    }

    public override string HandleUserText(string text)
    {
        // 処理前に実行可能なタスクを確定（連鎖防止）
        var eligible = new HashSet<TalkCondition>();
        foreach (var c in conditions)
            if (c.CanAttemptNow()) eligible.Add(c);

        string lastReply = null;
        foreach (var c in conditions)
        {
            if (!eligible.Contains(c)) continue;
            string reply = c.TryComplete(text);
            if (reply != null) lastReply = reply;
        }
        return lastReply ?? "none";
    }
}
