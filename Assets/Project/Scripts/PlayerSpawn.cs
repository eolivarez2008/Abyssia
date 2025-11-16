using UnityEngine;

/// <summary>
/// Positionne le joueur à un point de spawn spécifique au chargement de la scène
/// </summary>
public class PlayerSpawn : MonoBehaviour
{
    [Header("Point de spawn")]
    [Tooltip("Nom du GameObject servant de point de spawn")]
    public string spawnPointName;
    
    [Tooltip("Décalage par rapport au point de spawn")]
    public Vector3 offset;

    void Start()
    {
        // Trouve le joueur
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("PlayerSpawn: Aucun joueur trouvé avec le tag 'Player'");
            return;
        }

        // Trouve le point de spawn
        if (string.IsNullOrEmpty(spawnPointName))
        {
            Debug.LogWarning("PlayerSpawn: spawnPointName est vide!");
            return;
        }

        GameObject spawn = GameObject.Find(spawnPointName);
        if (spawn != null)
        {
            // Positionne le joueur au point de spawn + offset
            player.transform.position = spawn.transform.position + offset;
            Debug.Log($"PlayerSpawn: Joueur positionné à {spawnPointName}");
        }
        else
        {
            Debug.LogWarning($"PlayerSpawn: Point de spawn '{spawnPointName}' introuvable!");
        }
    }
}