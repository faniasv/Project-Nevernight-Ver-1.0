using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("UI Reference")]
    public Image transitionPanel;

    [Header("Settings")]
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

    private void Start()
    {
        // Buka tirai jadi terang saat game pertama kali dijalankan (Main Menu / Intro)
        if (transitionPanel != null)
        {
            transitionPanel.color = new Color(0, 0, 0, 1f); // Set hitam pekat
            transitionPanel.gameObject.SetActive(true);
            StartCoroutine(Fade(0f)); // Perlahan jadi transparan
        }
    }

    private void OnEnable()
    {
        GameEvents.OnActChanged += LoadAct;
    }

    private void OnDisable()
    {
        GameEvents.OnActChanged -= LoadAct;
    }

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
            // Perbarui data Act di dalam session telemetri sebelum pindah scene
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.SessionData.currentAct = actNumber;
            }
            StartCoroutine(FadeAndLoad(targetScene));
        }
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        if (transitionPanel != null) 
        {
            transitionPanel.color = new Color(0, 0, 0, 0);
            transitionPanel.gameObject.SetActive(true);
        }

        // 1. Tutup Tirai Hitam (Target Alpha maksimal adalah 1.0f, bukan 2f)
        yield return StartCoroutine(Fade(1f)); 

        SceneManager.LoadScene(sceneName);

        yield return new WaitForSeconds(0.3f); 

        // 2. Buka Tirai Kembali ke Terang (Target Alpha 0f)
        yield return StartCoroutine(Fade(0f));

        if (transitionPanel != null) transitionPanel.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (transitionPanel == null) yield break;

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

        transitionPanel.color = new Color(0, 0, 0, targetAlpha);
        
        if (targetAlpha == 0) transitionPanel.gameObject.SetActive(false);
    }
}