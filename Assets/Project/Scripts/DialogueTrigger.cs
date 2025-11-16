using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Trigger pour initier un dialogue quand le joueur interagit avec
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Configuration")]
    [Tooltip("Configuration du dialogue")]
    public DialogueManager.Dialogue dialogue;
    
    [Header("UI")]
    [Tooltip("Texte affichant comment interagir")]
    public Text interactUI;

    [Header("Input Settings")]
    [Tooltip("Touche pour démarrer le dialogue")]
    public KeyCode interactKey = KeyCode.E;
    
    [Tooltip("Touche pour passer à la phrase suivante")]
    public KeyCode nextKey = KeyCode.Return;
    
    [Tooltip("Touche pour fermer le dialogue")]
    public KeyCode closeKey = KeyCode.Tab;

    private bool isInRange;

    void Update()
    {
        if (DialogueManager.instance == null)
        {
            Debug.LogWarning("DialogueTrigger: DialogueManager.instance est null!");
            return;
        }

        // Démarre le dialogue
        if (isInRange && Input.GetKeyDown(interactKey) && !DialogueManager.instance.dialogueActive)
        {
            TriggerDialogue();
        }

        // Passe à la phrase suivante
        if (DialogueManager.instance.dialogueActive && Input.GetKeyDown(nextKey))
        {
            DialogueManager.instance.DisplayNextSentence();
        }

        // Ferme le dialogue
        if (DialogueManager.instance.dialogueActive && Input.GetKeyDown(closeKey))
        {
            DialogueManager.instance.EndDialogue();
            OnDialogueEnd();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            
            // Affiche l'UI d'interaction si le dialogue n'est pas ouvert
            if (DialogueManager.instance != null && 
                !DialogueManager.instance.dialogueActive && 
                interactUI != null)
            {
                interactUI.enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            
            if (interactUI != null)
                interactUI.enabled = false;
            
            // Ferme le dialogue si le joueur s'éloigne
            if (DialogueManager.instance != null && DialogueManager.instance.dialogueActive)
            {
                DialogueManager.instance.EndDialogue();
            }
        }
    }

    /// <summary>
    /// Démarre le dialogue
    /// </summary>
    void TriggerDialogue()
    {
        if (DialogueManager.instance == null)
        {
            Debug.LogError("DialogueTrigger: DialogueManager.instance est null!");
            return;
        }

        if (interactUI != null)
            interactUI.enabled = false;
        
        DialogueManager.instance.StartDialogue(dialogue, OnDialogueEnd);
    }

    /// <summary>
    /// Appelé quand le dialogue se termine
    /// </summary>
    void OnDialogueEnd()
    {
        // Réaffiche l'UI d'interaction si le joueur est encore dans la zone
        if (isInRange && interactUI != null)
            interactUI.enabled = true;
    }
}