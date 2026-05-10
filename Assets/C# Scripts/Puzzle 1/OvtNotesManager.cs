using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OvtNotesManager : MonoBehaviour
{
    [Header("Pool Pikiran Berisik")]
    public List<string> thoughtPool = new List<string>();

    [Header("Spawn Area")]
    public RectTransform spawnArea; 

    [Header("References")]
    public GameObject stickyNotePrefab;
    public GameObject photoButton;
    public GameObject photocardPanel;

    [Header("System References")]
    public DialogueManager dialogueManager; 
    public AudioSource dialogueAudio;      
    public Puzzle1_Manager puzzleManager;
    public LevelLoader levelLoader; 

    private List<GameObject> activeNotes = new List<GameObject>();
    private int currentAttempt;
    private CanvasGroup photoBtnCanvasGroup;

    [Header("UI Blocker")]
    [Tooltip("Tarik 'Puzzle1_Panel' ke sini agar tombol Kerjakan mati saat OVT")]
    public CanvasGroup puzzleCanvasGroup;

    void Start()
    {
        if (photoButton != null) 
        {
            photoButton.SetActive(false);
            photoBtnCanvasGroup = photoButton.GetComponent<CanvasGroup>();
            if (photoBtnCanvasGroup == null) photoBtnCanvasGroup = photoButton.AddComponent<CanvasGroup>();
        }
        if (photocardPanel != null) photocardPanel.SetActive(false);
    }

    public void TriggerOVT(int attemptCount)
    {
        currentAttempt = attemptCount;

        if (puzzleCanvasGroup != null)
        {
            puzzleManager.SetPuzzleInteractive(false);
        }

        // 1. MATIKAN SUARA DIALOG SECARA HALUS
        if (dialogueAudio != null) dialogueAudio.Stop();
        
        // 2. KOSONGKAN TEKS TANPA MEMATIKAN PANEL
        if (dialogueManager != null) 
        {
            // Kita hentikan coroutine pengetikan teks agar tidak lanjut muncul
            dialogueManager.StopAllCoroutines(); 
            
            // Cari komponen teks di dalam DialogueManager dan kosongkan
            // Ini memastikan panel tetap aktif tapi terlihat "bersih"
            var txt = dialogueManager.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = ""; 
        }
        
        activeNotes.Clear();
        List<string> shuffledThoughts = new List<string>(thoughtPool);
        for (int i = 0; i < shuffledThoughts.Count; i++) {
            string temp = shuffledThoughts[i];
            int randomIndex = Random.Range(i, shuffledThoughts.Count);
            shuffledThoughts[i] = shuffledThoughts[randomIndex];
            shuffledThoughts[randomIndex] = temp;
        }

        int notesToSpawn = 7 + (attemptCount - 3); 
        notesToSpawn = Mathf.Min(notesToSpawn, shuffledThoughts.Count); 

        for (int i = 0; i < notesToSpawn; i++)
        {
            SpawnNote(shuffledThoughts[i]);
        }
    }

    private void SpawnNote(string thoughtText)
    {
        GameObject newNote = Instantiate(stickyNotePrefab, spawnArea);
        RectTransform noteRect = newNote.GetComponent<RectTransform>();
        float randomScale = Random.Range(0.3f, 0.5f);
        newNote.transform.localScale = new Vector3(randomScale, randomScale, 1f);

        // Rumus Safe Zone agar tidak keluar layar
        float noteHalfWidth = noteRect.rect.width * randomScale / 2f;
        float noteHalfHeight = noteRect.rect.height * randomScale / 2f;
        float padding = 10f;
        float xLimit = (spawnArea.rect.width / 2f) - noteHalfWidth - padding;
        float yLimit = (spawnArea.rect.height / 2f) - noteHalfHeight - padding;

        newNote.transform.localPosition = new Vector3(Random.Range(-xLimit, xLimit), Random.Range(-yLimit, yLimit), 0);
        newNote.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-20f, 20f));
        newNote.GetComponentInChildren<TextMeshProUGUI>().text = thoughtText;
        activeNotes.Add(newNote);
    }

    public void CloseThisNote(GameObject note)
    {
        if (activeNotes.Contains(note))
        {
            activeNotes.Remove(note);
            Destroy(note);
        }
        if (activeNotes.Count <= 0) RevealPhoto();
    }

    private void RevealPhoto()
    {
        // 1. HIDUPKAN KEMBALI PANEL DIALOGNYA
        if (dialogueManager != null) 
        {
            dialogueManager.gameObject.SetActive(true); 
        }

        if (photoButton != null) 
        {
            photoButton.SetActive(true);
            photoButton.transform.SetAsLastSibling(); 
            float targetAlpha = Mathf.Clamp((currentAttempt - 2) * 0.2f, 0.2f, 1f);
            photoBtnCanvasGroup.alpha = targetAlpha;
            if (currentAttempt >= 7) StartCoroutine(GlowEffect());
        }

        // 2. JALANKAN DIALOG OVT
        if (puzzleManager != null) puzzleManager.PlayOvtDialogue(); 
    }

    IEnumerator GlowEffect()
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.time * 2f) + 1f) / 2f; 
            photoBtnCanvasGroup.alpha = Mathf.Lerp(0.6f, 1f, t);
            yield return null;
        }
    }

    // --- FUNGSI KLIK FOTO KE ACT 2 ---
    public void OnPhotoClicked()
    {
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {

        // 1. Munculkan Photocard Memori
        if (photocardPanel != null) photocardPanel.SetActive(true);
        
        // 2. Beri waktu player meresapi foto (3.5 detik)
        yield return new WaitForSeconds(2.5f);

        // 3. Panggil LevelLoader lo untuk pindah scene
        if (levelLoader != null)
        {
            levelLoader.LoadNextScene("SC_Act2"); 
        }
        else if (LevelLoader.instance != null) // Backup pakai Singleton kalau slot Inspector lupa diisi
        {
            LevelLoader.instance.LoadNextScene("SC_Act2");
        }
        else
        {
            Debug.LogError("OvtNotesManager: LevelLoader tidak ditemukan!");
        }
    }
}