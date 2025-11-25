using UnityEngine;

public class TalkGoal : MonoBehaviour
{
    public ClerkRobot clerk;
    //public int taskIndex;  // このOrderManagerに紐づくタスク番号
    private string reply;  // 最後に出した注文

    // ユーザーからのテキストを処理
    public string HandleUserText(string userText)
    {
        if (clerk == null)
        {
            Debug.LogError("❌ clerk が null");
            return null;
        }
        if (TaskManager.Instance.CurrentIndex == 0)
        {

            if (userText == "きらと")
            {
                Debug.Log("Goal Reached: " + gameObject.name);
                TaskManager.Instance.CompleteCurrentTask();
                reply = null; // タスク完了したら注文をリセット
            }
        }
        else if (TaskManager.Instance.CurrentIndex == 1)
        {

            // 注文リクエスト
            if (userText == "注文お願いします")
            {
                reply = clerk.GetRandomItem();
                Debug.Log($"お客さん: {reply}");
                return reply; // ここを返す
            }
            // 注文の復唱確認
            else if (userText == reply)
            {
                Debug.Log("Goal Reached: " + gameObject.name);
                TaskManager.Instance.CompleteCurrentTask();
                reply = null; // タスク完了したら注文をリセット
            }
        }


        return null; // 返答なしの場合は null
    }
}
