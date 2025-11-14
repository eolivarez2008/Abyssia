using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public string spawnPointName;
    public Vector3 offset;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        GameObject spawn = GameObject.Find(spawnPointName);
        if (spawn != null)
        {
            player.transform.position = spawn.transform.position + offset;
        }
    }
}
