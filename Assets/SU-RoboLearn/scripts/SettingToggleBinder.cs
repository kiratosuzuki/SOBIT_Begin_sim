using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SettingToggleBinder : MonoBehaviour
{
        public SettingKey key;
    public bool invert = false;

    Toggle toggle;

    void Awake() => toggle = GetComponent<Toggle>();

    void Start()
    {
        if (CompetitionSettings.Instance == null) return;

        bool value = CompetitionSettings.Instance.Get(key);
        toggle.SetIsOnWithoutNotify(invert ? !value : value);
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnDestroy()
    {
        if (toggle != null)
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        if (CompetitionSettings.Instance == null) return;
        CompetitionSettings.Instance.Set(key, invert ? !isOn : isOn);
    }
}
