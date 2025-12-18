using UnityEngine;

public class ClerkRobot : MonoBehaviour
{
    public int taskIndex;
    private string[] fruits = new string[]
    {
        "orange", "chocolate", "banana", "apple", "cola"
    };

    public string GetRandomItem()
    {
        int i = Random.Range(0, fruits.Length);
        return fruits[i];
    }
}
