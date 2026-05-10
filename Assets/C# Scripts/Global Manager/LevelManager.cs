using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header ("UI Reference")]
    // Image utk transisi antar Scene nanti
    public Image transitionPanel;

    [Header ("Settings")]
    public float transitionTime = 1.0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (transitionPanel != null)
        {
            transitionPanel.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // Jika ada perintah OnActChanged, panggil fungsi LoadAct
        GameEvents.OnActChanged += LoadAct;
    }

    private void OnDisable()
    {
        // Selalu lepas pendaftaran saat script mati untuk mencegah error
        GameEvents.OnActChanged -= LoadAct;
    }

    // Fungsi untuk menerjemahkan Angka Act menjadi Nama Scene di Unity.
    public void LoadAct(int actNumber)
    {
        string targetScene = "";

        switch (actNumber)
        {
            case 0: targetScene = "SC_MainMenu"; break;
            case 1: targetScene = "SC_Intro"; break;
            case 2: targetScene = "SC_Act1"; break;
            case 3: targetScene = "SC_Act2"; break;
            case 4: targetScene = "SC_Act3"; break;
            case 5: targetScene = "SC_Act4"; break;
            default: targetScene = "SC_MainMenu"; break;
        }

        if (!string.IsNullOrEmpty(targetScene))
        {
            // Jalankan mesin transisinya
            StartCoroutine(FadeAndLoad(targetScene));
        }
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        if (transitionPanel != null) transitionPanel.gameObject.SetActive(true);

        // 1. Mulai menutup tirai (Layar jadi hitam)
        yield return StartCoroutine(Fade(1)); 

        // 2. Perintah Unity untuk pindah scene
        SceneManager.LoadScene(sceneName);

        // 3. Jeda sebentar (Opsional, agar transisi tidak terlalu terburu-buru)
        yield return new WaitForSeconds(0.5f);

        // 4. Mulai membuka tirai (Layar jadi terang)
        yield return StartCoroutine(Fade(0));

        if (transitionPanel != null) transitionPanel.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (transitionPanel == null) yield break;

        // Pastikan panel aktif saat proses fade
        transitionPanel.gameObject.SetActive(true);
        
        float startAlpha = transitionPanel.color.a;
        float timer = 0;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer / transitionTime);
            transitionPanel.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        // Pastikan nilai akhir tepat
        transitionPanel.color = new Color(0, 0, 0, targetAlpha);
        
        // Jika layar sudah terang (Alpha 0), matikan panel agar tidak menghalangi klik mouse pemain
        if (targetAlpha == 0) transitionPanel.gameObject.SetActive(false);
    }
}
