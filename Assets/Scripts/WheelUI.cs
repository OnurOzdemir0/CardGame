using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class WheelUI : MonoBehaviour
{
    [Header("UI References")] 
    [SerializeField] private GameObject slotsParent;
    [SerializeField] private List<GameObject> slots;
    [SerializeField] private RectTransform wheel;
    [SerializeField] private TextMeshProUGUI ui_gold_text;
    [SerializeField] private TextMeshProUGUI ui_card_text_title;
    [SerializeField] private TextMeshProUGUI ui_card_text_desc;
    [SerializeField] private GameObject ui_panel_death;
    [SerializeField] private Button GiveUp_btn;
    [SerializeField] private Button CoinRevive_btn;
    [SerializeField] private Button AdRevive_btn;
    [SerializeField] private Button Spin_btn;

    public void Start()
    {
        slots = new List<GameObject>();
        for (int i = 0; i < slotsParent.transform.childCount; i++)
        {
            slots.Add(slotsParent.transform.GetChild(i).gameObject);
        }
    }
    
    public int GetSlotCount()
    {
        return slots.Count;
    }

    public void UpdateGoldUI(int gold)
    {
        ui_gold_text.text = gold.ToString();
    }

    public void AddButtonListeners(System.Action onGiveUp, System.Action onCoinRevive, System.Action onAdRevive, System.Action onSpin)
    {
        GiveUp_btn.onClick.AddListener(() => onGiveUp());
        CoinRevive_btn.onClick.AddListener(() => onCoinRevive());
        AdRevive_btn.onClick.AddListener(() => onAdRevive());
        Spin_btn.onClick.AddListener(() => onSpin());
    }

    public void UpdateWheelUI(List<SlotItem> rewards)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (i < rewards.Count)
            {
                var reward = rewards[i];
                var textComponent = slot.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                var imageComponent = slot.transform.GetChild(1).GetComponent<Image>();

                textComponent.text = reward.itemCount + "x";
                imageComponent.sprite = reward.itemIcon;
                imageComponent.SetNativeSize();

                float maxSize = 60f;
                float scaleFactor = maxSize / Mathf.Max(imageComponent.rectTransform.sizeDelta.x, imageComponent.rectTransform.sizeDelta.y);
                imageComponent.rectTransform.sizeDelta = new Vector2(
                    imageComponent.rectTransform.sizeDelta.x * scaleFactor,
                    imageComponent.rectTransform.sizeDelta.y * scaleFactor
                );
            }
        }
    }

    public void RotateWheel(float totalRotation, float spinTime)
    {
        wheel.DORotate(new Vector3(0, 0, -totalRotation), spinTime, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
    }

    public void SetWheelRotation(float targetAngle)
    {
        wheel.localRotation = Quaternion.Euler(0, 0, -targetAngle);
    }

    public void ShowReward(SlotItem reward)
    {
        ui_card_text_title.text = reward.isBomb ? "Bomb!" : reward.itemName;
        ui_card_text_desc.text = reward.isBomb
            ? "You hit a bomb and lost all your rewards!"
            : $"You won: {reward.itemName} x{reward.itemCount}";
        ui_panel_death.SetActive(reward.isBomb);
    }

    public void HideDeathPanel()
    {
        ui_panel_death.SetActive(false);
    }
}
