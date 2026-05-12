using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;

public class IntroSequence : MonoBehaviour
{
    [System.Serializable]
    public struct IntroStep // Satu paket untuk tiap adegan opening
    {
        public CanvasGroup panel;
        [TextArea(2, 5)] public string caption;
        public string sfxName; // Nama SFX yang ada di AudioData kamu
        public AudioClip ambienceClip; // Opsional: jika ingin ambience spesifik yang loop
    }

    [Header("Sequence Settings")]
    public List<IntroStep> introSteps;
    public float fadeDuration = 1.5f;
    public float waitDuration = 1f;
    public float finalWait = 2.0f;

    [Header("UI References")]
    public TextMeshProUGUI captionTextDisplay;

    [Header("Scene Navigation")]
    public int nextActNumber = 2; 

    [Header("Events")]
    public UnityEvent OnIntroFinished;

    void Start()
    {
        if (captionTextDisplay != null) captionTextDisplay.text = "";
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // Sembunyikan semua panel dulu
        foreach (var step in introSteps) step.panel.alpha = 0;

        for (int i = 0; i < introSteps.Count; i++)
        {
            IntroStep currentStep = introSteps[i];

            // 1. Play Audio (SFX atau Ambience)
            if (!string.IsNullOrEmpty(currentStep.sfxName) && AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(currentStep.sfxName);
            }
            
            // [Optional] Jika ada ambience spesifik per gambar
            if (currentStep.ambienceClip != null && AudioManager.instance != null)
            {
                AudioManager.instance.PlayAmbience(currentStep.ambienceClip);
            }

            // 2. Update Teks
            if (captionTextDisplay != null)
            {
                captionTextDisplay.text = currentStep.caption;
            }

            // 3. Fade In Panel
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                currentStep.panel.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                yield return null;
            }
            currentStep.panel.alpha = 1;

            // 4. Tunggu
            yield return new WaitForSeconds(waitDuration);
        }

        yield return new WaitForSeconds(finalWait);
        OnIntroFinished.Invoke();
        GameEvents.OnActChanged?.Invoke(nextActNumber);
    }
}