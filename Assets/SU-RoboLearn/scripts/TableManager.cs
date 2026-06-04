using System.Collections;
using UnityEngine;

/// <summary>
/// シーン上の静的テーブルQRオブジェクトの表示を管理する。
/// TalkCondition / MoveCondition の onCompleted から呼ぶ。
/// </summary>
public class TableManager : MonoBehaviour
{
    public static TableManager Instance { get; private set; }

    [Header("テーブルオブジェクト")]
    public GameObject table1;
    public GameObject table2;
    public GameObject table3;

    void Awake() => Instance = this;

    /// <summary>InitialRandomSettings.customerTableNo に一致するテーブルだけ表示</summary>
    public void ShowCurrentTable()
    {
        string current = InitialRandomSettings.Instance?.customerTableNo;
        SetActive(table1, "テーブル1" == current);
        SetActive(table2, "テーブル2" == current);
        SetActive(table3, "テーブル3" == current);
    }

    /// <summary>現在の客席テーブルだけ非表示にする</summary>
    public void HideCurrentTable()
    {
        string current = InitialRandomSettings.Instance?.customerTableNo;
        SetActive(table1, "テーブル1" != current);
        SetActive(table2, "テーブル2" != current);
        SetActive(table3, "テーブル3" != current);
    }

    public void ShowAll()
    {
        SetActive(table1, true);
        SetActive(table2, true);
        SetActive(table3, true);
    }

    public void HideAll()
    {
        SetActive(table1, false);
        SetActive(table2, false);
        SetActive(table3, false);
    }

    public void ShowTable1() => SetActive(table1, true);
    public void ShowTable2() => SetActive(table2, true);
    public void ShowTable3() => SetActive(table3, true);

    public void HideTable1() => SetActive(table1, false);
    public void HideTable2() => SetActive(table2, false);
    public void HideTable3() => SetActive(table3, false);

    void SetActive(GameObject obj, bool value)
    {
        if (obj != null) obj.SetActive(value);
    }
}
