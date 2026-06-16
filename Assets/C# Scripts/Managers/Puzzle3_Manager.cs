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

// 1. DATA STRUKTUR BARU UNTUK CHAT ENDING
[System.Serializable]
public class ChatDialogue
{
    [TextArea(2, 3)]
    public string message;
    [Tooltip("Centang jika ini ucapan Ava (Kanan), kosongkan jika Minion (Kiri)")]
    public bool isAva; 
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
    public GameObject endingTunnel;   

    [Header("Referensi Spawner")]
    public BubbleSpawner bubbleSpawner;

    [Header("Referensi Ending Chat di Tunnel")]
    public GameObject chatEndingPrefab; 
    public List<RectTransform> chatPositions; // 5 Titik RectTransform di dalam tunnel UI

    [Header("Pengaturan Warna & Skenario Chat")]
    public Color minionBubbleColor = Color.white;
    public Color avaBubbleColor = new Color(0.6f, 0.85f, 1f); // Default Biru Muda untuk Ava
    public List<ChatDialogue> endingDialogues; // Isi dialog dari Inspector

    private bool isFrozen = false; 
    private bool isGameEnded = false;
    private int currentPackageIndex = 0;
    private QuestionPackage currentPackage;

    public bool IsSystemFrozen => isFrozen;

    void Start()
    {
        SetTrafficLight(true);
        StartProtoGameplay();
    }

    void StartProtoGameplay()
    {
        isFrozen = false;
        if (allPackages != null && allPackages.Count > 0)
        {
            Debug.Log("Semua paket aman? Meluncur ges");
            LoadPackage(0);
        }
    
        if (bubbleSpawner != null) 
            Debug.Log("Bubble Start Spawning dipanggil");
            bubbleSpawner.StartSpawning();
            Debug.Log("Bubble jalan yeay");
    }

    public void LoadPackage(int index)
    {
        if (index >= allPackages.Count)
        {
            if (bubbleSpawner != null) bubbleSpawner.StopSpawning();
            StartCoroutine(PlayTunnelChatSequence());
            return;
        }

        currentPackageIndex = index;
        currentPackage = allPackages[index];
        boardText.text = currentPackage.questionText;

        if (bubbleSpawner != null) bubbleSpawner.GenerateSpawnPool();
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
        SetTrafficLight(false); 
        yield return new WaitForSeconds(3f); 
        SetTrafficLight(true); 
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

    IEnumerator PlayTunnelChatSequence()
    {
        if (endingTunnel != null) endingTunnel.SetActive(true);
        isGameEnded = true;
        isFrozen = true;
        boardText.text = ""; 

        ThoughtBubble[] activeBubbles = FindObjectsOfType<ThoughtBubble>();
        foreach (ThoughtBubble b in activeBubbles) Destroy(b.gameObject);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < endingDialogues.Count; i++)
        {
            // Hapus syarat bubbleSpawner == null di sini biar ga error kalau spawner ga kepake lagi
            if (i >= chatPositions.Count || chatEndingPrefab == null) break;

            // 🔥 PERUBAHANNYA DI SINI COY:
            // Kita jadikan endingTunnel sebagai parent (induk) tempat chat ini muncul!
            Transform chatParent = endingTunnel != null ? endingTunnel.transform : transform;
            GameObject chatGo = Instantiate(chatEndingPrefab, chatParent);
            
            chatGo.transform.localScale = Vector3.one;
            chatGo.transform.localPosition = chatPositions[i].localPosition;

            // AMBIL KOMPONEN GAMBAR & TEKS
            Image bubbleImage = chatGo.GetComponent<Image>();
            TextMeshProUGUI chatText = chatGo.GetComponentInChildren<TextMeshProUGUI>();

            // CEK SIAPA YANG BICARA
            if (endingDialogues[i].isAva) 
            {
                // JIKA AVA: Balik arah (Flip) dan ubah warna ke warna Ava
                Vector3 flippedScale = chatGo.transform.localScale;
                flippedScale.x = -1f; 
                chatGo.transform.localScale = flippedScale;

                if (bubbleImage != null) bubbleImage.color = avaBubbleColor;

                // Kembalikan Scale X pada teksnya ke -1 agar tulisannya tidak ikut terbalik/cermin
                if (chatText != null)
                {
                    Vector3 textScale = chatText.transform.localScale;
                    textScale.x = -1f;
                    chatText.transform.localScale = textScale;
                }
            }
            else
            {
                // JIKA MINION: Arah normal, ubah warna ke warna Minion
                if (bubbleImage != null) bubbleImage.color = minionBubbleColor;
            }

            // SET TEKS DIALOGNYA
            if (chatText != null) 
            {
                chatText.text = endingDialogues[i].message;
            }

            yield return new WaitForSeconds(2.5f); 
        }

        yield return new WaitForSeconds(1.5f); 
        boardText.text = "PROTOTYPE SUCKSEED!";
        Debug.Log("Selesai! Seluruh alur inti prototype Puzzle 3 berhasil dieksekusi.");
    }
    
    public QuestionPackage GetCurrentPackage()
    {
        return currentPackage;
    }
}