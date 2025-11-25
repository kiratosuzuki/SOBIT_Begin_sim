using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClerkAudio : MonoBehaviour
{
    public AudioSource audioSource;

    // 文字列 → AudioClip の辞書
    public List<StringAudio> audioList;

    [System.Serializable]
    public struct StringAudio
    {
        public string key;
        public AudioClip clip;
    }

    private Dictionary<string, AudioClip> clipDict;

    void Awake()
    {
        clipDict = new Dictionary<string, AudioClip>();
        foreach (var item in audioList)
        {
            if (item.clip != null && !clipDict.ContainsKey(item.key))
                clipDict.Add(item.key, item.clip);
        }
    }

    public IEnumerator Speak(string text)
    {
        Debug.Log($"店員（Audio）: {text}");

        if (clipDict.TryGetValue(text, out AudioClip clip))
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(clip.length);
        }
        else
        {
            Debug.LogWarning($"AudioClip が見つかりません: {text}");
        }
    }
}
