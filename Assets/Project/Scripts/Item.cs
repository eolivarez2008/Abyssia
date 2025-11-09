using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int id;
    public string nameItem;
    public string description;
    public int price;
    public Sprite image;

    public int hpGiven;

    // Boost de vitesse
    public int speedGiven;
    public float speedDuration;

    // Boost de dégâts
    public int damageGiven;
    public float damageDuration;
}
