using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueManager.Dialogue dialogue;
    public Text interactUI;

    private bool isInRange;

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue();
        }

        if (DialogueManager.instance.dialogueActive && Input.GetKeyDown(KeyCode.Return))
        {
            DialogueManager.instance.DisplayNextSentence();
        }

        if (DialogueManager.instance != null && DialogueManager.instance.dialogueActive && Input.GetKeyDown(KeyCode.Tab))
        {
            DialogueManager.instance.EndDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            if (!DialogueManager.instance.dialogueActive)
                interactUI.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            interactUI.enabled = false;
            DialogueManager.instance.EndDialogue();
        }
    }

    void TriggerDialogue()
    {
        interactUI.enabled = false;
        DialogueManager.instance.StartDialogue(dialogue, OnDialogueEnd);
    }

    void OnDialogueEnd()
    {
        if (isInRange)
            interactUI.enabled = true;
    }
}
