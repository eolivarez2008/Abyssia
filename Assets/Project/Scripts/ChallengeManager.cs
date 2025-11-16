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

    private Dictionary<string, ChallengeData> activeChallenges = new Dictionary<string, ChallengeData>();
    
    private class ChallengeData
    {
        public int enemiesRemaining;
        public Challenge challenge;
        public System.Action onComplete;
    }

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

        if (challengeTitle != null)
            challengeTitle.text = challenge.title;
        if (challengeDescription != null)
            challengeDescription.text = challenge.description;

        if (acceptButton != null)
        {
            acceptButton.interactable = true;
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(() => {
                if (onAccept != null) onAccept.Invoke();
                EndChallenge();
                if (onEnd != null) onEnd.Invoke();
            });
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

        if (challengeTitle != null)
            challengeTitle.text = rewardTitle;
        if (challengeDescription != null)
            challengeDescription.text = rewardDescription;

        if (claimButton != null)
        {
            claimButton.interactable = true;
            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(() => {
                ClaimReward(challenge);
                if (onClaim != null) onClaim.Invoke();
                EndChallenge();
                if (onEnd != null) onEnd.Invoke();
            });
        }

        if (acceptButton != null)
            acceptButton.interactable = false;

        if (animator != null)
            animator.SetBool("isOpen", true);
    }

    private void ClaimReward(Challenge challenge)
    {
        if (challenge != null)
        {
            if (challenge.rewardCoins > 0 && Inventory.instance != null)
                Inventory.instance.AddCoins(challenge.rewardCoins);

            if (challenge.rewardItems != null && Inventory.instance != null)
            {
                foreach (Item item in challenge.rewardItems)
                {
                    if (item != null)
                        Inventory.instance.content.Add(item);
                }
            }

            if (Inventory.instance != null)
                Inventory.instance.UpdateInventoryUI();
        }
    }

    public void OpenChallengeCompleted(string title, string description, System.Action onEnd = null)
    {
        if (challengeMenuOpen) return;

        challengeMenuOpen = true;

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
    }

    public void RegisterChallenge(string challengeID, int enemyCount, Challenge challenge, System.Action onComplete)
    {
        if (activeChallenges.ContainsKey(challengeID))
        {
            Debug.LogWarning($"Challenge {challengeID} déjà enregistré!");
            return;
        }

        activeChallenges[challengeID] = new ChallengeData
        {
            enemiesRemaining = enemyCount,
            challenge = challenge,
            onComplete = onComplete
        };

        Debug.Log($"Challenge {challengeID} enregistré avec {enemyCount} ennemis");
    }

    public void OnChallengeEnemyKilled(string challengeID)
    {
        if (string.IsNullOrEmpty(challengeID)) return;

        if (!activeChallenges.ContainsKey(challengeID))
        {
            Debug.LogWarning($"Challenge {challengeID} non trouvé!");
            return;
        }

        ChallengeData data = activeChallenges[challengeID];
        data.enemiesRemaining--;

        Debug.Log($"Challenge {challengeID}: {data.enemiesRemaining} ennemis restants");

        if (data.enemiesRemaining <= 0)
        {
            Debug.Log($"Challenge {challengeID} terminé!");
            
            if (data.onComplete != null)
                data.onComplete.Invoke();

            activeChallenges.Remove(challengeID);
        }
    }

    public bool IsChallengeActive(string challengeID)
    {
        return activeChallenges.ContainsKey(challengeID);
    }
}