using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    [Header("UI Reference")]
    public Image transitionPanel; // Panel Hitam Fullscreen
    public float transitionTime = 1f;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        // Setiap scene mulai, pastikan layar terang (Fade Out Hitam)
        if (transitionPanel != null)
        {
            transitionPanel.gameObject.SetActive(true);
            StartCoroutine(FadeOut());
        }
    }

    // Panggil fungsi ini dari Puzzle Manager saat "Give Up"
    public void LoadNextScene(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        // 1. Fade In (Layar jadi Gelap)
        if (transitionPanel != null)
        {
            transitionPanel.gameObject.SetActive(true);
            for (float t = 0; t < 1; t += Time.deltaTime / transitionTime)
            {
                transitionPanel.color = new Color(0, 0, 0, t);
                yield return null;
            }
            transitionPanel.color = new Color(0, 0, 0, 1);
        }

        // 2. Tunggu sebentar biar dramatis
        yield return new WaitForSeconds(0.5f);

        // 3. Pindah Scene (Scene lama mati, Scene baru load)
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOut()
    {
        // Fade Out (Layar jadi Terang)
        for (float t = 1; t > 0; t -= Time.deltaTime / transitionTime)
        {
            transitionPanel.color = new Color(0, 0, 0, t);
            yield return null;
        }

        // Matikan panel biar bisa diklik game-nya
        if (transitionPanel != null) transitionPanel.gameObject.SetActive(false);
    }
}