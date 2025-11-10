using UnityEngine;

/// <summary>
/// Fait suivre la caméra au joueur avec un mouvement lissé
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Cible")]
    [Tooltip("Transform du joueur à suivre")]
    public Transform player;
    
    [Header("Paramètres de suivi")]
    [Tooltip("Temps de lissage du mouvement (plus petit = plus rapide)")]
    public float timeOffset = 0.2f;
    
    [Tooltip("Décalage de position par rapport au joueur")]
    public Vector3 posOffset;
    
    [Header("Recherche automatique")]
    [Tooltip("Délai entre chaque tentative de recherche du joueur (en secondes)")]
    public float searchDelay = 1f;

    private Vector3 velocity;
    private float lastSearchTime;

    void Awake()
    {
        // Recherche du joueur au démarrage
        if (player == null)
        {
            FindPlayer();
        }

        // Positionne la caméra immédiatement sur le joueur
        if (player != null)
        {
            transform.position = player.position + posOffset;
        }
    }

    void Update()
    {
        // Si le joueur n'est pas trouvé, tenter de le rechercher
        if (player == null)
        {
            // Utilise un délai pour éviter de chercher à chaque frame
            if (Time.time - lastSearchTime >= searchDelay)
            {
                FindPlayer();
                lastSearchTime = Time.time;
            }
            return;
        }

        // Suit le joueur avec un mouvement lissé
        transform.position = Vector3.SmoothDamp(
            transform.position,
            player.position + posOffset,
            ref velocity,
            timeOffset
        );
    }

    /// <summary>
    /// Recherche le GameObject avec le tag "Player" dans la scène
    /// </summary>
    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            Debug.Log("CameraFollow: Joueur trouvé");
        }
    }

    /// <summary>
    /// Permet de définir manuellement le joueur à suivre
    /// </summary>
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
        if (player != null)
        {
            transform.position = player.position + posOffset;
        }
    }
}