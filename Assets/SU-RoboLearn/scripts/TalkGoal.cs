using UnityEngine;

public abstract class TalkGoal : MonoBehaviour
{
    // 会話入力に対する唯一の責務
    public abstract string HandleUserText(string userText);

    // 必要なら共通ライフサイクル
    public virtual void OnTaskChanged(int taskIndex) { }
}
