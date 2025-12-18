using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;                         // 取得したスコア
    public TextMeshProUGUI scoreText;             // UI の Text への参照

    void Awake()
    {
        Instance = this; // シングルトンとして保持
    }

    void Start()
    {
        UpdateScoreUI();
    }

    // スコアを加算する関数
    public void AddScore(int amount)
    {
        Debug.Log("AddScore called! amount = " + amount);

        score += amount;
        UpdateScoreUI();
    }


    // UI 表示更新
    void UpdateScoreUI()
    {
        scoreText.text = "Score : " + score;
    }
}
