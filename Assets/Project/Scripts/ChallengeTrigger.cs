using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChallengeTrigger : MonoBehaviour
{
    [Header("Challenge Configuration")]
    public ChallengeManager.Challenge challenge;

    [Header("Interaction")]
    public Text interactUI;
    public float spawnDelay = 0.5f;
    public float delayBeforeSpawn = 1f;

    [Header("Completed Message")]
    public string completedTitle = "Défi terminé";
    [TextArea(3, 5)]
    public string completedDescription = "Vous avez déjà complété ce défi et réclamé votre récompense.";

    private bool isInRange;
    private bool challengeCompleted = false;
    private bool rewardClaimed = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E) && !ChallengeManager.instance.IsChallengeActive())
        {
            TriggerChallenge();
        }

        if (ChallengeManager.instance != null && ChallengeManager.instance.challengeMenuOpen && Input.GetKeyDown(KeyCode.Tab))
        {
            ChallengeManager.instance.EndChallenge();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = true;
            
            if (!ChallengeManager.instance.challengeMenuOpen && !ChallengeManager.instance.IsChallengeActive())
                interactUI.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInRange = false;
            interactUI.enabled = false;
            
            if (ChallengeManager.instance.challengeMenuOpen)
                ChallengeManager.instance.EndChallenge();
        }
    }

    void TriggerChallenge()
    {
        interactUI.enabled = false;

        if (rewardClaimed)
        {
            ChallengeManager.instance.OpenChallengeCompleted(completedTitle, completedDescription, OnChallengeEnd);
            return;
        }

        if (challengeCompleted && !rewardClaimed)
        {
            ChallengeManager.instance.OpenChallengeReward(challenge, OnRewardClaimed, OnChallengeEnd);
        }
        else if (!challengeCompleted)
        {
            ChallengeManager.instance.OpenChallengeOffer(challenge, StartChallenge, OnChallengeEnd);
        }
    }

    void StartChallenge()
    {
        StartCoroutine(SpawnEnemiesWithDelay());
    }

    private System.Collections.IEnumerator SpawnEnemiesWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeSpawn);
        yield return StartCoroutine(SpawnEnemies());
    }

    private System.Collections.IEnumerator SpawnEnemies()
    {
        spawnedEnemies.Clear();
        int totalEnemies = 0;

        for (int i = 0; i < challenge.enemyPrefabs.Length; i++)
        {
            if (challenge.enemyPrefabs[i] == null) continue;

            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0
            );

            GameObject enemy = Instantiate(challenge.enemyPrefabs[i], spawnPosition, Quaternion.identity);
            spawnedEnemies.Add(enemy);
            totalEnemies++;

            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.challengeTrigger = this;
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        ChallengeManager.instance.StartChallenge(totalEnemies);
    }

    public void OnEnemyKilled(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
        ChallengeManager.instance.OnChallengeEnemyKilled();

        if (spawnedEnemies.Count == 0)
        {
            challengeCompleted = true;
        }
    }

    void OnChallengeEnd()
    {
        if (isInRange && !ChallengeManager.instance.IsChallengeActive())
            interactUI.enabled = true;
    }

    void OnRewardClaimed()
    {
        rewardClaimed = true;
    }
}