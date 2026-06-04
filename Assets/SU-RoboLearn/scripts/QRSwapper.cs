using System.Collections;
using UnityEngine;

/// <summary>
/// このオブジェクトの子に各食品オブジェクト（名前＝食品名、tag="scan"）を置く。
/// Show系メソッドで InitialRandomSettings の食品名と一致する子だけをアクティブにする。
/// DoScan() がアクティブな "scan" タグオブジェクトを検出するので、名前が一致する。
/// TalkCondition / MoveCondition の onCompleted から呼ぶ。
/// </summary>
public class QRSwapper : MonoBehaviour
{
    [Header("表示までの遅延（秒）")]
    public float delayMin = 3f;
    public float delayMax = 5f;

    public void ShowOrder()        => StartCoroutine(ActivateDelayed(InitialRandomSettings.Instance?.wantItem));
    public void ShowOrder2()       => StartCoroutine(ActivateDelayed(InitialRandomSettings.Instance?.wantItem2));
    public void ShowOrder3()       => StartCoroutine(ActivateDelayed(InitialRandomSettings.Instance?.wantItem3));
    public void ShowSoldOut()      => StartCoroutine(ActivateDelayed(InitialRandomSettings.Instance?.soldOutItem));
    public void ShowSoldOut2()     => StartCoroutine(ActivateDelayed(InitialRandomSettings.Instance?.soldOutItem2));
    public void ShowSoldOut3()     => StartCoroutine(ActivateDelayed(InitialRandomSettings.Instance?.soldOutItem3));
    public void ShowDummy()        => StartCoroutine(ActivateDelayed(InitialRandomSettings.Instance?.dummyItem));
    public void ShowCurrentTable() => StartCoroutine(ActivateDelayed(InitialRandomSettings.Instance?.customerTableNo));
    public void HideAll()          => SetAllActive(false);

    IEnumerator ActivateDelayed(string itemName)
    {
        yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
        Activate(itemName);
    }

    void Activate(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return;

        bool found = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            bool match = child.name == itemName;
            child.SetActive(match);
            if (match) found = true;
        }

        if (!found)
            Debug.LogWarning($"QRSwapper: '{itemName}' に一致する子オブジェクトが見つかりません");
    }

    void SetAllActive(bool value)
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(value);
    }
}
