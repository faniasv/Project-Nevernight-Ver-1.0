using System;

[System.Serializable]
public class PlayerData
{
    // Identitas Dasar (Diisi di Main Menu)
    public string playerID;

    public string username;
    public string timestamp;

    // Metrik Perilaku Kognitif Per Babak (Diperbarui Selama Playtesting)
    public int currentAct;
    public float timeToFirstAction; // Mengukur Decision Paralysis
    public int failedAttempts;      // Mengukur Overthinking
    public int ovtSeverityScale;   // failedAttempts - 3 (Cognitive Overload)
    public float timeToObserveExit; // Mengukur Tunnel Vision

    public void ResetActData()
    {
        timeToFirstAction = 0f;
        failedAttempts = 0;
        ovtSeverityScale = 0;
        timeToObserveExit = 0f;
    }
}