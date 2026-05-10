using System.Collections;
using UnityEngine;

public class Act1_Manager : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private Puzzle1_Manager puzzle1Manager;
    [SerializeField] private DialogueData introDialogue;
    [SerializeField] private Minion_Act1 minion;

    void Start()
    {
        // Pastikan Minion jalan-jalan di awal
        if(minion != null) minion.SetToIntroMode();

        // SEMBUNYIKAN PUZZLE DI AWAL BANGET
        if(puzzle1Manager != null) puzzle1Manager.SetPuzzleState(false, false);

        StartCoroutine(StartSequenceRoutine());
    }

    private IEnumerator StartSequenceRoutine()
    {
        yield return new WaitForSeconds(1f); // Kasih waktu Unity buat loading

        // MULAI DIALOG
        // Fungsi OnIntroFinished bakal dipanggil OTOMATIS pas dialog beres
        if (dialogueManager != null && introDialogue != null)
        {
            dialogueManager.StartDialogue(introDialogue, OnIntroFinished);
        }
    }

    private void OnIntroFinished()
    {
        Debug.Log("<color=green>[Act1_Manager]</color> Menyalakan Puzzle Sekarang!");

        if (puzzle1Manager != null)
        {
            // Munculkan Puzzle (Visible = true, Interactable = true)
            puzzle1Manager.SetPuzzleState(true, true);
            
            // Minion jadi diam
            if(minion != null) minion.SetToPuzzleMode();
        }
    }
}