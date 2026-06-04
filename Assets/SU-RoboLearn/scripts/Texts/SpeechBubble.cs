using TMPro;
using UnityEngine;
using System.Collections;
using System.Text;

public class SpeechBubble : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI text;

    [Header("Timing")]
    public float displayTime = 3f;
    public float typeInterval = 0.05f;

    [Header("Line Break")]
    public int maxCharsPerLine = 8; // ← ここで文字数指定

    Coroutine sayCoroutine;

    public void Say(string message)
    {
        gameObject.SetActive(true);

        if (sayCoroutine != null)
            StopCoroutine(sayCoroutine);

        sayCoroutine = StartCoroutine(SayRoutine(message));
    }

    IEnumerator SayRoutine(string message)
    {
        // 自動改行を挿入
        string formatted = InsertLineBreaks(message, maxCharsPerLine);

        text.text = "";

        // 文字送り
        foreach (char c in formatted)
        {
            text.text += c;
            yield return new WaitForSeconds(typeInterval);
        }

        // 表示時間待機
        yield return new WaitForSeconds(displayTime);

        gameObject.SetActive(false);
    }

    // ============================
    // 自動改行処理
    // ============================
    string InsertLineBreaks(string text, int maxCharsPerLine)
    {
        int count = 0;
        StringBuilder sb = new StringBuilder();

        foreach (char c in text)
        {
            sb.Append(c);
            count++;

            if (count >= maxCharsPerLine)
            {
                sb.Append('\n');
                count = 0;
            }
        }
        return sb.ToString();
    }
}
