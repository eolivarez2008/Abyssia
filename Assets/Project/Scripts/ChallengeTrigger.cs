using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Trigger pour initier un défi quand le joueur interagit avec
/// Fonctionne sur le même principe que ShopTrigger et DialogueTrigger
/// </summary>
public class ChallengeTrigger : MonoBehaviour
{
    [Header("Challenge Configuration")]
    [Tooltip("Configuration du défi")]
    public ChallengeManager.Challenge challenge;
    
    [Header("Challenge Unique ID")]
    [Tooltip("ID unique pour ce challenge. Doit être différent pour chaque ChallengeTrigger!")]
    public string challengeID = "challenge_1";

    [Header("Interaction")]
    [Tooltip("Texte UI montrant comment interagir")]
    public Text interactUI;
    
    [Header("Spawn Settings")]
    [Tooltip("Délai entre chaque apparition d'ennemi")]
    public float spawnDelay = 0.5f;
    
    [Tooltip("Délai avant de commencer à faire apparaître les ennemis")]
    public float delayBeforeSpawn = 1f;

    [Header("Completed Message")]
    [Tooltip("Titre affiché si le défi est déjà complété")]
    public string completedTitle = "Défi terminé";
    
    [TextArea(3, 5)]
    [Tooltip("Description affichée si le défi est déjà complété")]
    public string completedDescription = "Vous avez déjà complété ce défi et réclamé votre récompense.";

    [Header("Input Settings")]
    [Tooltip("Touche pour interagir avec le défi")]
    public KeyCode interactKey = KeyCode.E;
    
    [Tooltip("Touche pour fermer l'UI du défi")]
    public KeyCode closeKey = KeyCode.Tab;

    private bool isInRange;
    private bool challengeCompleted = false;
    private bool rewardClaimed = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Update()
    {
        // Interaction pour ouvrir le défi
        if (isInRange && Input.GetKeyDown(interactKey) && 
            ChallengeManager.instance != null && 
            !ChallengeManager.instance.IsChallengeActive(challengeID))
        {
            TriggerChallenge();
        }

        // Fermeture du menu avec Tab
        if (ChallengeManager.instance != null && 
            ChallengeManager.instance.IsChallengeMenuOpen && 
            Input.GetKeyDown(closeKey))
        {
            ChallengeManager.instance.CloseChallenge();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            
            // Affiche l'UI d'interaction si le menu n'est pas ouvert et le défi pas actif
            if (ChallengeManager.instance != null && 
                !ChallengeManager.instance.IsChallengeMenuOpen && 
                !ChallengeManager.instance.IsChallengeActive(challengeID) &&
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
            
            // Ferme le menu si ouvert
            if (ChallengeManager.instance != null && ChallengeManager.instance.IsChallengeMenuOpen)
            {
                ChallengeManager.instance.CloseChallenge();
            }
        }
    }

    /// <summary>
    /// Déclenche l'ouverture du menu de défi
    /// </summary>
    void TriggerChallenge()
    {
        if (ChallengeManager.instance == null)
        {
            Debug.LogError("ChallengeTrigger: ChallengeManager.instance est null!");
            return;
        }

        if (interactUI != null)
            interactUI.enabled = false;

        // Si le défi est complété ET la récompense récupérée
        if (rewardClaimed)
        {
            ChallengeManager.instance.OpenChallengeCompleted(
                completedTitle, 
                completedDescription, 
                OnChallengeMenuClosed
            );
        }
        // Si le défi est complété MAIS la récompense pas encore récupérée
        else if (challengeCompleted && !rewardClaimed)
        {
            ChallengeManager.instance.OpenChallengeReward(
                challenge, 
                OnRewardClaimed, 
                OnChallengeMenuClosed
            );
        }
        // Si le défi n'est pas encore accepté/complété
        else
        {
            ChallengeManager.instance.OpenChallengeOffer(
                challenge, 
                StartChallenge, 
                OnChallengeMenuClosed
            );
        }
    }

    /// <summary>
    /// Lance le défi en faisant apparaître les ennemis
    /// </summary>
    void StartChallenge()
    {
        StartCoroutine(SpawnEnemiesWithDelay());
    }

    /// <summary>
    /// Attend avant de commencer à faire apparaître les ennemis
    /// </summary>
    private System.Collections.IEnumerator SpawnEnemiesWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeSpawn);
        yield return StartCoroutine(SpawnEnemies());
    }

    /// <summary>
    /// Fait apparaître tous les ennemis du défi avec un délai entre chaque
    /// </summary>
    private System.Collections.IEnumerator SpawnEnemies()
    {
        if (challenge.enemyPrefabs == null || challenge.enemyPrefabs.Length == 0)
        {
            Debug.LogWarning($"ChallengeTrigger {challengeID}: Aucun ennemi configuré!");
            yield break;
        }

        spawnedEnemies.Clear();
        int totalEnemies = 0;

        for (int i = 0; i < challenge.enemyPrefabs.Length; i++)
        {
            if (challenge.enemyPrefabs[i] == null)
            {
                Debug.LogWarning($"ChallengeTrigger {challengeID}: Prefab ennemi {i} est null!");
                continue;
            }

            // Position aléatoire autour du trigger
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0
            );

            // Instancie l'ennemi
            GameObject enemy = Instantiate(challenge.enemyPrefabs[i], spawnPosition, Quaternion.identity);
            spawnedEnemies.Add(enemy);
            totalEnemies++;

            // Assigne l'ID du défi à l'ennemi
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.challengeID = challengeID;
            }
            else
            {
                Debug.LogWarning($"ChallengeTrigger {challengeID}: L'ennemi spawné n'a pas de EnemyAI!");
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        // Enregistre le défi auprès du ChallengeManager
        if (ChallengeManager.instance != null && totalEnemies > 0)
        {
            ChallengeManager.instance.RegisterChallenge(
                challengeID, 
                totalEnemies, 
                challenge, 
                OnChallengeComplete
            );
        }
    }

    /// <summary>
    /// Appelé quand tous les ennemis du défi sont éliminés
    /// </summary>
    void OnChallengeComplete()
    {
        challengeCompleted = true;
        Debug.Log($"ChallengeTrigger {challengeID}: Défi complété!");
    }

    /// <summary>
    /// Appelé quand le menu de défi se ferme
    /// </summary>
    void OnChallengeMenuClosed()
    {
        // Réaffiche l'UI d'interaction si le joueur est encore dans la zone
        // et qu'aucun défi n'est actif
        if (isInRange && 
            ChallengeManager.instance != null && 
            !ChallengeManager.instance.IsChallengeActive(challengeID) &&
            interactUI != null)
        {
            interactUI.enabled = true;
        }
    }

    /// <summary>
    /// Appelé quand le joueur récupère la récompense
    /// </summary>
    void OnRewardClaimed()
    {
        rewardClaimed = true;
        Debug.Log($"ChallengeTrigger {challengeID}: Récompense réclamée!");
    }
}