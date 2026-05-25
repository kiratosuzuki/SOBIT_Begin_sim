using UnityEngine;

public enum SettingKey
{
    SkipDoorOpen,
    SkipRecognizeKitchen,
    SkipRecognizeSoldOut,
    SkipHandleSoldOut,
    SkipObstacleAvoidance,
    DoorOpenBeta,
    MoveToCustomerBeta,
    RecognizeOrderTypeBeta,
}

public class CompetitionSettings : MonoBehaviour
{
    public static CompetitionSettings Instance { get; private set; }

    // ===== スキップ可能タスク =====
    [Header("Skip Settings")]
    public bool skipDoorOpen            = false;
    public bool skipRecognizeKitchen    = false;
    public bool skipRecognizeSoldOut    = false;
    public bool skipHandleSoldOut       = false;
    public bool skipObstacleAvoidance   = false;

    // ===== アルファ(false) / ベータ(true) 選択 =====
    [Header("Difficulty  (false=Alpha / true=Beta)")]
    public bool doorOpenBeta            = false;
    public bool moveToCustomerBeta      = false;
    public bool recognizeOrderTypeBeta  = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        Load();
    }

    void Load()
    {
        foreach (SettingKey k in System.Enum.GetValues(typeof(SettingKey)))
            SetField(k, PlayerPrefs.GetInt(k.ToString(), 0) == 1);

        // 認識スキップが保存されていた場合は対応も強制スキップ
        if (skipRecognizeSoldOut)
            skipHandleSoldOut = true;
    }

    public bool Get(SettingKey k)
    {
        switch (k)
        {
            case SettingKey.SkipDoorOpen:           return skipDoorOpen;
            case SettingKey.SkipRecognizeKitchen:   return skipRecognizeKitchen;
            case SettingKey.SkipRecognizeSoldOut:   return skipRecognizeSoldOut;
            case SettingKey.SkipHandleSoldOut:      return skipHandleSoldOut;
            case SettingKey.SkipObstacleAvoidance:  return skipObstacleAvoidance;
            case SettingKey.DoorOpenBeta:           return doorOpenBeta;
            case SettingKey.MoveToCustomerBeta:     return moveToCustomerBeta;
            case SettingKey.RecognizeOrderTypeBeta: return recognizeOrderTypeBeta;
            default:                                return false;
        }
    }

    public void Set(SettingKey k, bool value)
    {
        SetField(k, value);
        PlayerPrefs.SetInt(k.ToString(), value ? 1 : 0);

        // 認識をスキップする場合は対応も自動でスキップ
        if (k == SettingKey.SkipRecognizeSoldOut && value == true)
        {
            SetField(SettingKey.SkipHandleSoldOut, true);
            PlayerPrefs.SetInt(SettingKey.SkipHandleSoldOut.ToString(), 1);
        }

        PlayerPrefs.Save();
    }

    void SetField(SettingKey k, bool value)
    {
        switch (k)
        {
            case SettingKey.SkipDoorOpen:           skipDoorOpen           = value; break;
            case SettingKey.SkipRecognizeKitchen:   skipRecognizeKitchen   = value; break;
            case SettingKey.SkipRecognizeSoldOut:   skipRecognizeSoldOut   = value; break;
            case SettingKey.SkipHandleSoldOut:      skipHandleSoldOut      = value; break;
            case SettingKey.SkipObstacleAvoidance:  skipObstacleAvoidance  = value; break;
            case SettingKey.DoorOpenBeta:           doorOpenBeta           = value; break;
            case SettingKey.MoveToCustomerBeta:     moveToCustomerBeta     = value; break;
            case SettingKey.RecognizeOrderTypeBeta: recognizeOrderTypeBeta = value; break;
        }
    }
}
