using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class Dialogue
    {
        public string name;

        [TextArea(3, 10)]
        public string[] sentences;
    }

    public Text nameText;
    public Text dialogueText;
    public Animator animator;

    private Queue<string> sentences;
    private System.Action endCallback;

    public static DialogueManager instance;

    public bool dialogueActive { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue, System.Action onEnd = null)
    {
        if (dialogueActive) return;

        dialogueActive = true;
        animator.SetBool("isOpen", true);

        nameText.text = dialogue.name;
        sentences.Clear();
        foreach (string s in dialogue.sentences)
            sentences.Enqueue(s);

        endCallback = onEnd;
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void EndDialogue()
    {
        animator.SetBool("isOpen", false);
        dialogueActive = false;

        if (endCallback != null)
        {
            endCallback.Invoke();
            endCallback = null;
        }
    }
}
