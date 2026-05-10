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
    [SerializeField] private Image photocardImageDisplay; 

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

    // 1. SAAT VAS DIPILIH
    public void OnVaseSelected(int puzzleIndex)
    {
        if (puzzleIndex < 0 || puzzleIndex >= allPuzzles.Length) return;
    
        currentActiveIndex = puzzleIndex; // Simpan index-nya
        currentActiveData = allPuzzles[puzzleIndex];

        vaseSelectionPanel.SetActive(false);

        // 4. SET KE PUZZLE (Pindah ke pojok & Shaking)
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
        // 1. Matikan panel puzzle
        reassemblePanel.SetActive(false);
        if (currentPuzzleSetInstance != null) Destroy(currentPuzzleSetInstance);

        // 2. Tampilkan Photocard
        photocardImageDisplay.sprite = completedPuzzleData.photocardSprite;
        photocardPanel.SetActive(true);

        // 3. Jalankan Dialog
        dialogueManager.StartDialogue(completedPuzzleData.afterPuzzleDialogue, () => {
        
            // --- SETELAH DIALOG HABIS ---
            photocardPanel.SetActive(false); // Tutup photocard
        
            // Lapor ke Act2_Manager (Sutradara)
            if (act2Manager != null) {
                act2Manager.ReportVaseCompleted(currentActiveIndex);
            }
        });
    }

    // 3. SAAT TOMBOL CLOSE DI PHOTOCARD DIKLIK
    public void OnPhotocardClosed()
    {
        photocardPanel.SetActive(false);

        if (currentActiveData.afterPuzzleDialogue != null)
        {
            dialogueManager.StartDialogue(currentActiveData.afterPuzzleDialogue, () => {
                // LAPOR KE SUTRADARA BESAR
                act2Manager.ReportVaseCompleted(currentActiveIndex);
            });
        }
    }

    public void ShowVaseSelection()
    {
        vaseSelectionPanel.SetActive(true);
        reassemblePanel.SetActive(false);
        photocardPanel.SetActive(false);
    }
}