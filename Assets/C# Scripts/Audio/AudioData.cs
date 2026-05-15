using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAudioData", menuName = "Nevernight/Audio Data")]
public class AudioData : ScriptableObject {
    [Header("Background Music")]
    public AudioClip mainBGM;

    [Header("Primary Ambience")]
    [Tooltip("Ambience utama yang otomatis jalan dan nge-loop di latar belakang")]
    public AudioClip environmentalAmbience;

    [Header("Secondary Ambience Library")]
    [Tooltip("Daftar ambience tambahan (misal: suara hujan, dengung mesin, dll)")]
    public List<SFXEntry> extraAmbienceLibrary;

    [Header("Sound Effects Library")]
    public List<SFXEntry> sfxLibrary;

    [System.Serializable]
    public struct SFXEntry {
        public string name;
        public AudioClip clip;
    }
}