using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ada

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    public string introSceneName = "SC_Intro"; // Nama scene tujuan

    public void StartGame()
    {
        Debug.Log("Memulai Game...");
        SceneManager.LoadScene(introSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Keluar Game...");
        Application.Quit();
    }
}