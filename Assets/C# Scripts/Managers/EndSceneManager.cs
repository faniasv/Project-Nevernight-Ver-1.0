using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    public PlayerDataManager playerDataManager;
    public TelemetryNetworkPipeline telemetryNetworkPipeline;


    public void FinalizeSession()
    {
        // 1. Ambil referensi

        if (playerDataManager != null)
        {
            // 2. Update status terakhir
            playerDataManager.SessionData.currentAct = 3; // Tandai sebagai sesi lengkap (Finish)
        }

        if (telemetryNetworkPipeline != null)
        {
            // 3. Kirim data final ke Sheets
            telemetryNetworkPipeline.SendTelemetryData();
            Debug.Log("<color=green>[Telemetry Final]</color> Data Sesi Lengkap berhasil dikirim!");
        }

        // 4. Kembali ke Main Menu
        SceneManager.LoadScene("MainMenu");
    }
}