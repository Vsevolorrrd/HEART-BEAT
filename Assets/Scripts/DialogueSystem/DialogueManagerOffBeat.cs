using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueManagerOffBeat : RespondToBeat
{
    [SerializeField] Dialogue currentDialogue;
    [SerializeField] float delay = 2f;

    private TMP_Text dialogueText;
    private GameObject dialogueUI;
    private int nodeIndex = 0;
    private int blockIndex = 0;
    private bool isWaiting = false;
    private bool started = false;
    private EmotionController emotionController;

    private static DialogueManagerOffBeat _instance;

    #region Singleton
    public static DialogueManagerOffBeat Instance => _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("DialogueManagerOffBeat already exists, destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion

    public void StartDialogue(Dialogue newDialogue, GameObject bubble, TMP_Text text, EmotionController emotionControl = null)
    {
        if (emotionControl)
        emotionController = emotionControl;

        currentDialogue = newDialogue;
        dialogueText = text;
        dialogueUI = bubble;
        nodeIndex = 0;
        blockIndex = 0;
        dialogueUI.SetActive(true);
        dialogueText.text = ""; // Start with empty text
        isWaiting = false;
        started = true;
        AdvanceDialogue();
    }

    protected override void OnBeat()
    {
        if (currentDialogue == null || isWaiting || !started)
        return;

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

            if (blockIndex == node.blocks.Count)
            {
                isWaiting = true;
                StartCoroutine(DelayBeforeNextBlock());
            }
        }
        else
        {
            NextNode();
        }
    }

    private IEnumerator DelayBeforeNextBlock()
    {
        yield return new WaitForSeconds(delay);
        isWaiting = false;
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
        isWaiting = false;
        started = false;
        currentDialogue = null;
        dialogueUI = null;
    }
}