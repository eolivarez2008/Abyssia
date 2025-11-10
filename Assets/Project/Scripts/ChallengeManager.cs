using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestionnaire centralisé des défis. Gère l'affichage de l'UI et le suivi des ennemis.
/// Pattern Singleton
/// </summary>
public class ChallengeManager : MonoBehaviour
{
    [System.Serializable]
    public class Challenge
    {
        [Tooltip("Titre du défi")]
        public string title;
        
        [TextArea(3, 5)]
        [Tooltip("Description du défi")]
        public string description;
        
        [Tooltip("Prefabs des ennemis à faire apparaître")]
        public GameObject[] enemyPrefabs;
        
        [Tooltip("Nombre de pièces données en récompense")]
        public int rewardCoins;
        
        [Tooltip("Items donnés en récompense")]
        public Item[] rewardItems;
    }

    [Header("UI Elements")]
    [Tooltip("Texte affichant le titre du défi/récompense")]
    public Text challengeTitle;
    
    [Tooltip("Texte affichant la description du défi/récompense")]
    public Text challengeDescription;
    
    [Tooltip("Bouton pour accepter le défi")]
    public Button acceptButton;
    
    [Tooltip("Bouton pour récupérer la récompense")]
    public Button claimButton;
    
    [Tooltip("Animator contrôlant l'ouverture/fermeture du panneau")]
    public Animator animator;

    [Header("Text Messages")]
    [Tooltip("Titre affiché lors de la récompense")]
    public string rewardTitle = "Récompense !";
    
    [Tooltip("Description affichée lors de la récompense")]
    [TextArea(2, 4)]
    public string rewardDescription = "Défi terminé ! Récupère ta récompense.";

    public static ChallengeManager instance;

    // Stocke les données des défis actifs en cours
    private Dictionary<string, ChallengeData> activeChallenges = new Dictionary<string, ChallengeData>();
    
    private class ChallengeData
    {
        public int enemiesRemaining;
        public Challenge challenge;
        public System.Action onComplete;
    }

    // État actuel de l'UI
    private System.Action currentEndCallback;

    public bool IsChallengeMenuOpen { get; private set; }

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

