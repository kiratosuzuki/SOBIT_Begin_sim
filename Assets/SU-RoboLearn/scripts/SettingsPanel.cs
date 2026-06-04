using UnityEngine;

/// <summary>
/// 設定パネルの表示・非表示を管理する。
/// ハンバーガーボタンが2つある前提（openButton / closeButton を入れ替える）。
/// </summary>
public class SettingsPanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public GameObject openButton;
    public GameObject closeButton;

    void Start()
    {
        settingsPanel.SetActive(false);
        openButton.SetActive(true);
        closeButton.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        openButton.SetActive(false);
        closeButton.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        openButton.SetActive(true);
        closeButton.SetActive(false);
    }
}
