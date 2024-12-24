using UnityEngine;

namespace Tools
{
    public class WheelEditor : MonoBehaviour
    {
        [Header("Slot Item Selection")]
        public SlotItem[] slotItems; // Array for slot items
        public SlotItem bombItem; // Predefined bomb item
        public SlotItem goldItem; // Predefined gold item

        public string[] resourceIcons; // List of icon names from the Resources folder
        public int selectedIconIndex; // Index of selected icon for custom items

        private void Awake()
        {
            // Load available icons from Resources folder
            var sprites = Resources.LoadAll<Sprite>("UI/Icons");
            resourceIcons = new string[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                resourceIcons[i] = sprites[i].name;
            }
        }
    }
}