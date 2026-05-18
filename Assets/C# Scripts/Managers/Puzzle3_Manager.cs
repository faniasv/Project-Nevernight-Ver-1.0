using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class QuestionPackage
{
    [TextArea(2, 3)]
    public string questionText;
    public List<string> goodAnswers;
    public List<string> badAnswers;
}

public class Puzzle3_Manager : MonoBehaviour
{
    [Header("Referensi Utama Act 3")]
    private Act3_Manager act3MainManager; // Otomatis dicari saat Start

    [Header("Konfigurasi Paket Soal")]
    public List<QuestionPackage> allPackages;
    
    [Header("Aset Visual")]
    public List<Sprite> bubbleSprites;
    
    [Header("Referensi UI")]
    public TextMeshProUGUI boardText;
    public Image trafficLight;
    public GameObject bubblePrefab;
    public RectTransform spawnPoint;
    public RectTransform endPoint;

    [Header("Referensi Spawner Baru")]
    public BubbleSpawner bubbleSpawner;

    [Header("Pengaturan Gameplay")]
    public float laneDistance = 120f;
    public float scrollSpeed = 250f;
    public float spawnInterval = 1.2f;

    [Header("Referensi Ending Chat di Tunnel (Internal Exit Condition)")]
    public GameObject chatEndingPrefab; 
    public List<RectTransform> chatPositions; // 5 Titik RectTransform di dalam tunnel UI

    private bool isFrozen = true; // Set true di awal agar tidak spawn sebelum opening selesai
    private bool isGameEnded = false;
    private int currentPackageIndex = 0;
    private QuestionPackage currentPackage;
    private List<bool> spawnPool = new List<bool>();

    public bool IsSystemFrozen => isFrozen;

    private List<string> endingDialogues = new List<string>()
    {
        "Ternyata... selama ini aku cuma takut.",
        "Takut kalau berhenti sebentar saja, aku akan tertinggal jauh.",
        "Tapi tubuhku sudah tidak bisa bohong lagi.",
        "Maaf ya, sudah memaksamu terlalu keras.",
        "Let's rest first."
    };

    void Start()
    {
        // Cari script sutradara besarnya di scene
        act3MainManager = FindObjectOfType<Act3_Manager>();
        SetTrafficLight(true);
    }

    // Fungsi ini dipanggil dari Act3_Manager setelah opening selesai
    public void StartPuzzleGameplay()
    {
        isFrozen = false;
        if (allPackages != null && allPackages.Count > 0)
        {
            LoadPackage(0);
        }
    
        // Perintahkan script spawner terpisah untuk mulai bekerja!
        if (bubbleSpawner != null) bubbleSpawner.StartSpawning();
    }

    public void LoadPackage(int index)
    {
        if (index >= allPackages.Count)
        {
            // Jika menang, matikan mesin spawner dulu
            if (bubbleSpawner != null) bubbleSpawner.StopSpawning();
        
            StartCoroutine(PlayTunnelChatSequence());
            return;
        }

        currentPackageIndex = index;
        currentPackage = allPackages[index];
        boardText.text = currentPackage.questionText;

        if (bubbleSpawner != null) bubbleSpawner.GenerateSpawnPool();
    }

    
    void GenerateSpawnPool()
    {
        spawnPool.Clear();
        spawnPool.Add(true); // 1 Jujur
        for (int i = 0; i < 5; i++) spawnPool.Add(false); // 5 Defensif

        for (int i = 0; i < spawnPool.Count; i++)
        {
            bool temp = spawnPool[i];
            int randomIndex = Random.Range(i, spawnPool.Count);
            spawnPool[i] = spawnPool[randomIndex];
            spawnPool[randomIndex] = temp;
        }
    }

    public void OnBubbleClicked(bool isCorrect)
    {
        if (isFrozen || isGameEnded) return;

        if (isCorrect) NextLevel();
        else StartCoroutine(FreezeSystem());
    }

    IEnumerator FreezeSystem()
    {
        isFrozen = true;
        SetTrafficLight(false); // Merah
        yield return new WaitForSeconds(3f); 
        SetTrafficLight(true); // Hijau
        isFrozen = false;
    }

    void SetTrafficLight(bool isGreen)
    {
        if (trafficLight != null)
            trafficLight.color = isGreen ? Color.green : Color.red;
    }

    void NextLevel()
    {
        currentPackageIndex++;
        LoadPackage(currentPackageIndex);
    }

    // URUTAN PERCAKAPAN TUNNEL (EXIT CONDITION PUZZLE)
    IEnumerator PlayTunnelChatSequence()
    {
        isGameEnded = true;
        isFrozen = true;
        boardText.text = ""; // Bersihkan papan kuning

        // Bersihkan balon gameplay yang tersisa
        ThoughtBubble[] activeBubbles = FindObjectsOfType<ThoughtBubble>();
        foreach (ThoughtBubble b in activeBubbles) Destroy(b.gameObject);

        yield return new WaitForSeconds(1f);

        // Munculkan chat satu per satu di dalam terowongan
        for (int i = 0; i < endingDialogues.Count; i++)
        {
            if (i >= chatPositions.Count || chatEndingPrefab == null) break;

            GameObject chatGo = Instantiate(chatEndingPrefab, spawnPoint.parent);
            chatGo.transform.localScale = Vector3.one;
            chatGo.transform.localPosition = chatPositions[i].localPosition;

            TextMeshProUGUI chatText = chatGo.GetComponentInChildren<TextMeshProUGUI>();
            if (chatText != null) chatText.text = endingDialogues[i];

            yield return new WaitForSeconds(2.5f); 
        }

        yield return new WaitForSeconds(1.5f); // Beri waktu baca kalimat terakhir "Let's rest first"

        // SELESAI! Lapor ke Act3_Manager kalau puzzle dan chat tunnel sudah tuntas
        if (act3MainManager != null)
        {
            act3MainManager.OnPuzzle3Complete();
        }
    }
    
    public QuestionPackage GetCurrentPackage()
    {
        return currentPackage;
    }
}