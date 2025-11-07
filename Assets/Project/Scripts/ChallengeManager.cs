using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeManager : MonoBehaviour
{
    [System.Serializable]
    public class Challenge
    {
        public string title;
        [TextArea(3, 5)]
        public string description;
        public GameObject[] enemyPrefabs;
        public int rewardCoins;
        public Item[] rewardItems;
    }

    [Header("UI Elements")]
    public Text challengeTitle;
    public Text challengeDescription;
    public Button acceptButton;
    public Button claimButton;
    public Animator animator;

    [Header("Text Messages")]
    public string rewardTitle = "Récompense !";
    public string rewardDescription = "Défi terminé ! Récupère ta récompense.";

    public static ChallengeManager instance;

    private bool isChallengeActive = false;
    private int enemiesRemaining = 0;
    private Challenge currentChallenge;
    private System.Action onChallengeAccepted;
    private System.Action onRewardClaimed;
    private System.Action endCallback;

    public bool challengeMenuOpen { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de ChallengeManager dans la scène");
            return;
        }

        instance = this;

        if (claimButton != null)
            claimButton.interactable = false;
        if (acceptButton != null)
            acceptButton.interactable = false;
    }

    /// <summary>
    /// Ouvre le menu de challenge - OFFRE (avant acceptation)
    /// </summary>
    public void OpenChallengeOffer(Challenge challenge, System.Action onAccept, System.Action onEnd = null)
    {
        if (challengeMenuOpen) return;

        challengeMenuOpen = true;
        currentChallenge = challenge;
        onChallengeAccepted = onAccept;
        endCallback = onEnd;

        challengeTitle.text = challenge.title;
        challengeDescription.text = challenge.description;

        // Active le bouton accepter, désactive réclamer
        if (acceptButton != null)
        {
            acceptButton.interactable = true;
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(AcceptChallenge);
        }

        if (claimButton != null)
            claimButton.interactable = false;

        animator.SetBool("isOpen", true);
    }

    /// <summary>
    /// Ouvre le menu de challenge - RÉCOMPENSE (après victoire)
    /// </summary>
    public void OpenChallengeReward(Challenge challenge, System.Action onClaim, System.Action onEnd = null)
    {
        if (challengeMenuOpen) return;

        challengeMenuOpen = true;
        currentChallenge = challenge;
        onRewardClaimed = onClaim;
        endCallback = onEnd;

        challengeTitle.text = rewardTitle;
        challengeDescription.text = rewardDescription;

        // Active le bouton réclamer, désactive accepter
        if (claimButton != null)
        {
            claimButton.interactable = true;
            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(ClaimReward);
        }

        if (acceptButton != null)
            acceptButton.interactable = false;

        animator.SetBool("isOpen", true);
    }

    /// <summary>
    /// Accepte le challenge
    /// </summary>
    private void AcceptChallenge()
    {
        isChallengeActive = true;
        EndChallenge(); // Ferme le menu

        if (onChallengeAccepted != null)
        {
            onChallengeAccepted.Invoke();
        }
    }

    /// <summary>
    /// Réclame la récompense
    /// </summary>
    private void ClaimReward()
    {
        if (currentChallenge != null)
        {
            if (currentChallenge.rewardCoins > 0)
                Inventory.instance.AddCoins(currentChallenge.rewardCoins);

            if (currentChallenge.rewardItems != null)
            {
                foreach (Item item in currentChallenge.rewardItems)
                {
                    if (item != null)
                        Inventory.instance.content.Add(item);
                }
            }

            Inventory.instance.UpdateInventoryUI();
        }

        EndChallenge();

        if (onRewardClaimed != null)
        {
            onRewardClaimed.Invoke();
            onRewardClaimed = null;
        }
    }

    /// <summary>
    /// Ouvre le menu "Déjà terminé" (après récompense réclamée)
    /// </summary>
    public void OpenChallengeCompleted(string title, string description, System.Action onEnd = null)
    {
        if (challengeMenuOpen) return;

        challengeMenuOpen = true;
        endCallback = onEnd;

        challengeTitle.text = title;
        challengeDescription.text = description;

        // Désactive les deux boutons
        if (acceptButton != null)
            acceptButton.interactable = false;
        if (claimButton != null)
            claimButton.interactable = false;

        animator.SetBool("isOpen", true);
    }

    /// <summary>
    /// Ferme le menu de challenge
    /// </summary>
    public void EndChallenge()
    {
        animator.SetBool("isOpen", false);
        challengeMenuOpen = false;

        if (acceptButton != null)
            acceptButton.interactable = false;
        if (claimButton != null)
            claimButton.interactable = false;

        if (endCallback != null)
        {
            endCallback.Invoke();
            endCallback = null;
        }
    }

    /// <summary>
    /// Démarre le comptage des ennemis
    /// </summary>
    public void StartChallenge(int enemyCount)
    {
        isChallengeActive = true;
        enemiesRemaining = enemyCount;
    }

    /// <summary>
    /// Appelé quand un ennemi du challenge meurt
    /// </summary>
    public void OnChallengeEnemyKilled()
    {
        if (!isChallengeActive) return;

        enemiesRemaining--;

        if (enemiesRemaining <= 0)
        {
            CompleteChallenge();
        }
    }

    /// <summary>
    /// Termine le challenge
    /// </summary>
    private void CompleteChallenge()
    {
        isChallengeActive = false;
        Debug.Log("Challenge terminé ! Retourne voir le NPC pour réclamer ta récompense.");
    }

    public bool IsChallengeActive()
    {
        return isChallengeActive;
    }
}