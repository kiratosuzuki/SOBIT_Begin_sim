using UnityEngine;

public class TalkGoal : MonoBehaviour
{
    public RandomPersonAndFood randomManager;
    public GameObject target; 
    public SpeechBubble wbubble;
    public SpeechBubble cbubble;

    //public int taskIndex;  // このOrderManagerに紐づくタスク番号
    private string reply;  // 最後に出した注文

    public Transform doorRoot;   // 回転させたいオブジェクト
    public float duration = 1f;  // 開くまでの時間（秒）

    float t = 0f;
    bool isPlaying = false;

    // ユーザーからのテキストを処理
    public string HandleUserText(string userText)
    {
        if (randomManager == null)
        {
            Debug.LogError("❌ randomManager が null");
            return null;
        }

        if (TaskManager.Instance.CurrentIndex == 0)
        {
            if (userText == "ドアを開けてください")
            {

                TaskManager.Instance.CompleteCurrentTask();

                isPlaying = true;

                reply = "none"; // タスク完了したら注文をリセット
            }
        }
        else if (TaskManager.Instance.CurrentIndex == 1)
        {
            if (randomManager.idx == 0)
            {
                if (userText == "キッチンは左です")
                {
                    TaskManager.Instance.CompleteCurrentTask();

                    isPlaying = true;

                    reply = "none"; // タスク完了したら注文をリセット
                }
            }
            else
            {
                if (userText == "キッチンは右です")
                {
                    TaskManager.Instance.CompleteCurrentTask();

                    isPlaying = true;

                    reply = "none"; // タスク完了したら注文をリセット
                }
            }
        }
        else if (TaskManager.Instance.CurrentIndex == 2)
        {
            // 注文リクエスト
            if (userText == "売り切れ商品は何ですか")
            {
                reply = randomManager.soldOutItem;
                wbubble.Say(reply);
                Debug.Log($"店員: {reply}");
                return reply; // ここを返す
            }
            // 注文の復唱確認
            else if (userText == $"売り切れ商品は{reply}")
            {
                TaskManager.Instance.CompleteCurrentTask();
                reply = "none";
            }
        }
        else if (TaskManager.Instance.CurrentIndex == 3)
        {
            // 注文リクエスト
            if (userText == "テーブルは何番ですか")
            {
                reply = (randomManager.cusidx + 1).ToString();
                wbubble.Say(reply);
                Debug.Log($"店員: {reply}");
                return reply;
            }
            // 注文の復唱確認
            else if (userText == $"テーブル{reply}に向かいます")
            {
                TaskManager.Instance.CompleteCurrentTask();
                reply = "none";
            }
        }
        else if (TaskManager.Instance.CurrentIndex == 5)
        {
            // 注文リクエスト
            if (userText == "注文は何ですか")
            {
                int rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    reply = randomManager.soldOutItem;
                    cbubble.Say(reply);
                    Debug.Log($"客: {reply}");
                    return reply;
                }
                else
                {
                    reply = randomManager.wantItem;
                    cbubble.Say(reply);
                    Debug.Log($"客: {reply}");
                    return reply;                    
                }
            }
            else if (userText =="別の商品を注文してください")
            {
                reply = randomManager.wantItem;
                cbubble.Say(reply);
                Debug.Log($"客: {reply}");
                return reply;
            }

            // 注文の復唱確認
            else if (userText == $"{reply}確認しました")
            {
                TaskManager.Instance.CompleteCurrentTask();
                reply = "none";
            }
            
        }
        else if (TaskManager.Instance.CurrentIndex == 6)
        {
            // 注文リクエスト
            if (userText == $"{randomManager.wantItem}を置いてください")
            {
                target.tag = "Untagged";
                int rand = Random.Range(0, 2);
                if (rand == 0)
                    randomManager.ShowWantItem();
                else
                    randomManager.ShowDummyItem();
            }
            // 注文の復唱確認
            else if (userText == "商品が違います")
            {
                randomManager.HideAllItems();
                randomManager.ShowWantItem();
            }
            else if (userText ==$"{randomManager.wantItem}確認しました")
            {
                TaskManager.Instance.CompleteCurrentTask();
            }
        }

        return "none"; // 返答不要の場合はnone
    }

    void Update()
    {
        if (isPlaying)
        {
            t += Time.deltaTime / duration;
            t = Mathf.Clamp01(t);

            float angle = Mathf.Lerp(0f, 90f, t);
            doorRoot.localRotation = Quaternion.Euler(0, 0, angle);

            // 完了したら止める
            if (t >= 1f)
            {
                doorRoot.tag = "Untagged";
                isPlaying = false;
            }
        }
    }
}
