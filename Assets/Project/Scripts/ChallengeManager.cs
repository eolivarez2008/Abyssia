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
            return;
        }

        instance = this;

        if (claimButton != null)
            claimButton.interactable = false;
        if (acceptButton != null)
            acceptButton.interactable = false;
    }

    public void OpenChallengeOffer(Challenge challenge, System.Action onAccept, System.Action onEnd = null)
    {
        if (challengeMenuOpen) return;

        challengeMenuOpen = true;
        currentChallenge = challenge;
        onChallengeAccepted = onAccept;
        endCallback = onEnd;

        if (challengeTitle != null)
            challengeTitle.text = challenge.title;
        if (challengeDescription != null)
            challengeDescription.text = challenge.description;

        if (acceptButton != null)
        {
            acceptButton.interactable = true;
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(AcceptChallenge);
        }

        if (claimButton != null)
            claimButton.interactable = false;

        if (animator != null)
            animator.SetBool("isOpen", true);
    }

    public void OpenChallengeReward(Challenge challenge, System.Action onClaim, System.Action onEnd = null)
    {
        if (challengeMenuOpen) return;

        challengeMenuOpen = true;
        currentChallenge = challenge;
        onRewardClaimed = onClaim;
        endCallback = onEnd;

        if (challengeTitle != null)
            challengeTitle.text = rewardTitle;
        if (challengeDescription != null)
            challengeDescription.text = rewardDescription;

        if (claimButton != null)
        {
            claimButton.interactable = true;
            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(ClaimReward);
        }

        if (acceptButton != null)
            acceptButton.interactable = false;

        if (animator != null)
            animator.SetBool("isOpen", true);
    }

    private void AcceptChallenge()
    {
        isChallengeActive = true;
        EndChallenge();

        if (onChallengeAccepted != null)
        {
            onChallengeAccepted.Invoke();
        }
    }

    private void ClaimReward()
    {
        if (currentChallenge != null)
        {
            if (currentChallenge.rewardCoins > 0 && Inventory.instance != null)
                Inventory.instance.AddCoins(currentChallenge.rewardCoins);

            if (currentChallenge.rewardItems != null && Inventory.instance != null)
            {
                foreach (Item item in currentChallenge.rewardItems)
                {
                    if (item != null)
                        Inventory.instance.content.Add(item);
                }
            }

            if (Inventory.instance != null)
                Inventory.instance.UpdateInventoryUI();
        }

        EndChallenge();

        if (onRewardClaimed != null)
        {
            onRewardClaimed.Invoke();
            onRewardClaimed = null;
        }
    }

    public void OpenChallengeCompleted(string title, string description, System.Action onEnd = null)
    {
        if (challengeMenuOpen) return;

        challengeMenuOpen = true;
        endCallback = onEnd;

        if (challengeTitle != null)
            challengeTitle.text = title;
        if (challengeDescription != null)
            challengeDescription.text = description;

        if (acceptButton != null)
            acceptButton.interactable = false;
        if (claimButton != null)
            claimButton.interactable = false;

        if (animator != null)
            animator.SetBool("isOpen", true);
    }

    public void EndChallenge()
    {
        if (animator != null)
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

    public void StartChallenge(int enemyCount)
    {
        isChallengeActive = true;
        enemiesRemaining = enemyCount;
    }

    public void OnChallengeEnemyKilled()
    {
        if (!isChallengeActive) return;

        enemiesRemaining--;

        if (enemiesRemaining <= 0)
        {
            CompleteChallenge();
        }
    }

    private void CompleteChallenge()
    {
        isChallengeActive = false;
    }

    public bool IsChallengeActive()
    {
        return isChallengeActive;
    }
}