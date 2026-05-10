using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueTextUI;
    [SerializeField] private TextMeshProUGUI nameTextUI;
    
    [Header("Character Expressions")]
    [SerializeField] private Image avaExpressionImage;
    [SerializeField] private Image minionExpressionImage;

    [Header("Animation")]
    [SerializeField] private Animator animator; 

    [Header("Character Styling")]
    [SerializeField] private Color minionColor = Color.yellow;
    [SerializeField] private Color avaColor = Color.cyan;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] typingSounds;
    [Range(1, 5)][SerializeField] private int frequencyLevel = 2;
    [SerializeField] private AudioSource dialogueAudioSource;
    [Range(0f, 1f)][SerializeField] private float typingVolume = 0.5f;

    [Header("Settings")]
    [SerializeField] public float charactersPerSecond = 25f;

    // Internal State
    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string currentFullLine;
    private bool shouldKeepPanelOpen = false;

    public static event Action OnDialogueEnded;
    private Action currentCallback;

    void Awake()
    {
       if (dialogueAudioSource == null)
       {
            dialogueAudioSource = GetComponent<AudioSource>();
            if (dialogueAudioSource == null) dialogueAudioSource = gameObject.AddComponent<AudioSource>();
       }

       if (avaExpressionImage != null) avaExpressionImage.gameObject.SetActive(false);
       if (minionExpressionImage != null) minionExpressionImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            if (isTyping) CompleteLine(); 
            else DisplayNextLine(); 
        }
    }

    public void StartDialogue(DialogueData dialogueData, Action onComplete = null)
    {
        if (isDialogueActive) return;
        currentCallback = onComplete;
        isDialogueActive = true;
        shouldKeepPanelOpen = dialogueData.keepPanelOpenAtEnd;
        dialogueQueue.Clear();

        //if (avaExpressionImage != null) avaExpressionImage.gameObject.SetActive(true);
        //if (minionExpressionImage != null) minionExpressionImage.gameObject.SetActive(true);

        foreach (DialogueLine line in dialogueData.lines)
        {
            dialogueQueue.Enqueue(line);
        }
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = dialogueQueue.Dequeue();
        StopAllCoroutines();
        currentFullLine = currentLine.lineText;

        string charName = currentLine.characterName.ToUpper().Trim();
        if (nameTextUI != null) nameTextUI.text = currentLine.characterName;
        
        if (currentLine.commands != null) ProcessCommands(currentLine.commands, charName);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(currentLine));
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;
        dialogueTextUI.text = "";
        string charName = line.characterName.ToUpper().Trim();

        if (charName == "MINION")
        {
            dialogueTextUI.color = minionColor;
            dialogueTextUI.alignment = TextAlignmentOptions.MidlineRight;
        }
        else if (charName == "AVA")
        {
            dialogueTextUI.color = avaColor;
            dialogueTextUI.alignment = TextAlignmentOptions.MidlineLeft;
        }
        else
        {
            dialogueTextUI.color = Color.white;
            dialogueTextUI.alignment = TextAlignmentOptions.MidlineLeft;
        }

        char[] charArray = currentFullLine.ToCharArray();
        for (int i = 0; i < charArray.Length; i++)
        {
            dialogueTextUI.text += charArray[i];
            if (charArray[i] != ' ' && i % frequencyLevel == 0) PlayTypingSound();
            yield return new WaitForSeconds(1f / charactersPerSecond);
        }
        isTyping = false;
    }

    private void CompleteLine()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueTextUI.text = currentFullLine;
        isTyping = false;
    }

    private void PlayTypingSound()
    {
        if (dialogueAudioSource != null && typingSounds != null && typingSounds.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, typingSounds.Length);
            dialogueAudioSource.PlayOneShot(typingSounds[index], typingVolume);
        }
    }

    private void ProcessCommands(GameCommand[] commands, string currentSpeaker)
    {
        foreach (GameCommand cmd in commands)
        {
            if (cmd.type == CommandType.PlaySound && cmd.audioAsset != null)
            {
                dialogueAudioSource.PlayOneShot(cmd.audioAsset);
            }
            else if (cmd.type == CommandType.ChangeBackground && cmd.spriteAsset != null)
            {
                if (currentSpeaker == "MINION")
                {
                    // Ganti ekspresi Minion
                    if (minionExpressionImage != null) {
                        minionExpressionImage.sprite = cmd.spriteAsset;
                        minionExpressionImage.gameObject.SetActive(true);
                    }
                }
                else 
                {
                // Default ke Ava
                    if (avaExpressionImage != null) {
                        avaExpressionImage.sprite = cmd.spriteAsset;
                        avaExpressionImage.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void ForceCloseDialogue()
    {
        isDialogueActive = false;
        StopAllCoroutines();
        if (animator != null) animator.SetBool("IsOpen", false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        if (dialogueTextUI != null) dialogueTextUI.text = "";

        if (currentCallback != null)
        {
            currentCallback.Invoke(); 
            currentCallback = null; 
        }

        OnDialogueEnded?.Invoke();
        Debug.Log("Dialog Selesai. Event Dikirim!");
    }
}