using UnityEngine;
using UnityEngine.UI;

public class MultiCameraView : MonoBehaviour
{
    public static MultiCameraView Instance { get; private set; }

    [Header("Panel")]
    public GameObject panel;

    [Header("Active Camera (俯瞰モード時にLookAtが参照するカメラ)")]
    public Camera activeCamera;

    public bool IsActive => panel != null && panel.activeSelf;

    [Header("Render Textures")]
    public RenderTexture mainTexture;
    public RenderTexture subTexture1;
    public RenderTexture subTexture2;

    [Header("RawImages")]
    public RawImage mainView;
    public RawImage subView1;
    public RawImage subView2;

    void Awake() => Instance = this;

    void Start()
    {
        if (mainView  != null) mainView.texture  = mainTexture;
        if (subView1  != null) subView1.texture  = subTexture1;
        if (subView2  != null) subView2.texture  = subTexture2;
    }

    public void Toggle()
    {
        if (panel != null)
        {
            Debug.Log($"[MultiCameraView] Toggle called. Before: {panel.activeSelf}");
            panel.SetActive(!panel.activeSelf);
            Debug.Log($"[MultiCameraView] Toggle done. After: {panel.activeSelf}");
        }
    }

    public void Show() { if (panel != null) panel.SetActive(true); }
    public void Hide() { if (panel != null) panel.SetActive(false); }
}
