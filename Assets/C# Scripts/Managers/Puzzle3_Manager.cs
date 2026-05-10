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

    [Header("Pengaturan Gameplay")]
    public float laneDistance = 120f;
    public float scrollSpeed = 250f;
    public float spawnInterval = 1.2f;
    public int targetScorePerLevel = 5;

    private bool isFrozen = false;
    private int currentPackageIndex = 0;
    private int currentScore = 0;
    private QuestionPackage currentPackage;

    void Start()
    {
        // 1. Inisialisasi Data Awal
        if (allPackages != null && allPackages.Count > 0)
        {
            LoadPackage(0);
        }
    
        // 2. Mulai Munculkan Gelembung (Pake Coroutine biar lebih stabil)
        StartCoroutine(SpawnLoop());
        
        SetTrafficLight(true);
    }

    public void LoadPackage(int index)
    {
        if (index >= allPackages.Count)
        {
            WinGame();
            return;
        }

        currentPackageIndex = index;
        currentPackage = allPackages[index];
        boardText.text = currentPackage.questionText;
        currentScore = 0;
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            // Hanya spawn kalau sistem tidak sedang beku
            if (!isFrozen)
            {
                SpawnBubble();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnBubble()
    {
        // Validasi: Berhenti di sini kalau ada referensi yang kosong
        if (bubblePrefab == null || spawnPoint == null || currentPackage == null) return;

        // 1. Munculkan gelembung
        GameObject go = Instantiate(bubblePrefab, spawnPoint.parent);
        go.transform.localScale = Vector3.one; // Reset skala biar gak gepeng

        // 2. Atur Posisi & Jalur (Lane)
        int randomLane = Random.Range(-1, 2); 
        float yOffset = randomLane * laneDistance;
        Vector3 startPos = spawnPoint.localPosition;
        startPos.y += yOffset;
        go.transform.localPosition = startPos;

        // 3. Tentukan Isi (Jujur vs Bohong)
        bool isGood = Random.value > 0.5f;
        string chosenText = isGood ? 
            currentPackage.goodAnswers[Random.Range(0, currentPackage.goodAnswers.Count)] : 
            currentPackage.badAnswers[Random.Range(0, currentPackage.badAnswers.Count)];

        // 4. Pilih Gambar Balon Acak
        Sprite chosenSprite = bubbleSprites[Random.Range(0, bubbleSprites.Count)];

        // 5. Kirim data ke script balon (PENTING: Gak ada lagi NotImplementedException di sini)
        ThoughtBubble script = go.GetComponent<ThoughtBubble>();
        if (script != null)
        {
            script.Setup(chosenSprite, chosenText, isGood, scrollSpeed, endPoint.localPosition.x);
        }
    }

    public void OnBubbleClicked(bool isCorrect)
    {
        if (isFrozen) return;

        if (isCorrect)
        {
            currentScore++;
            if (currentScore >= targetScorePerLevel)
            {
                NextLevel();
            }
        }
        else
        {
            StartCoroutine(FreezeSystem());
        }
    }

    IEnumerator FreezeSystem()
    {
        isFrozen = true;
        SetTrafficLight(false); // Lampu Merah
        yield return new WaitForSeconds(2f); // Durasi Freeze
        SetTrafficLight(true); // Lampu Hijau
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

    void WinGame()
    {
        isFrozen = true;
        boardText.text = "Selesai. Ava mulai tenang.";
        Debug.Log("Act 3 Clear!");
    }
}