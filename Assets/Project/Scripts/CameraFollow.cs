using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float timeOffset = 0.2f;
    public Vector3 posOffset;

    private Vector3 velocity;

    void Awake()
    {
        if (player == null)
        {
            FindPlayer();
        }

        if (player != null)
        {
            transform.position = player.position + posOffset;
        }
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return; // évite erreur le temps de retrouver
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            player.position + posOffset,
            ref velocity,
            timeOffset
        );
    }

    private void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }
}
