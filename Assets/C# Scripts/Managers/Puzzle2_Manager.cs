using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class PuzzleData
{
    [Header("Data Inti Puzzle")]
    public string puzzleName; 
    public GameObject vasePuzzleSetPrefab; 
    public Sprite photocardSprite; // Balik ke Sprite biasa

    [Header("Data Audio")]
    public string ambienceName;
    public string sfxName;

    [Header("Data Narasi")]
    public DialogueData afterPuzzleDialogue; // Slot Dialog SETELAH puzzle
}

public class Puzzle2_Manager : MonoBehaviour
{
    [Header("Panel Utama")]
    [SerializeField] private GameObject vaseSelectionPanel;
    [SerializeField] private GameObject reassemblePanel;
    
    [Header("Panel Photocard")]
    [SerializeField] private GameObject photocardPanel; 
    [SerializeField] private Image blurImageDisplay;  // Tarik Base_Img_Blur ke sini
    [SerializeField] private Image vividImageDisplay; // Tarik Vivid_Img_Clear ke sini  
    [SerializeField] private PCVisualReveal visualRevealScript;

    [Header("Referensi Lain")]
    [SerializeField] private Transform puzzleSetParent; 
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private Act2_Manager act2Manager;
    [SerializeField] private Minion_Act2 minion;

    [Header("Database Puzzle")]
    public PuzzleData[] allPuzzles;
    private GameObject currentPuzzleSetInstance;
    private PuzzleData currentActiveData;
    private int currentActiveIndex;

    void Start()
    {
        if (visualRevealScript != null) {
            visualRevealScript.OnRevealComplete = HandleRevealAudio;
        }
    }
    // 1. SAAT VAS DIPILIH
    public void OnVaseSelected(int puzzleIndex)
    {
        if (puzzleIndex < 0 || puzzleIndex >= allPuzzles.Length) return;
    
        currentActiveIndex = puzzleIndex; // Simpan index-nya
        currentActiveData = allPuzzles[puzzleIndex];

        vaseSelectionPanel.SetActive(false);

        if(minion != null) minion.ChangeState(Minion_Act2.MinionState.Puzzle);

        StartCoroutine(StartPuzzle(currentActiveData));
    }

    private IEnumerator StartPuzzle(PuzzleData puzzleData)
    {
        reassemblePanel.SetActive(true);
        if (currentPuzzleSetInstance != null) Destroy(currentPuzzleSetInstance);

        currentPuzzleSetInstance = Instantiate(puzzleData.vasePuzzleSetPrefab, puzzleSetParent);

        VasePuzzleSet vaseScript = currentPuzzleSetInstance.GetComponent<VasePuzzleSet>();
        if (vaseScript != null)
        {
            vaseScript.SetupSet(this, puzzleData);
        }
        yield return null;
    }

    // 2. SAAT PUZZLE SELESAI
    
    public void HandlePuzzleCompletion(PuzzleData completedPuzzleData)
    {
        currentActiveData = completedPuzzleData;
    
        // 1. Matikan panel puzzle
        reassemblePanel.SetActive(false);
        if (currentPuzzleSetInstance != null) Destroy(currentPuzzleSetInstance);

        // 2. Tampilkan Photocard (Vivid Layer 0 / Buram)
        if(blurImageDisplay != null) blurImageDisplay.sprite = completedPuzzleData.photocardSprite;
        if(vividImageDisplay != null) vividImageDisplay.sprite = completedPuzzleData.photocardSprite;

        if(visualRevealScript != null) visualRevealScript.ResetReveal();
        photocardPanel.SetActive(true);
        // 3. MULAI AUDIO & STANDBY
        // Di sini JANGAN panggil dialogueManager. Biarkan player gosok foto dulu.
        StartCoroutine(PhotocardAudioSequence(completedPuzzleData));
    }

// SAAT TOMBOL CLOSE DI PHOTOCARD DIKLIK
    public void OnPhotocardClosed()
    {
        photocardPanel.SetActive(false);

        // Lapor ke Act2_Manager (Sutradara) bahwa vas ini sudah beres
        if (act2Manager != null) {
            act2Manager.ReportVaseCompleted(currentActiveIndex);
        }
    }

    public void ShowVaseSelection()
    {
        vaseSelectionPanel.SetActive(true);
        reassemblePanel.SetActive(false);
        photocardPanel.SetActive(false);
    }

    private IEnumerator PhotocardAudioSequence(PuzzleData data)
    {
        // 1. Jeda sebentar setelah puzzle beres
        yield return new WaitForSeconds(1.0f);
        
        // 2. Play Ava Sighs (Suara dasar lelah)
        AudioManager.instance.PlaySFX("AvaSighs");

        // 3. Play Ambience sesuai lokasi vas (Playground/Classroom/Space)
        // Kita gunakan fungsi PlayExtraAmbience yang kita buat kemarin
        AudioManager.instance.PlayExtraAmbience(data.ambienceName);
        
        // Sekarang script berhenti di sini, menunggu player narik handle visual...
    }

    private void HandleRevealAudio()
    {
        // 4. Play SFX spesifik (Crayon/Star) saat foto sudah terlihat jelas
        if (currentActiveData != null) {
            AudioManager.instance.PlaySFX(currentActiveData.sfxName);
        }

        // 5. Jalankan dialog setelah reveal beres
        StartCoroutine(WaitAndStartDialogue());
    }

    private IEnumerator WaitAndStartDialogue()
    {
        yield return new WaitForSeconds(1.5f); // Jeda biar SFX crayon/star terdengar dulu
        
        dialogueManager.StartDialogue(currentActiveData.afterPuzzleDialogue, () => {
            if (visualRevealScript != null) visualRevealScript.ShowCloseButton(); // Munculkan tombol close setelah dialog
        });
    }
}