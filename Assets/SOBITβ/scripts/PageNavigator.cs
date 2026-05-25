using UnityEngine;

public class PageNavigator : MonoBehaviour
{
    public GameObject[] pages;

    int current = 0;

    void Start() => ShowPage(0);

    public void NextPage() => ShowPage(current + 1);
    public void PrevPage() => ShowPage(current - 1);

    void ShowPage(int index)
    {
        current = (index % pages.Length + pages.Length) % pages.Length;
        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(i == current);
    }
}
