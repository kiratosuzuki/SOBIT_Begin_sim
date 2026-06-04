using UnityEngine;
using UnityEngine.UI;

public class CameraSpeedSettings : MonoBehaviour
{
    [Header("Camera")]
    public FreeCameraController cameraController;

    [Header("Sliders")]
    public Slider moveSpeedSlider;
    public Slider lookSpeedSlider;

    void OnEnable()
    {
        if (cameraController == null)
            cameraController = FindObjectOfType<FreeCameraController>();

        if (cameraController == null)
        {
            Debug.LogError("[CameraSpeedSettings] FreeCameraController が見つかりません。Camera Controller フィールドに手動でアサインしてください。");
            return;
        }

        if (moveSpeedSlider == null || lookSpeedSlider == null)
        {
            Debug.LogError("[CameraSpeedSettings] スライダーが未アサインです。インスペクターで設定してください。");
            return;
        }

        moveSpeedSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("CamMoveSpeed", 5f));
        lookSpeedSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("CamLookSpeed", 5f));

        moveSpeedSlider.onValueChanged.RemoveListener(OnMoveSpeedChanged);
        moveSpeedSlider.onValueChanged.AddListener(OnMoveSpeedChanged);

        lookSpeedSlider.onValueChanged.RemoveListener(OnLookSpeedChanged);
        lookSpeedSlider.onValueChanged.AddListener(OnLookSpeedChanged);

        ApplyAll();
    }

    void OnDisable()
    {
        if (moveSpeedSlider != null) moveSpeedSlider.onValueChanged.RemoveListener(OnMoveSpeedChanged);
        if (lookSpeedSlider != null) lookSpeedSlider.onValueChanged.RemoveListener(OnLookSpeedChanged);
    }

    void ApplyAll()
    {
        if (cameraController == null) return;
        cameraController.moveSpeed = moveSpeedSlider.value;
        cameraController.lookSpeed = lookSpeedSlider.value;
    }

    public void OnMoveSpeedChanged(float value)
    {
        PlayerPrefs.SetFloat("CamMoveSpeed", value);
        PlayerPrefs.Save();
        if (cameraController != null) cameraController.moveSpeed = value;
    }

    public void OnLookSpeedChanged(float value)
    {
        PlayerPrefs.SetFloat("CamLookSpeed", value);
        PlayerPrefs.Save();
        if (cameraController != null) cameraController.lookSpeed = value;
    }
}
