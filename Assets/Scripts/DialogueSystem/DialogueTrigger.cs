using UnityEngine;

public class DialogueTrigger : Interactable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] EmotionController controller;

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogue, controller);
    }
}