using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class WheelUI : MonoBehaviour
{
    public static WheelUI Instance { get; private set; }

    [SerializeField] private GameObject slotsParent;
    [SerializeField] private List<GameObject> slots;
    [SerializeField] public RectTransform wheel;
    [SerializeField] public TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] public Button giveUpButton;
    [SerializeField] public Button coinReviveButton;
    [SerializeField] public Button adReviveButton;
    [SerializeField] private Button spinButton;
    
    [SerializeField] public List<Sprite> availableIcons;
    [SerializeField] private List<GameObject> wheels;
    [SerializeField] private List<GameObject> indicators;
    [SerializeField] private List<GameObject> backgrounds;
    
    public int SlotCount => slots.Count;

    public List<GameObject> Slots => slots;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) DestroyImmediate(this);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Instance == null) Instance = this;
        
        availableIcons = new List<Sprite>(Resources.LoadAll<Sprite>("UI/Icons"));
    }
#endif

    private void Start()
    {
        if (slots == null || slots.Count == 0)
        {
            slots = new List<GameObject>();
            for (int i = 0; i < slotsParent.transform.childCount; i++)
            {
                slots.Add(slotsParent.transform.GetChild(i).gameObject);
            }
        }
    }

    public void UpdateGold(int gold)
    {
        goldText.text = gold.ToString();
    }

    public void UpdateUI(List<SlotItem> items)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i >= items.Count) continue;

            var item = items[i];
            var quantityRenderer = slots[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var iconRenderer = slots[i].transform.GetChild(1).GetComponent<Image>();

            quantityRenderer.text = $"{item.itemCount}x";
            iconRenderer.sprite = item.itemIcon;
            iconRenderer.SetNativeSize();

            float maxSize = 60f;
            float scaleFactor = maxSize / Mathf.Max(iconRenderer.rectTransform.sizeDelta.x, iconRenderer.rectTransform.sizeDelta.y);
            iconRenderer.rectTransform.sizeDelta *= scaleFactor;
        }
        
        HandleZoneUI(GameManager.Instance.GetCurrentZone());
        
    }

    public static string NameFormatter(Sprite icon)
    {
        string formattedName = icon.name; // example: ui_icon_aviator_glasses_easter
        formattedName = formattedName.ToLower();
        string[] prefixesToRemove = { "ui", "icons", "icon", "_t_", "renders", "render", "cons" }; 
        
        foreach (string prefix in prefixesToRemove)
            formattedName = formattedName.Replace(prefix, "");  //__aviator_glasses_easter
        
        formattedName = formattedName.TrimStart('_');  // aviator_glasses_easter
        formattedName = formattedName.Replace("_", " "); // aviator glasses easter

        return formattedName;
    }

    public void Rotate(float angle, float time)
    {
        wheel.DORotate(new Vector3(0, 0, -angle), time, RotateMode.FastBeyond360).SetEase(Ease.OutQuad);
    }

    public void ShowReward(SlotItem reward)
    {
        titleText.text = reward.itemName;
        descriptionText.text = $"You got: {reward.itemName} x{reward.itemCount}";
        deathPanel.SetActive(false);
        if (reward.isBomb)
        {
            StartCoroutine(RevealBomb());
        }
    }

    private IEnumerator RevealBomb()
    {
        yield return new WaitForSeconds(1f);
        titleText.text = "Bomb!";
        descriptionText.text = "You lost everything!";
        deathPanel.SetActive(true);
    }

    public void HandleZoneUI(int zone)
    {
        // if zone == 5, safe zone
        // if zone == 30, golden zone
        // else, normal zone

        if (zone == 5)
        {
            wheels[0].SetActive(false);
            indicators[0].SetActive(false);
            backgrounds[0].SetActive(false);
            wheels[1].SetActive(true);
            indicators[1].SetActive(true);
            backgrounds[1].SetActive(true);
        }
        else if (zone == 30)
        {
            wheels[0].SetActive(false);
            indicators[0].SetActive(false);
            backgrounds[0].SetActive(false);
            wheels[2].SetActive(true);
            indicators[2].SetActive(true);
            backgrounds[2].SetActive(true);
        }
        else
        {
            wheels[0].SetActive(true);
            indicators[0].SetActive(true);
            backgrounds[0].SetActive(true);
            wheels[1].SetActive(false);
            indicators[1].SetActive(false);
            backgrounds[1].SetActive(false);
            wheels[2].SetActive(false);
            indicators[2].SetActive(false);
            backgrounds[2].SetActive(false);
        }
    }

    public void HideDeathPanel()
    {
        deathPanel.SetActive(false);
    }

    public void AddButtonListeners(UnityEngine.Events.UnityAction giveUp, UnityEngine.Events.UnityAction coinRevive, UnityEngine.Events.UnityAction adRevive, UnityEngine.Events.UnityAction spin)
    {
        giveUpButton.onClick.AddListener(giveUp);
        coinReviveButton.onClick.AddListener(coinRevive);
        adReviveButton.onClick.AddListener(adRevive);
        spinButton.onClick.AddListener(spin);
    }
    
    public IEnumerator ChangeButtonColor(Button button, Color color, float duration)
    {
        Color originalColor = button.image.color;
        button.image.color = color;
        yield return new WaitForSeconds(duration);
        button.image.color = originalColor;
    }
}
