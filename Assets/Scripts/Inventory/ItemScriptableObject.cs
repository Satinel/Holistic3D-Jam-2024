using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Item", order = 0)]
public class ItemScriptableObject : ScriptableObject
{
    public string ItemName;
    public Sprite ItemSprite;
    public int BaseValue;
    public Origin Origin;
    public ItemType ItemType;
    
}
