using UnityEngine;

public class ActAudioLoader : MonoBehaviour {
    public AudioData actData;

    void Start() {
        if (AudioManager.instance != null && actData != null) {
            AudioManager.instance.LoadActAudio(actData);
        }
    }
}