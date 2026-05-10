using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro; // WAJIB TAMBAHIN INI

public class IntroSequence : MonoBehaviour
{
    [Header("Panel References")]
    public List<CanvasGroup> panels;
    public float fadeDuration = 1.5f;
    public float waitDuration = 1f;
    public float finalWait = 2.0f;

    [Header("Caption References")]
    public TextMeshProUGUI captionTextDisplay; // Tarik objek TMP Text ke sini
    [TextArea(2, 5)] public List<string> captions; // Isi teksnya di Inspector

    [Header("Scene Navigation")]
    public int nextActNumber = 2; 

    [Header("Events")]
    public UnityEvent OnIntroFinished;

    void Start()
    {
        // Pastikan teks kosong di awal
        if (captionTextDisplay != null) captionTextDisplay.text = "";
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // Sembunyikan semua panel dulu
        foreach (var p in panels) p.alpha = 0;

        for (int i = 0; i < panels.Count; i++)
        {
            // 1. Update Teks Caption (jika ada)
            if (captionTextDisplay != null && i < captions.Count)
            {
                captionTextDisplay.text = captions[i];
                // Opsional: Kalau mau teksnya juga fade in, lo bisa taruh CanvasGroup di objek teksnya
            }

            // 2. Fade In Panel
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                panels[i].alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                yield return null;
            }
            panels[i].alpha = 1;

            // 3. Tunggu sebelum panel berikutnya
            yield return new WaitForSeconds(waitDuration);
        }

        yield return new WaitForSeconds(finalWait);
        OnIntroFinished.Invoke();

        Debug.Log("Intro Selesai. Pindah ke Act: " + nextActNumber);
        GameEvents.OnActChanged?.Invoke(nextActNumber);
    }
}