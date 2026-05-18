using UnityEngine;
using System.Collections;

public class Act3_Manager : MonoBehaviour
{
    [Header("Referensi Manager Script")]
    public Puzzle3_Manager puzzleManager; // Seret object Puzzle3_Manager ke sini

    [Header("Referensi UI Game Object")]
    public GameObject openingDialogueUI; // Canvas/Panel untuk opening dialogue
    public GameObject puzzleGameplayUI;  // Canvas/Panel utama tempat puzzle & tunnel berada
    public GameObject exitDialogueUI;    // Canvas/Panel untuk exit dialogue setelah puzzle

    void Start()
    {
        StartAct3Sequence();
    }

    void StartAct3Sequence()
    {
        // 1. Tampilkan dialog pembuka, sembunyikan gameplay puzzle dulu
        if (openingDialogueUI != null) openingDialogueUI.SetActive(true);
        if (puzzleGameplayUI != null) puzzleGameplayUI.SetActive(false);
        if (exitDialogueUI != null) exitDialogueUI.SetActive(false);

        Debug.Log("Act 3 Dimulai: Memutar Opening Dialogue...");
        // NOTE: Di sini kamu bisa panggil Dialogue System kamu untuk opening.
        // Sebagai contoh, setelah dialog pembuka diklik habis, panggil TriggerPuzzle3().
    }

    // Fungsi ini dipanggil saat dialog pembuka selesai
    public void TriggerPuzzle3()
    {
        if (openingDialogueUI != null) openingDialogueUI.SetActive(false);
        if (puzzleGameplayUI != null) puzzleGameplayUI.SetActive(true);

        Debug.Log("Opening Selesai. Puzzle 3 Dimulai!");
        
        // Perintahkan Puzzle3_Manager untuk mulai menyalakan gameplay & spawner
        if (puzzleManager != null)
        {
            puzzleManager.StartPuzzleGameplay();
        }
    }

    // KUNCI UTAMA: Fungsi callback yang akan dipanggil oleh Puzzle3_Manager kalau dia SUDAH SELESAI
    public void OnPuzzle3Complete()
    {
        Debug.Log("Menerima laporan dari Puzzle3_Manager: Puzzle & Chat Tunnel Selesai!");
        
        // Matikan UI puzzle, aktifkan UI dialog penutup menuju Act 4
        if (puzzleGameplayUI != null) puzzleGameplayUI.SetActive(false);
        if (exitDialogueUI != null) exitDialogueUI.SetActive(true);

        StartCoroutine(TransitionToAct4());
    }

    IEnumerator TransitionToAct4()
    {
        Debug.Log("Memutar Exit Dialogue Act 3...");
        // Tunggu pemain membaca dialog penutup (misal 4 detik atau pakai trigger klik dialog)
        yield return new WaitForSeconds(4f); 

        Debug.Log("Transisi kembali ke Act 4 (Papan tugas baru terpasang).");
        // Di sini kamu tinggal panggil SceneManager.LoadScene("Act4_Scene") atau sistem pindah tokomu.
    }
}