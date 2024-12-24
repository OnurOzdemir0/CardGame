using UnityEngine;

namespace Tools
{
    public class WheelEditor : MonoBehaviour
    {
        [Header("Slot Item Selection")]
        public SlotItem[] slotItems; 
        public SlotItem bombItem; 
        public SlotItem goldItem; 

        public string[] resourceIcons; 
        public int selectedIconIndex;

        private void Awake()
        {
            var sprites = Resources.LoadAll<Sprite>("UI/Icons");
            resourceIcons = new string[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                resourceIcons[i] = sprites[i].name;
            }
        }
    }
}