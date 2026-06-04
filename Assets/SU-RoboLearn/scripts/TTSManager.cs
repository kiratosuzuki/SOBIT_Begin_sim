using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Windows / Mac 両対応のシンプルな TTS。
/// Windows : System.Speech (PowerShell 経由)
/// Mac     : say コマンド (-v Kyoko)
/// Speak(text) を呼ぶだけで動作する。
/// </summary>
public class TTSManager : MonoBehaviour
{
    public static TTSManager Instance { get; private set; }

    [Header("Mac の日本語音声名 (say -v <voice>)")]
    public string macVoice = "Kyoko";

    void Awake() => Instance = this;

    public void Speak(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                SpeakWindows(text);
                break;

            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
                SpeakMac(text);
                break;

            default:
                UnityEngine.Debug.LogWarning("TTSManager: 非対応プラットフォーム");
                break;
        }
    }

    void SpeakWindows(string text)
    {
        // エスケープ（シングルクォートをエスケープ）
        string escaped = text.Replace("'", "''");
        Process.Start(new ProcessStartInfo
        {
            FileName               = "powershell",
            Arguments              = $"-Command \"Add-Type -AssemblyName System.Speech; " +
                                     $"(New-Object System.Speech.Synthesis.SpeechSynthesizer).Speak('{escaped}')\"",
            UseShellExecute        = false,
            CreateNoWindow         = true
        });
    }

    void SpeakMac(string text)
    {
        // エスケープ（ダブルクォートをエスケープ）
        string escaped = text.Replace("\"", "\\\"");
        Process.Start(new ProcessStartInfo
        {
            FileName               = "say",
            Arguments              = $"-v {macVoice} \"{escaped}\"",
            UseShellExecute        = false,
            CreateNoWindow         = true
        });
    }
}
