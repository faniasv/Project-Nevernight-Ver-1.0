using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;

    private Dictionary<string, AudioClip> currentSFXDict = new Dictionary<string, AudioClip>();

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    // Dipanggil setiap ganti Act/Scene
    public void LoadActAudio(AudioData data) {
        if (data == null) {
            Debug.LogWarning("Savilla, kamu lupa masukin file AudioData di scene ini!");
            return;
        }

        // Hanya ganti BGM kalau di AudioData baru ada isinya
        if (data.mainBGM != null && bgmSource != null) {
            PlayBGM(data.mainBGM);
        }

        // Hanya ganti Ambience kalau ada isinya
        if (data.environmentalAmbience != null && ambienceSource != null) {
            PlayAmbience(data.environmentalAmbience);
        }

        // Isi library SFX
        currentSFXDict.Clear();
        if (data.sfxLibrary != null) {
            foreach (var entry in data.sfxLibrary) {
                if (entry.clip != null) {
                    currentSFXDict.Add(entry.name, entry.clip);
                }
            }
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

    public void PlaySFX(string sfxName) {
        if (currentSFXDict.ContainsKey(sfxName)) {
            sfxSource.PlayOneShot(currentSFXDict[sfxName]);
        }
    }
}