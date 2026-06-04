using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// キーワードグループ全てにマッチしたときにタスクを達成する条件コンポーネント。
/// 各グループ内はOR判定（どれか1つ含まれればOK）、グループ間はAND判定（全グループ必須）。
/// キーワードにはプレースホルダー {wantItem} {soldOutItem} {soldOutItem2} {soldOutItem3} {dummyItem} {kitchenSide} {customerTable} を使用可能。
/// </summary>
public class TalkCondition : MonoBehaviour
{
    [System.Serializable]
    public class KeywordGroup
    {
        [Tooltip("このグループ内のいずれか1つが入力に含まれればOK（OR）")]
        public string[] keywords;
    }

    [Header("Task")]
    public string taskName;

    [Header("Condition  ※グループ間はAND・グループ内はOR  ※{wantItem}{wantItem2}{wantItem3}{soldOutItem}{soldOutItem2}{soldOutItem3}{dummyItem}{kitchenSide}{customerTable}")]
    public KeywordGroup[] keywordGroups;

    [Header("Reply (空欄なら返答なし)  ※プレースホルダー使用可")]
    public string replyText;
    public SpeechBubble npcBubble;
    public float npcBubbleDelay = 4f;

    [Header("Difficulty (アルファ/ベータでスコアを変える場合)")]
    public bool hasDifficulty = false;
    public SettingKey difficultyKey;

    [Header("Prerequisites (前提タスク名、任意)")]
    public string[] prerequisites;

    [Header("On Completed")]
    public UnityEvent onCompleted;

    public bool CanAttemptNow()
        => !FreeTaskManager.Instance.IsCompleted(taskName)
        && FreeTaskManager.Instance.CanAttempt(prerequisites);

    public string TryComplete(string text)
    {
        if (!IsMatch(text)) return null;
        if (FreeTaskManager.Instance.IsCompleted(taskName)) return null;

        bool isBeta = hasDifficulty
            && CompetitionSettings.Instance != null
            && CompetitionSettings.Instance.Get(difficultyKey);

        FreeTaskManager.Instance.CompleteTask(taskName, isBeta);
        onCompleted.Invoke();

        string reply = Resolve(replyText);
        if (!string.IsNullOrEmpty(reply) && npcBubble != null)
            StartCoroutine(SayDelayed(reply));
        return string.IsNullOrEmpty(reply) ? null : reply;
    }

    bool IsMatch(string text)
    {
        if (keywordGroups == null || keywordGroups.Length == 0) return false;

        foreach (var group in keywordGroups)
        {
            if (group.keywords == null || group.keywords.Length == 0) continue;

            bool anyMatch = false;
            foreach (var kw in group.keywords)
            {
                if (!string.IsNullOrEmpty(kw) && text.Contains(Resolve(kw)))
                {
                    anyMatch = true;
                    break;
                }
            }
            if (!anyMatch) return false;
        }
        return true;
    }

    System.Collections.IEnumerator SayDelayed(string reply)
    {
        yield return new WaitForSeconds(npcBubbleDelay);
        npcBubble.Say(reply);
        TTSManager.Instance?.Speak(reply);
    }

    static string Resolve(string template)
    {
        if (string.IsNullOrEmpty(template)) return template;

        var r = InitialRandomSettings.Instance;
        if (r == null) return template;

        return template
            .Replace("{wantItem}",      r.wantItem)
            .Replace("{wantItem2}",     r.wantItem2)
            .Replace("{wantItem3}",     r.wantItem3)
            .Replace("{soldOutItem}",   r.soldOutItem)
            .Replace("{soldOutItem2}",  r.soldOutItem2)
            .Replace("{soldOutItem3}",  r.soldOutItem3)
            .Replace("{dummyItem}",     r.dummyItem)
            .Replace("{kitchenSide}",   r.kitchenSide)
            .Replace("{customerTable}", r.customerTableNo);
    }
}
