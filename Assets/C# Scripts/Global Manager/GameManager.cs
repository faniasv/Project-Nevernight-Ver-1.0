using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Live State")]
    public string playerID = "Tester_01";
    public int currentAct = 1;
    public int avaLoad = 0; 

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // "Kalau ada OnAvaLoadAdded,jalankan fungsi AddAvaLoad"
        GameEvents.OnAvaLoadAdded += AddAvaLoad;
    }

    private void OnDisable()
    {
        // Cabut pendaftaran saat objek mati (mencegah memory leak)
        GameEvents.OnAvaLoadAdded -= AddAvaLoad;
    }

    private void Start()
    {
        // Panggil LoadDataFromJSON() saat game baru buka
        Debug.Log($"GameManager Siap! Player: {playerID}, Act: {currentAct}, Load: {avaLoad}");
    }

    // Fungsi untuk menambah beban mental Ava.
    private void AddAvaLoad(int amount)
    {
        avaLoad += amount;
        Debug.Log($"[GameManager] Ava Load bertambah {amount}. Total sekarang: {avaLoad}");

        // "Woi UI Manager dan Audio Manager, nilai load berubah nih, update tampilan kalian!"
        GameEvents.OnAvaLoadChanged?.Invoke(avaLoad);
    }

    // === SISTEM SAVE & LOAD (Akan diisi nanti) ===

    public void LoadDataFromJSON()
    {
        // TODO: Logika membaca file JSON untuk mengisi variabel di atas
        Debug.Log("[GameManager] Membaca data dari JSON...");
    }

    public void SaveDataToJSON()
    {
        // TODO: Logika menulis variabel saat ini ke dalam file JSON
        Debug.Log("[GameManager] Menyimpan data ke JSON...");
    }
}
