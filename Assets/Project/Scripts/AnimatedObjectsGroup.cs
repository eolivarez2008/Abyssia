using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gère un groupe d'objets animés et permet de les activer/désactiver simultanément
/// </summary>
public class AnimatedObjectsGroup : MonoBehaviour
{
    [Tooltip("Liste des GameObjects contenant des Animators à gérer")]
    public List<GameObject> groupObjects = new List<GameObject>();
    private List<Animator> animators = new List<Animator>();

    private void Start()
    {
        // Collecte tous les Animators des objets du groupe
        foreach (GameObject item in groupObjects)
        {
            if (item != null)
            {
                // Si l'objet a plusieurs enfants
                if (item.transform.childCount > 1)
                {
                    for (int i = 0; i < item.transform.childCount; i++)
                    {
                        Animator animator = item.transform.GetChild(i).GetComponentInChildren<Animator>();
                        if (animator != null)
                        {
                            animators.Add(animator);
                        }
                    }
                }
                else
                {
                    // Sinon récupère l'Animator de l'objet ou de ses enfants
                    Animator animator = item.GetComponentInChildren<Animator>();
                    if (animator != null)
                    {
                        animators.Add(animator);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Active ou désactive tous les Animators du groupe
    /// </summary>
    /// <param name="onOrOff">True pour activer, False pour désactiver</param>
    public void OnOffSwitch(bool onOrOff)
    {
        foreach (Animator item in animators)
        {
            if (item != null)
            {
                item.enabled = onOrOff;
            }
        }
    }
}