using UnityEngine;
using UnityEngine.UI;

public class ToggleTest : MonoBehaviour
{
    public GameObject target;
    private Toggle toggle;

    void Start()
    {
        toggle = GetComponent<Toggle>();

        int saved = PlayerPrefs.GetInt("RobotToggle", 1);
        toggle.isOn = (saved == 1);

        target.SetActive(toggle.isOn);
    }

    public void OnToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt("RobotToggle", isOn ? 1 : 0);

        target.SetActive(isOn);
    }
}