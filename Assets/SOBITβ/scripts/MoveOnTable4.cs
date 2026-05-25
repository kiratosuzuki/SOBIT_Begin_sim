using System.Collections;
using UnityEngine;

/// <summary>
/// cusidx == 3（テーブル4）のときだけ cusPos3 の位置に自分自身を移動する。
/// アタッチするだけで動作する。Inspector 設定不要。
/// </summary>
public class MoveOnTable4 : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null; // InitialRandomSettings.Start() の完了を待つ

        var r = InitialRandomSettings.Instance;
        if (r == null || r.cusidx != 3) yield break;

        transform.position = r.cusPos3.position;
        transform.Rotate(0f, 0f, 180f);
    }
}
