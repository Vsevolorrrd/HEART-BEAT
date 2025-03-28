using TMPro;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] Dialogue dialogue;
    [SerializeField] GameObject dialogueBubble;
    [SerializeField] EmotionController controller;

    public override void Interact()
    {
        DialogueManagerOffBeat.Instance.StartDialogue(dialogue, dialogueBubble, dialogueText, controller);
    }
}