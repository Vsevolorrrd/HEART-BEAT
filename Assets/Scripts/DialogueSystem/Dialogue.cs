using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Header("Dialogue")]
    public List<DialogueNode> dialogueNodes;
}
[System.Serializable]
public class DialogueNode
{
    public List<DialogueBlocks> blocks;
}
[System.Serializable]
public class DialogueBlocks
{
    [TextArea(3, 10)]
    public string dialogueText;
}