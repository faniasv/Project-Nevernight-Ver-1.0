using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [Header("Referensi Utama")]
    public Puzzle3_Manager puzzleManager; // Seret objek _Managers ke sini

    [Header("Referensi UI Canvas")]
    public GameObject bubblePrefab;      // Prefab gelembung UI
    public RectTransform spawnPoint;     // Titik spawn sebelah kanan luar layar
    public RectTransform endPoint;       // Titik batas kiri luar layar

    [Header("Pengaturan Jalur & Kecepatan")]
    public float laneDistance = 120f;    // Jarak antar jalur (atas/bawah)
    public float scrollSpeed = 250f;     // Kecepatan jalan balon ke kiri
    public float spawnInterval = 1.2f;   // Jeda waktu spawn (detik)
    
    [Header("Aset Visual Balon")]
    public List<Sprite> bubbleSprites;   // Variasi gambar balon Naïve Art

    private List<bool> spawnPool = new List<bool>();
    private Coroutine spawnCoroutine;

    // Fungsi untuk mulai menyalakan mesin spawn (Dipanggil dari Puzzle3_Manager)
    public void StartSpawning()
    {
        GenerateSpawnPool();
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    // Fungsi untuk menghentikan spawn balon (Dipanggil saat game over/clear)
    public void StopSpawning()
    {
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            // Hanya bikin balon kalau game TIDAK SEDANG FREEZE
            if (puzzleManager != null && !puzzleManager.IsSystemFrozen)
            {
                SpawnBubble();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Mengatur antrean rasio tepat 1 Jujur (True) : 5 Defensif (False)
    public void GenerateSpawnPool()
    {
        spawnPool.Clear();
        spawnPool.Add(true); // 1 Kata jujur
        for (int i = 0; i < 5; i++)
        {
            spawnPool.Add(false); // 5 Kata defensif
        }

        // Acak urutan list (Fisher-Yates Shuffle)
        for (int i = 0; i < spawnPool.Count; i++)
        {
            bool temp = spawnPool[i];
            int randomIndex = Random.Range(i, spawnPool.Count);
            spawnPool[i] = spawnPool[randomIndex];
            spawnPool[randomIndex] = temp;
        }
    }

    void SpawnBubble()
    {
        if (bubblePrefab == null || spawnPoint == null || puzzleManager == null) return;
        if (puzzleManager.GetCurrentPackage() == null) return;

        // Jika isi bensin antrean habis, isi ulang lalu acak lagi
        if (spawnPool.Count == 0)
        {
            GenerateSpawnPool();
        }

        // 1. Kloning Prefab Gelembung di dalam Canvas UI
        GameObject go = Instantiate(bubblePrefab, spawnPoint.parent);
        go.transform.localScale = Vector3.one; // Reset skala biar tidak gepeng

        // 2. Atur Posisi & Jalur (Lane) acak: Atas (120), Tengah (0), Bawah (-120)
        int randomLane = Random.Range(-1, 2); 
        float yOffset = randomLane * laneDistance;
        Vector3 startPos = spawnPoint.localPosition;
        startPos.y += yOffset;
        go.transform.localPosition = startPos;

        // 3. Tentukan isi gelembung berdasarkan antrean pool
        bool isGood = spawnPool[0];
        spawnPool.RemoveAt(0); // Hapus antrean terdepan yang barusan diambil

        QuestionPackage currentPackage = puzzleManager.GetCurrentPackage();
        string chosenText = "";

        if (isGood)
        {
            chosenText = currentPackage.goodAnswers[Random.Range(0, currentPackage.goodAnswers.Count)];
        }
        else
        {
            chosenText = currentPackage.badAnswers[Random.Range(0, currentPackage.badAnswers.Count)];
        }

        // 4. Pilih Gambar Balon Acak
        Sprite chosenSprite = (bubbleSprites.Count > 0) ? bubbleSprites[Random.Range(0, bubbleSprites.Count)] : null;

        // 5. Kirim data ke script ThoughtBubble
        ThoughtBubble script = go.GetComponent<ThoughtBubble>();
        if (script != null)
        {
            script.Setup(chosenSprite, chosenText, isGood, scrollSpeed, endPoint.localPosition.x);
        }
    }
}