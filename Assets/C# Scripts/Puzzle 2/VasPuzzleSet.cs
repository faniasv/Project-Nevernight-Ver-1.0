using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VasePuzzleSet : MonoBehaviour
{
    void Start()
    {
        // For testing in editor when puzzle is already in scene
        if (puzzleManager == null)
        {
            Debug.LogWarning("Testing mode - auto-setup");
            // puzzleManager = FindObjectOfType<Puzzle2_Manager>();
            // allTargets = new List<FragmentTarget>(GetComponentsInChildren<FragmentTarget>());
            // totalTargets = allTargets.Count;
            // completedTargets = 0;
        
            // Debug.Log($"VasePuzzleSet: Test setup. Found {totalTargets} targets.");
        
            // foreach (FragmentTarget target in allTargets)
            // {
            //      target.SetupTarget(this);
            // }
        }
    }
    // Referensi yang diisi oleh manager
    private Puzzle2_Manager puzzleManager;
    private PuzzleData myPuzzleData; // Kita simpan semua data puzzle

    private List<FragmentTarget> allTargets;
    private int totalTargets;
    private int completedTargets = 0;

    // Fungsi ini dipanggil oleh Puzzle2_Manager saat prefab ini di-Instantiate
    public void SetupSet(Puzzle2_Manager manager, PuzzleData data)
    {
        puzzleManager = manager;
        myPuzzleData = data; // Simpan data puzzle-nya

        allTargets = new List<FragmentTarget>(GetComponentsInChildren<FragmentTarget>());
        totalTargets = allTargets.Count;
        completedTargets = 0;

        Debug.Log($"VasePuzzleSet: Siap. Membutuhkan {totalTargets} kepingan.");

        foreach (FragmentTarget target in allTargets)
        {
            target.SetupTarget(this);
        }
    }

    // Fungsi ini dipanggil oleh FragmentTarget saat 1 kepingan benar
    public void NotifyTargetCompleted()
    {
        completedTargets++;
        Debug.Log($"VasePuzzleSet: Progres {completedTargets} / {totalTargets}");

        if (completedTargets >= totalTargets)
        {
            PuzzleSelesai();
        }
    }

    // Fungsi ini akan memicu event "Puzzle Selesai" ke manager utama
    private void PuzzleSelesai()
    {
        Debug.Log("VasePuzzleSet: SEMUA KEPINGAN BENAR! Mengirim sinyal ke Puzzle2_Manager.");
        // Kirim kembali SEMUA data puzzle
        puzzleManager.HandlePuzzleCompletion(myPuzzleData);
    }
}