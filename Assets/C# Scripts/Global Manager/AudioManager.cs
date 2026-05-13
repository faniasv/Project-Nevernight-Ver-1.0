using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    [Header("Global Data")]
    public AudioData globalData;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;

    private Dictionary<string, AudioClip> globalSFXDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> actSFXDict = new Dictionary<string, AudioClip>();

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGlobalAudio(); // Load suara UI global di awal
        } else {
            Destroy(gameObject);
        }
    }

    private void InitializeGlobalAudio() {
        if (globalData != null) {
            globalSFXDict.Clear();
            foreach (var entry in globalData.sfxLibrary) {
                if (entry.clip != null) globalSFXDict.Add(entry.name, entry.clip);
            }
        }
    }

    // Dipanggil setiap ganti Act/Scene
    public void LoadActAudio(AudioData data) {
        if (data == null) return;

        // BGM & Ambience logic
        if (data.mainBGM != null) PlayBGM(data.mainBGM);
        if (data.environmentalAmbience != null) PlayAmbience(data.environmentalAmbience);

        // Update Kamus SFX khusus Act ini
        actSFXDict.Clear();
        foreach (var entry in data.sfxLibrary) {
            if (entry.clip != null) actSFXDict.Add(entry.name, entry.clip);
        }
    }

    public void PlaySFX(string sfxName) {
        // ALUR BACA: Cek Act dulu (Spesifik), kalau gak ada cek Global (Umum)
        if (actSFXDict.ContainsKey(sfxName)) {
            sfxSource.PlayOneShot(actSFXDict[sfxName]);
        } 
        else if (globalSFXDict.ContainsKey(sfxName)) {
            sfxSource.PlayOneShot(globalSFXDict[sfxName]);
        }
        else {
            Debug.LogWarning("Suara " + sfxName + " tidak ketemu di mana-mana!");
        }
    }

    public IEnumerator FadeBGM(float targetVolume, float duration)
    {
        float startVolume = bgmSource.volume;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            yield return null;
        }
        bgmSource.volume = targetVolume;
    }

    public void PlayBGM(AudioClip clip) {
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlayAmbience(AudioClip clip) {
        if (ambienceSource.clip == clip) return;
        ambienceSource.clip = clip;
        ambienceSource.Play();
    }

}