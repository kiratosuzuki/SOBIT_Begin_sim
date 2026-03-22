using UnityEngine;
using System.Collections;

public class TalkGoal2 : TalkGoal
{
    public RandomPersonAndFood randomManager;
    public GameObject target; 
    public GameObject target2; 
    public GameObject target3; 
    public GameObject target4; 

    private string reply;  

    public Transform doorRoot;   
    public float duration = 1f;  

    float t = 0f;
    bool isPlaying = false;

    // ===== 遅延処理 =====
    IEnumerator ActivateWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        target3.SetActive(true);
        target.SetActive(false);
        target2.SetActive(true);
    }

    IEnumerator DeactivateWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        target3.SetActive(false);
    }

    // ===== ユーザー入力処理 =====
    public override string HandleUserText(string userText)
    {
        if (userText == randomManager.wantItem)
        {
            FreeTaskManager.Instance.CompleteTask("オレオ");
            reply = "none";
        }

        if (userText == $"{randomManager.wantItem}を置いてください")
        {
            FreeTaskManager.Instance.CompleteTask("商品を置く");

            // ★ここで遅延実行
            StartCoroutine(ActivateWithDelay(5f));

            reply = "none";
        }

        if (userText == "置きましたか")
        {
            FreeTaskManager.Instance.CompleteTask("確認");
            reply = "none";
        }

        if (userText == "商品をとってください")
        {
            StartCoroutine(DeactivateWithDelay(2f));
            FreeTaskManager.Instance.CompleteTask("届ける");
            reply = "none";
        }

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
                target.SetActive(false);
                reply = "none";
            }
        }
        else if (TaskManager.Instance.CurrentIndex == 1)
        {
            if (userText == "キッチンは左です")
            {
                TaskManager.Instance.CompleteCurrentTask();
                reply = "none";
            }
        }
        else if (TaskManager.Instance.CurrentIndex == 2)
        {
            if (userText == "売り切れ商品は何ですか")
            {
                target2.SetActive(false);
                target3.SetActive(true);
            }
            else if (userText == "売り切れ商品はコーヒー")
            {
                TaskManager.Instance.CompleteCurrentTask();
                reply = "none";
            }
        }
        else if (TaskManager.Instance.CurrentIndex == 3)
        {
            if (userText == "テーブルは何番ですか")
            {
                target3.SetActive(false);
                target4.SetActive(true); 
            }
            else if (userText == "テーブル3に向かいます")
            {
                TaskManager.Instance.CompleteCurrentTask();
                reply = "none";
            }
        }
        else if (TaskManager.Instance.CurrentIndex == 5)
        {
            if (userText == "注文は何ですか")
            {
                int rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    reply = randomManager.soldOutItem;
                    Debug.Log($"客: {reply}");
                    return reply;
                }
                else
                {
                    reply = randomManager.wantItem;
                    Debug.Log($"客: {reply}");
                    return reply;                    
                }
            }
            else if (userText == "別の商品を注文してください")
            {
                reply = randomManager.wantItem;
                Debug.Log($"客: {reply}");
                return reply;
            }
            else if (userText == $"{reply}確認しました")
            {
                TaskManager.Instance.CompleteCurrentTask();
                reply = "none";
            }
        }
        else if (TaskManager.Instance.CurrentIndex == 6)
        {
            if (userText == $"{randomManager.wantItem}を置いてください")
            {
                target.tag = "Untagged";

                int rand = Random.Range(0, 2);
                if (rand == 0)
                    randomManager.ShowWantItem();
                else
                    randomManager.ShowDummyItem();
            }
            else if (userText == "商品が違います")
            {
                randomManager.HideAllItems();
                randomManager.ShowWantItem();
            }
            else if (userText == $"{randomManager.wantItem}確認しました")
            {
                TaskManager.Instance.CompleteCurrentTask();
            }
        }

        return "none";
    }

    // ===== ドアアニメーション =====
    void Update()
    {
        if (isPlaying)
        {
            t += Time.deltaTime / duration;
            t = Mathf.Clamp01(t);

            float angle = Mathf.Lerp(0f, 90f, t);
            doorRoot.localRotation = Quaternion.Euler(0, 0, angle);

            if (t >= 1f)
            {
                doorRoot.tag = "Untagged";
                isPlaying = false;
            }
        }
    }
}