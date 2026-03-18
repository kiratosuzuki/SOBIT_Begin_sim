using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void RestartGame()
    {
        Debug.Log("ボタン押された");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}