        // Désactive les boutons au démarrage
        if (claimButton != null)
            claimButton.interactable = false;
        if (acceptButton != null)
            acceptButton.interactable = false;
    }

    /// <summary>
    /// Ouvre le panneau pour proposer un défi au joueur
    /// </summary>
    /// <param name="challenge">Données du défi</param>
    /// <param name="onAccept">Callback appelé quand le joueur accepte</param>
    /// <param name="onEnd">Callback appelé quand le panneau se ferme</param>
    public void OpenChallengeOffer(Challenge challenge, System.Action onAccept, System.Action onEnd = null)
    {
        if (IsChallengeMenuOpen) return;

        IsChallengeMenuOpen = true;
        currentEndCallback = onEnd;

        // Configure le texte
        if (challengeTitle != null)
            challengeTitle.text = challenge.title;
        if (challengeDescription != null)
            challengeDescription.text = challenge.description;

        // Configure le bouton d'acceptation
        if (acceptButton != null)
        {
            acceptButton.interactable = true;
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(() => {
                if (onAccept != null) onAccept.Invoke();
                CloseChallenge();
                
                if (AudioManager.instance != null)
                    AudioManager.instance.PlayValidate();
            });
        }

        // Désactive le bouton de récupération
        if (claimButton != null)
            claimButton.interactable = false;

        // Ouvre l'animation
        if (animator != null)
            animator.SetBool("isOpen", true);
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayOpenDialogue();
    }

    /// <summary>
    /// Ouvre le panneau pour réclamer la récompense d'un défi complété
    /// </summary>
    /// <param name="challenge">Données du défi complété</param>
    /// <param name="onClaim">Callback appelé quand le joueur récupère la récompense</param>
    /// <param name="onEnd">Callback appelé quand le panneau se ferme</param>
    public void OpenChallengeReward(Challenge challenge, System.Action onClaim, System.Action onEnd = null)
    {
        if (IsChallengeMenuOpen) return;

        IsChallengeMenuOpen = true;
        currentEndCallback = onEnd;

        // Configure le texte de récompense
        if (challengeTitle != null)
            challengeTitle.text = rewardTitle;
        if (challengeDescription != null)
            challengeDescription.text = rewardDescription;

        // Configure le bouton de récupération
        if (claimButton != null)
        {
            claimButton.interactable = true;
            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(() => {
                ClaimReward(challenge);
                if (onClaim != null) onClaim.Invoke();
                CloseChallenge();
            });
        }

        // Désactive le bouton d'acceptation
        if (acceptButton != null)
            acceptButton.interactable = false;

        // Ouvre l'animation
        if (animator != null)
            animator.SetBool("isOpen", true);
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayOpenDialogue();
    }

    /// <summary>
    /// Ouvre un panneau d'information (défi déjà complété)
    /// </summary>
    public void OpenChallengeCompleted(string title, string description, System.Action onEnd = null)
    {
        if (IsChallengeMenuOpen) return;

        IsChallengeMenuOpen = true;
        currentEndCallback = onEnd;

        // Configure le texte
        if (challengeTitle != null)
            challengeTitle.text = title;
        if (challengeDescription != null)
            challengeDescription.text = description;

        // Désactive les deux boutons
        if (acceptButton != null)
            acceptButton.interactable = false;
        if (claimButton != null)
            claimButton.interactable = false;

        // Ouvre l'animation
        if (animator != null)
            animator.SetBool("isOpen", true);
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayOpenDialogue();
    }

    /// <summary>
    /// Ferme le panneau de défi
    /// </summary>
    public void CloseChallenge()
    {
        if (animator != null)
            animator.SetBool("isOpen", false);
            
        IsChallengeMenuOpen = false;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayCloseDialogue();

        // Désactive les boutons
        if (acceptButton != null)
            acceptButton.interactable = false;
        if (claimButton != null)
            claimButton.interactable = false;

        // Appelle le callback de fin
        if (currentEndCallback != null)
        {
            currentEndCallback.Invoke();
            currentEndCallback = null;
        }
    }

    /// <summary>
    /// Donne la récompense du défi au joueur
    /// </summary>
    private void ClaimReward(Challenge challenge)
    {
        if (challenge == null) return;

        // Ajoute les pièces
        if (challenge.rewardCoins > 0 && Inventory.instance != null)
        {
            Inventory.instance.AddCoins(challenge.rewardCoins);
        }

        // Ajoute les items
        if (challenge.rewardItems != null && Inventory.instance != null)
        {
            foreach (Item item in challenge.rewardItems)
            {
                if (item != null)
                    Inventory.instance.content.Add(item);
            }
        }

        // Met à jour l'UI de l'inventaire
        if (Inventory.instance != null)
            Inventory.instance.UpdateInventoryUI();

        if (AudioManager.instance != null)
            AudioManager.instance.PlayReward();
    }

    /// <summary>
    /// Enregistre un nouveau défi actif
    /// </summary>
    /// <param name="challengeID">ID unique du défi</param>
    /// <param name="enemyCount">Nombre d'ennemis à éliminer</param>
    /// <param name="challenge">Données du défi</param>
    /// <param name="onComplete">Callback appelé quand tous les ennemis sont éliminés</param>
    public void RegisterChallenge(string challengeID, int enemyCount, Challenge challenge, System.Action onComplete)
    {
        if (string.IsNullOrEmpty(challengeID))
        {
            Debug.LogWarning("ChallengeManager: Tentative d'enregistrement d'un défi avec un ID vide");
            return;
        }

        if (activeChallenges.ContainsKey(challengeID))
        {
            Debug.LogWarning($"ChallengeManager: Le défi {challengeID} est déjà actif");
            return;
        }

        activeChallenges[challengeID] = new ChallengeData
        {
            enemiesRemaining = enemyCount,
            challenge = challenge,
            onComplete = onComplete
        };

        Debug.Log($"ChallengeManager: Défi {challengeID} enregistré avec {enemyCount} ennemis");
    }

    /// <summary>
    /// Appelé quand un ennemi de défi est tué
    /// </summary>
    /// <param name="challengeID">ID du défi auquel appartient l'ennemi</param>
    public void OnChallengeEnemyKilled(string challengeID)
    {
        if (string.IsNullOrEmpty(challengeID))
        {
            Debug.LogWarning("ChallengeManager: Tentative de tuer un ennemi avec un ID de défi vide");
            return;
        }

        if (!activeChallenges.ContainsKey(challengeID))
        {
            Debug.LogWarning($"ChallengeManager: Aucun défi actif trouvé avec l'ID {challengeID}");
            return;
        }

        ChallengeData data = activeChallenges[challengeID];
        data.enemiesRemaining--;

        Debug.Log($"ChallengeManager: Ennemi tué dans {challengeID}. Restants: {data.enemiesRemaining}");

        // Si tous les ennemis sont éliminés
        if (data.enemiesRemaining <= 0)
        {
            Debug.Log($"ChallengeManager: Défi {challengeID} complété!");
            
            if (data.onComplete != null)
                data.onComplete.Invoke();

            activeChallenges.Remove(challengeID);
        }
    }

    /// <summary>
    /// Vérifie si un défi est actuellement actif
    /// </summary>
    public bool IsChallengeActive(string challengeID)
    {
        return !string.IsNullOrEmpty(challengeID) && activeChallenges.ContainsKey(challengeID);
    }
}