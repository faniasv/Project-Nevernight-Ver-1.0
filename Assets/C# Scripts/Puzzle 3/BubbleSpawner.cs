using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [Header("Referensi Utama")]
    public Puzzle3_Manager puzzleManager; 

    [Header("Referensi UI Canvas")]
    public GameObject bubblePrefab;      
    public RectTransform spawnPoint;     
    public RectTransform endPoint;       

    [Header("Pengaturan Jalur & Kecepatan")]
    public float laneDistance = 120f;    
    public float scrollSpeed = 250f;     
    public float spawnInterval = 1.2f;   
    
    [Header("Aset Visual Balon")]
    public List<Sprite> bubbleSprites;   

    private List<bool> spawnPool = new List<bool>();
    private Coroutine spawnCoroutine;

    public void StartSpawning()
    {
        GenerateSpawnPool();
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (puzzleManager != null && !puzzleManager.IsSystemFrozen)
            {
                SpawnBubble();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void GenerateSpawnPool()
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

    void SpawnBubble()
    {
        // CCTV: Cek apakah ada yang menahan proses spawn!
        if (bubblePrefab == null) { Debug.Log("❌ GAGAL SPAWN: Prefab kosong!"); return; }
        if (spawnPoint == null) { Debug.Log("❌ GAGAL SPAWN: SpawnPoint kosong!"); return; }
        if (puzzleManager == null) { Debug.Log("❌ GAGAL SPAWN: Manager kosong!"); return; }
        if (puzzleManager.GetCurrentPackage() == null) { Debug.Log("❌ GAGAL SPAWN: Paket Soal kosong!"); return; }

        if (spawnPool.Count == 0) GenerateSpawnPool();

        GameObject go = Instantiate(bubblePrefab, spawnPoint.parent);
        go.transform.localScale = Vector3.one; 

        int randomLane = Random.Range(-1, 2); 
        float yOffset = randomLane * laneDistance;
        Vector3 startPos = spawnPoint.localPosition;
        startPos.y += yOffset;
        go.transform.localPosition = startPos;

        bool isGood = spawnPool[0];
        spawnPool.RemoveAt(0); 

        QuestionPackage currentPackage = puzzleManager.GetCurrentPackage();
        string chosenText = "...";

        // Mencegah error kalau list kata-katamu di Inspector belum diisi
        if (isGood && currentPackage.goodAnswers.Count > 0)
        {
            chosenText = currentPackage.goodAnswers[Random.Range(0, currentPackage.goodAnswers.Count)];
        }
        else if (!isGood && currentPackage.badAnswers.Count > 0)
        {
            chosenText = currentPackage.badAnswers[Random.Range(0, currentPackage.badAnswers.Count)];
        }

        Sprite chosenSprite = (bubbleSprites.Count > 0) ? bubbleSprites[Random.Range(0, bubbleSprites.Count)] : null;

        ThoughtBubble script = go.GetComponent<ThoughtBubble>();
        if (script != null)
        {
            script.Setup(chosenSprite, chosenText, isGood, scrollSpeed, endPoint.localPosition.x);
        }
        else
        {
            Debug.Log("❌ GAGAL SPAWN: Prefab gelembung tidak punya skrip ThoughtBubble!");
        }
    }
}