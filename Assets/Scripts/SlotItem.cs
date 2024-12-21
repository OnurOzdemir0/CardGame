using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSlotItem", menuName = "Wheel/SlotItem")]

public class SlotItem : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public int itemCount;
    public bool isBomb;
    public bool isGold;
    public int zoneMultiplier;

    public static SlotItem CreateInstance(string itemName, Sprite itemIcon, int itemCount)
    {
        var slotItem = ScriptableObject.CreateInstance<SlotItem>();
        slotItem.itemName = itemName;
        slotItem.itemIcon = itemIcon;
        slotItem.itemCount = itemCount;
        return slotItem;
    }
    
    public static SlotItem CreateInstance()
    {
        var slotItem = ScriptableObject.CreateInstance<SlotItem>();
        slotItem.itemName = "itemName";
        slotItem.itemIcon = Resources.Load<Sprite>("image");
        
        slotItem.itemCount = 1;
        return slotItem;
    }

    public static SlotItem CreateInstance(Sprite itemIcon)
    {
        var slotItem = ScriptableObject.CreateInstance<SlotItem>();
        string name = itemIcon.name;
        name = name.ToLower();
        string[] prefixesToRemove = { "ui", "icons", "icon", "_t_", "renders", "render", "cons" };

        foreach (string prefix in prefixesToRemove)
            name = name.Replace(prefix, "");
        
        name = name.TrimStart('_');
        name = name.Replace("_", " ");
        
        slotItem.itemName = name;
        slotItem.itemIcon = itemIcon;
        slotItem.itemCount = Random.Range(1, 10);
        
        return slotItem;
    }
}