using UnityEngine;

/// <summary>
/// ScriptableObject définissant les propriétés d'un item (potion, consommable, etc.)
/// </summary>
[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Informations de base")]
    [Tooltip("ID unique de l'item")]
    public int id;
    
    [Tooltip("Nom de l'item")]
    public string nameItem;
    
    [Tooltip("Prix de l'item dans les magasins")]
    public int price;
    
    [Tooltip("Sprite de l'item")]
    public Sprite image;

    [Header("Effet de soin")]
    [Tooltip("Points de vie restaurés")]
    public int hpGiven;

    [Header("Effet de vitesse")]
    [Tooltip("Bonus de vitesse")]
    public int speedGiven;
    
    [Tooltip("Durée de l'effet de vitesse en secondes")]
    public float speedDuration;

    [Header("Effet de dégâts")]
    [Tooltip("Bonus de dégâts")]
    public int damageGiven;
    
    [Tooltip("Durée de l'effet de dégâts en secondes")]
    public float damageDuration;

    [Header("Effet d'invincibilité")]
    [Tooltip("Donne l'invincibilité")]
    public bool givesInvincibility;
    
    [Tooltip("Durée de l'invincibilité en secondes")]
    public float invincibilityDuration;
}