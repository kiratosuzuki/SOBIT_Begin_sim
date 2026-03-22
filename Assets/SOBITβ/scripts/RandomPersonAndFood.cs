using UnityEngine;

public class RandomPersonAndFood : MonoBehaviour
{
    [Header("Person")]
    public GameObject person;
    public Transform personPos0;
    public Transform personPos1;
    public bool useLocal_P_Position = false;

    [Header("Tray (連動オブジェクト)")]
    public GameObject tray;
    public Transform trayPos0;
    public Transform trayPos1;
    public bool useLocal_T_Position = false;

    [Header("Customer")]
    public GameObject customer;
    public Transform cusPos0;
    public Transform cusPos1;
    public Transform cusPos2;
    public bool useLocal_C_Position = false;

    [Header("Foods (parent)")]
    public Transform foodsParent;

    [Header("Behavior")]
    public bool randomizeOnStart = true;
    [Range(0, 1)]
    public int forcedPersonIndex = 0;
    [Range(0, 2)]
    public int forcedcusIndex = 0;

    public bool enableLogs = true;

    // ====== 外部参照用 ======
    public string wantItem;
    public string soldOutItem;
    public string dummyItem;

    public int idx;
    public int cusidx;

    // ====== ★重要：食品インデックス（状態） ======
    int want;
    int soldOut;
    int dummy;

    void Start()
    {
        ChooseItems();

        //=========== 人の位置 ===========
        idx = randomizeOnStart ? Random.Range(0, 2)
                               : Mathf.Clamp(forcedPersonIndex, 0, 1);

        SetPersonPositionByIndex(idx);
        SetTrayPositionByIndex(idx);

        //=========== 客の位置 ===========
        cusidx = randomizeOnStart ? Random.Range(0, 3)
                                  : Mathf.Clamp(forcedcusIndex, 0, 2);

        SetcusPositionByIndex(cusidx);
    }

    //=====================================================
    // 食品ランダム選択
    //=====================================================
    void ChooseItems()
    {
        int n = foodsParent.childCount;

        // 全部非表示
        for (int i = 0; i < n; i++)
            foodsParent.GetChild(i).gameObject.SetActive(false);
        want = Random.Range(0, n);

        do
        {
            soldOut = Random.Range(0, n);
        }
        while (soldOut == want);

        do
        {
            dummy = Random.Range(0, n);
        }
        while (dummy == want || dummy == soldOut);

        wantItem    = foodsParent.GetChild(want).name;
        soldOutItem = foodsParent.GetChild(soldOut).name;
        dummyItem   = foodsParent.GetChild(dummy).name;

        if (enableLogs)
        {
            Debug.Log($"Want: {wantItem}");
            Debug.Log($"SoldOut: {soldOutItem}");
            Debug.Log($"Dummy: {dummyItem}");
        }

        //★特別枠
        customer = foodsParent.GetChild(want).gameObject;
        foodsParent.GetChild(want).gameObject.SetActive(true);
    }

    //=====================================================
    // 表示制御（TalkGoal から呼ぶ用）
    //=====================================================
    public void ShowWantItem()
    {
        foodsParent.GetChild(want).gameObject.SetActive(true);
    }

    public void ShowDummyItem()
    {
        foodsParent.GetChild(dummy).gameObject.SetActive(true);
    }

    public void HideAllItems()
    {
        for (int i = 0; i < foodsParent.childCount; i++)
            foodsParent.GetChild(i).gameObject.SetActive(false);
    }

    //=====================================================
    // 人の移動
    //=====================================================
    public void SetPersonPositionByIndex(int idx)
    {
        if (person == null) return;

        Transform target = (idx == 0) ? personPos0 : personPos1;

        if (useLocal_P_Position)
            person.transform.localPosition = target.localPosition;
        else
            person.transform.position = target.position;

        person.transform.rotation =
            (idx == 0) ? Quaternion.Euler(0f, 180f, 0f)
                       : Quaternion.Euler(0f, 0f, 0f);
    }

    //=====================================================
    // tray の移動
    //=====================================================
    public void SetTrayPositionByIndex(int idx)
    {
        if (tray == null) return;

        Transform target = (idx == 0) ? trayPos0 : trayPos1;

        if (useLocal_T_Position)
            tray.transform.localPosition = target.localPosition;
        else
            tray.transform.position = target.position;

        tray.transform.rotation =
            (idx == 0) ? Quaternion.Euler(0f, 0f, 0f)
                       : Quaternion.Euler(0f, 180f, 0f);
    }

    //=====================================================
    // 客の移動
    //=====================================================
    public void SetcusPositionByIndex(int idx)
    {
        if (customer == null) return;

        Transform target =
            (idx == 0) ? cusPos0 :
            (idx == 1) ? cusPos1 : cusPos2;

        if (useLocal_C_Position)
            customer.transform.localPosition = target.localPosition;
        else
            customer.transform.position = target.position;

        customer.transform.rotation =
            (idx == 2) ? Quaternion.Euler(270f, 0f, 180f)   //0,0,0
                       : Quaternion.Euler(270f, 90f, 0f);   //0,270,0

    }
}
