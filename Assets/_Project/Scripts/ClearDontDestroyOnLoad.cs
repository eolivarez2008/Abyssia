using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearDontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        // Crée un objet temporaire dans la zone persistante
        GameObject temp = new GameObject("TempDDOL");
        DontDestroyOnLoad(temp);

        // Récupère la vraie "scène" interne DontDestroyOnLoad
        Scene ddolScene = temp.scene;

        // Supprime tout sauf l'objet temporaire
        foreach (GameObject go in ddolScene.GetRootGameObjects())
        {
            if (go != temp)
                Destroy(go);
        }

        // Détruit aussi l'objet temporaire
        Destroy(temp);
    }
}
