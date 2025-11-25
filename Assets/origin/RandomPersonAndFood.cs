using UnityEngine;

public class RandomPersonAndFood : MonoBehaviour
{
    [Header("Person")]
    public GameObject person;              // 人オブジェクト（1つ）
    public Transform personPos0;           // 位置0（Transform をインスペクタで置く）
    public Transform personPos1;           // 位置1（Transform をインスペクタで置く）
    public bool useLocalPosition = false;  // trueなら localPosition にセット

    [Header("Foods (parent)")]
    public Transform foodsParent;          // 食べ物の親（子に orange/chocolate/...）

    [Header("Behavior")]
    public bool randomizeOnStart = true;   // Start 時にランダムで決める
    [Range(0,1)]
    public int forcedPersonIndex = 0;      // randomizeOnStart=false のとき使える（0 or 1）
    public bool enableLogs = true;

    void Start()
    {
        // 1) 食べ物の自動選択（親の子を全て取得して1つだけ有効）
        if (foodsParent != null)
        {
            int n = foodsParent.childCount;
            for (int i = 0; i < n; i++)
                foodsParent.GetChild(i).gameObject.SetActive(false);

            if (n > 0)
            {
                int chosenFood = Random.Range(0, n);
                foodsParent.GetChild(chosenFood).gameObject.SetActive(true);
                if (enableLogs) Debug.Log($"Chosen food: {foodsParent.GetChild(chosenFood).name}");
            }
        }

        // 2) 人の位置決め
        int idx = randomizeOnStart ? Random.Range(0, 2) : Mathf.Clamp(forcedPersonIndex, 0, 1);
        SetPersonPositionByIndex(idx);
    }

    /// <summary>
    /// 人を0または1の位置に移動させる。Runtime に呼べる。
    /// </summary>
    public void SetPersonPositionByIndex(int idx)
    {
        if (person == null) return;

        Transform target = idx == 0 ? personPos0 : personPos1;

        // ★ 位置を設定
        if (useLocalPosition)
            person.transform.localPosition = target.localPosition;
        else
            person.transform.position = target.position;

        // ★ 向きを設定（修正ポイント）
        if (idx == 0)
        {
            // pos0 の時 → 180°
            person.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            // pos1 の時 → 0°
            person.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (enableLogs) Debug.Log($"Person moved to pos{idx} and rotated.");
    }


}
