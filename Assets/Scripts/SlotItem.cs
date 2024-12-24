using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSlotItem", menuName = "Wheel/SlotItem")]

public class SlotItem : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public int itemCount;
    public bool isBomb = false;
    public bool isGold = false;
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
        slotItem.name = WheelUI.NameFormatter(itemIcon);
        slotItem.itemIcon = itemIcon;
        slotItem.itemCount = Random.Range(1, 10);
        
        return slotItem;
    }
    
    public static SlotItem CreateInstance(Sprite itemIcon, int itemCount = -1)
    {
        var slotItem = ScriptableObject.CreateInstance<SlotItem>();
        slotItem.name = WheelUI.NameFormatter(itemIcon);
        slotItem.itemIcon = itemIcon;
        
        if (itemCount == -1)
            slotItem.itemCount = Random.Range(1, 10);
        
        slotItem.itemCount = itemCount;
        
        return slotItem;
    }
}