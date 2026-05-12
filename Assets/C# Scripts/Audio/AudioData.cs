using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAudioData", menuName = "Nevernight/Audio Data")]
public class AudioData : ScriptableObject {
    [Header("Background Music")]
    public AudioClip mainBGM;
    public AudioClip puzzleBGM; 

    [Header("Ambience")]
    public AudioClip environmentalAmbience;

    [Header("Sound Effects Library")]
    public List<SFXEntry> sfxLibrary;

    [System.Serializable]
    public struct SFXEntry {
        public string name;
        public AudioClip clip;
    }
}