using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TelemetryNetworkPipeline : MonoBehaviour
{
    // Boleh biarkan Instance ini aktif sebagai backup sistem global kamu
    public static TelemetryNetworkPipeline Instance { get; private set; }

    [Header("Konfigurasi API Google Sheets")]
    public string webAppUrl = "https://script.google.com/macros/s/AKfycbypEw8M8nTgwU3oUF6WRLApKyMtjEGWk7NsMW8gZk3tNkUgNjXQOq3StNU99VJdsCqytQ/exec";

    // 1. INJEKSI VARIABEL MANUAL INSPECTOR
    [Header("Data Reference")]
    public PlayerDataManager playerDataManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SendTelemetryData()
    {
        Debug.Log("<color=cyan>[Telemetry Pipeline]</color> Fungsi SendTelemetryData() BERHASIL DIPANGGIL!");

        // 2. CEK VARIABEL MANUAL TERLEBIH DAHULU
        PlayerData currentData = null;
        
        if (playerDataManager != null)
        {
            currentData = playerDataManager.SessionData;
        }
        else if (PlayerDataManager.Instance != null) // Backup kalau di Inspector lupa ditarik
        {
            currentData = PlayerDataManager.Instance.SessionData;
        }

        if (currentData == null)
        {
            Debug.LogError("[Telemetry Pipeline] Gagal! Referensi PlayerDataManager tidak ditemukan.");
            return;
        }
        
        // 3. Hitung Skala Keparahan Overthinking
        currentData.ovtSeverityScale = Mathf.Max(0, currentData.failedAttempts - 3);

        // 4. Ubah objek C# menjadi string JSON
        string jsonPayload = JsonUtility.ToJson(currentData);
        Debug.Log($"<color=yellow>[Telemetry Pipeline]</color> JSON siap dikirim: {jsonPayload}");
        
        StartCoroutine(PostToGoogleSheets(jsonPayload));
    }

    private IEnumerator PostToGoogleSheets(string json)
    {
        Debug.Log($"<color=orange>[Telemetry Pipeline]</color> Mengunggah data ke Google Sheets... \nPayload: {json}");

        using (UnityWebRequest request = new UnityWebRequest(webAppUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"<color=red>[Telemetry Pipeline]</color> Upload Gagal: {request.error}");
            }
            else
            {
                Debug.Log($"<color=green>[Telemetry Pipeline]</color> Upload Sukses! Server merespons: {request.downloadHandler.text}");
            }
        }
    }
}