using System;
using TMPro;
using UnityEngine;

public class DialogueManager : RhythmInput
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private Dialogue currentDialogue;

    private int nodeIndex = 0;
    private int blockIndex = 0;
    private bool awaitingInput = false;
    private EmotionController emotionController;

    public static Action<bool> OnPauseInput;

    private static DialogueManager _instance;

    #region Singleton
    public static DialogueManager Instance => _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("DialogueManager already exists, destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion

    protected override void Start()
    {
        base.Start();
        dialogueUI.SetActive(false);
    }

    public void StartDialogue(Dialogue newDialogue, EmotionController emotionControl = null)
    {
        if (emotionControl)
        emotionController = emotionControl;

        currentDialogue = newDialogue;
        nodeIndex = 0;
        blockIndex = 0;
        dialogueUI.SetActive(true);
        awaitingInput = true;
        dialogueText.text = ""; // Start with empty text
        AdvanceDialogue();

        OnPauseInput?.Invoke(true);
    }

    protected override void Update()
    {
        if (!playerInput || isBlocked || !awaitingInput)
            return;

        if (Input.GetKeyDown(actionKey))
        {
            HandleKeyPress();
        }
    }

    protected override void OnPerfectHit()
    {
        AdvanceDialogue();
    }
    protected override void OnGoodHit()
    {
        AdvanceDialogue();
    }

    private void AdvanceDialogue()
    {
        if (currentDialogue == null) return;

        if (emotionController)
        emotionController.SetEmotion(EmotionType.Speaking, true, 0.3f);

        var node = currentDialogue.dialogueNodes[nodeIndex];
        if (blockIndex < node.blocks.Count)
        {
            dialogueText.text += " " + node.blocks[blockIndex].dialogueText; // add spaces between blocks
            blockIndex++;
        }
        else
        {
            NextNode();
        }
    }

    private void NextNode()
    {
        nodeIndex++;
        blockIndex = 0;
        dialogueText.text = ""; // empty text

        if (nodeIndex >= currentDialogue.dialogueNodes.Count)
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialogueText.text = "";
        dialogueUI.SetActive(false);
        awaitingInput = false;
        currentDialogue = null;

        OnPauseInput?.Invoke(false);
    }
}