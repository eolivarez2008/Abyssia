using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public string spawnPointName; // 🔹 ce champ doit être public

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        GameObject spawn = GameObject.Find(spawnPointName);
        if (spawn != null)
        {
            player.transform.position = spawn.transform.position;
        }
    }
}
