using System;

public static class GameEvents
{
    // ==== Global State Events ====
    // Dipanggil oleh manajer untuk memerintahkan LevelManager pindah babak
    public static Action<int> OnActChanged;

    // ==== ACT 1: Task Puzzle Events (Analyst District) ====
    // Dipanggil setiap kali pemain menekan Save tapi menghasilkan Error/Overload
    public static Action OnTaskFailed;

    // Dipanggil saat attemptCounter mencapai batas maksimal (3x salah)
    public static Action OnPlayerStuck;

    // ==== ACT 2: Vase Puzzle Events (Memory Market) ====
    // Dipanggil saat satu vas kenangan berhasil dirakit utuh
    public static Action OnVaseCompleted;
}