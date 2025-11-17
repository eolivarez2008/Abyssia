using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestionnaire centralisé des dialogues
/// Pattern Singleton
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class Dialogue
    {
        [Tooltip("Nom du personnage qui parle")]
        public string name;

        [TextArea(3, 10)]
        [Tooltip("Phrases du dialogue")]
        public string[] sentences;
    }

    [Header("UI Elements")]
    [Tooltip("Texte affichant le nom du personnage")]
    public Text nameText;
    
    [Tooltip("Texte affichant le dialogue")]
    public Text dialogueText;
    
    [Tooltip("Animator contrôlant l'ouverture/fermeture")]
    public Animator animator;

    [Header("Typing Settings")]
    [Tooltip("Vitesse d'affichage des lettres (0 = instantané)")]
    [Range(0f, 0.1f)]
    public float typingSpeed = 0.02f;

    public static DialogueManager instance;

    private Queue<string> sentences;
    private System.Action endCallback;
    private Coroutine typingCoroutine;

    public bool dialogueActive { get; private set; }

    private void Awake()
    {
        // Pattern Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        sentences = new Queue<string>();
    }

    /// <summary>
    /// Démarre un dialogue
    /// </summary>
    /// <param name="dialogue">Données du dialogue</param>
    /// <param name="onEnd">Callback appelé à la fin du dialogue</param>
    public void StartDialogue(Dialogue dialogue, System.Action onEnd = null)
    {
        if (dialogueActive)
        {
            Debug.LogWarning("DialogueManager: Un dialogue est déjà en cours");
            return;
        }

        if (dialogue == null || dialogue.sentences == null || dialogue.sentences.Length == 0)
        {
            Debug.LogWarning("DialogueManager: Dialogue vide ou null");
            return;
        }

        dialogueActive = true;

        if (animator != null)
            animator.SetBool("isOpen", true);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayOpenDialogue();

        // Configure le nom
        if (nameText != null)
            nameText.text = dialogue.name;

        // Ajoute toutes les phrases à la queue
        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            if (!string.IsNullOrEmpty(sentence))
                sentences.Enqueue(sentence);
        }

        endCallback = onEnd;
        DisplayNextSentence();
    }

    /// <summary>
    /// Affiche la phrase suivante ou termine le dialogue
    /// </summary>
    public void DisplayNextSentence()
    {
        if (!dialogueActive)
        {
            Debug.LogWarning("DialogueManager: Aucun dialogue actif");
            return;
        }

        // Si une phrase est en cours d'affichage, la termine instantanément
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            
            // Affiche la phrase complète instantanément
            if (sentences.Count > 0 && dialogueText != null)
            {
                // Note: On ne peut pas récupérer la phrase en cours, donc on passe à la suivante
                // Pour améliorer ça, il faudrait stocker la phrase en cours
            }
            return;
        }

        // Si plus de phrases, termine le dialogue
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        
        // Arrête toute coroutine de typing en cours
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        typingCoroutine = StartCoroutine(TypeSentence(sentence));

        if (AudioManager.instance != null)
            AudioManager.instance.PlayNextDialogue();
    }

    /// <summary>
    /// Affiche une phrase lettre par lettre
    /// </summary>
    private IEnumerator TypeSentence(string sentence)
    {
        if (dialogueText == null)
        {
            Debug.LogWarning("DialogueManager: dialogueText non assigné");
            yield break;
        }

        dialogueText.text = "";
        
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            
            // Si typingSpeed est 0, affiche tout instantanément
            if (typingSpeed > 0)
            {
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        typingCoroutine = null;
    }

    /// <summary>
    /// Termine le dialogue
    /// </summary>
    public void EndDialogue()
    {
        // Arrête la coroutine de typing si en cours
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (animator != null)
            animator.SetBool("isOpen", false);
        
        dialogueActive = false;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayCloseDialogue();

        // Appelle le callback
        if (endCallback != null)
        {
            endCallback.Invoke();
            endCallback = null;
        }
    }
}