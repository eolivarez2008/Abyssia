using UnityEngine;

public class DontDestroyCanvas : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject); // le Canvas restera actif entre toutes les scènes
    }
}
