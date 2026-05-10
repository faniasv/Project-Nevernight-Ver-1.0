using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class TaskData
{
    public string taskID;
    [Range(0, 100)] public float taskLoad;
    public DialogueData forgottenDialogue;
}

public class Puzzle1_Manager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject puzzlePanel;
    public CanvasGroup puzzleCanvasGroup;
    public GameObject errorPanel;

    public Slider mentalLoadSlider;
    public float maxLoad = 100f;

    [Header("System References")]
    public DialogueManager dialogueManager;
    [SerializeField] private Minion_Act1 minion;
    public OvtNotesManager ovtManager;

    [Header("OVT Reminders")]
    public List<DialogueData> ovtReminderDialogues; 

    [Tooltip("Isi dengan kalimat pendek: 'Lihat fotonya', 'Fokus ke meja', dll")]
    public List<DialogueData> guidingBarks; 

    [Header("Puzzle Data")]
    public List<TaskData> allTasksInLevel;

    private int attemptCounter = 0;
    private Dictionary<int, string> occupiedSlots = new Dictionary<int, string>();

    private void OnEnable() => DialogueManager.OnDialogueEnded += ReboundPuzzle;
    private void OnDisable() => DialogueManager.OnDialogueEnded -= ReboundPuzzle;

    void Start()
    {
        if (puzzlePanel != null) puzzlePanel.SetActive(true);

        if (mentalLoadSlider != null)
        {
            mentalLoadSlider.maxValue = maxLoad;
            mentalLoadSlider.value = 0;
        }

        SetPuzzleState(false, false);
    }

    public void OnTaskDroppedOnSlot(int slotIndex, string taskID)
    {
        if (occupiedSlots.ContainsKey(slotIndex)) occupiedSlots[slotIndex] = taskID;
        else occupiedSlots.Add(slotIndex, taskID);
        UpdateSliderVisual();
    }

    public void OnTaskRemovedFromSlot(int slotIndex)
    {
        if (occupiedSlots.ContainsKey(slotIndex)) occupiedSlots.Remove(slotIndex);
        UpdateSliderVisual();
    }

    private void UpdateSliderVisual()
    {
        if (mentalLoadSlider == null) return;
        float currentTotalLoad = 0f;
        foreach (var item in occupiedSlots)
        {
            string id = item.Value;
            TaskData data = allTasksInLevel.Find(x => x.taskID == id);
            if (data != null) currentTotalLoad += data.taskLoad;
        }
        mentalLoadSlider.value = currentTotalLoad;
    }

    public void OnSaveButtonPressed()
    {
       if (puzzleCanvasGroup != null && !puzzleCanvasGroup.interactable) return;
        StartCoroutine(HandleSubmissionSequence());
    }

    private IEnumerator HandleSubmissionSequence()
    {
        SetPuzzleInteractive(false);
        if (errorPanel != null) errorPanel.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        if (errorPanel != null) errorPanel.SetActive(false);

        attemptCounter++;
        Debug.Log($"<color=orange>[Puzzle1]</color> Attempt Gagal ke: <b>{attemptCounter}</b>");

        DialogueData dialogueToPlay = null;
        if(minion != null) minion.SetToPuzzleMode();

        if (attemptCounter >= 3)
        {
            if (ovtManager != null) 
            {
                ovtManager.TriggerOVT(attemptCounter);
            }
        }
        else
        {
            List<TaskData> forgottenList = new List<TaskData>();
            List<string> placedIDs = occupiedSlots.Values.ToList();

            foreach (var task in allTasksInLevel)
            {
                if (!placedIDs.Contains(task.taskID)) forgottenList.Add(task);
            }

            if (forgottenList.Count > 0)
            {
                int randomIndex = Random.Range(0, forgottenList.Count);
                dialogueToPlay = forgottenList[randomIndex].forgottenDialogue;
            }
        }

        if (dialogueToPlay != null)
        {
            dialogueManager.StartDialogue(dialogueToPlay);
        }
        else
        {
            SetPuzzleInteractive(true);
        }
    }

    private void ReboundPuzzle()
    {
        if (attemptCounter < 3) SetPuzzleInteractive(true);
    }

    public void SetPuzzleInteractive(bool status)
    {
        if (puzzleCanvasGroup != null)
        {
            puzzleCanvasGroup.interactable = status;
            puzzleCanvasGroup.blocksRaycasts = status;
        }
    }

    public void SetPuzzleState(bool isVisible, bool isInteractable)
    {
        if (puzzleCanvasGroup != null)
        {
            puzzleCanvasGroup.alpha = isVisible ? 1f : 0f;
            puzzleCanvasGroup.interactable = isInteractable;
            puzzleCanvasGroup.blocksRaycasts = isInteractable;
        }
        if (puzzlePanel != null && !puzzlePanel.activeSelf)
            puzzlePanel.SetActive(true);
    }

    public void PlayOvtDialogue()
    {
        int index = attemptCounter - 3; // Attempt 3 = index 0

        // 1. JIKA MASIH DALAM URUTAN 4 DIALOG UTAMA (Attempt 3, 4, 5, 6)
        if (index >= 0 && index < ovtReminderDialogues.Count)
        {
            dialogueManager.StartDialogue(ovtReminderDialogues[index]);
        }
        // 2. JIKA SUDAH LEWAT (Attempt 7 Ke Atas)
        else if (guidingBarks.Count > 0)
        {
            // Ambil secara acak dari list pengingat agar tidak bosan
            int randomBarkIndex = Random.Range(0, guidingBarks.Count);
            dialogueManager.StartDialogue(guidingBarks[randomBarkIndex]);
        }
    }

}