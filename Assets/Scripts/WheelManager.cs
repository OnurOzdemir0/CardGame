using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WheelManager : MonoBehaviour
{
    [Header("Wheel Configurations")]
    [SerializeField] private SlotItem goldSlot;
    [SerializeField] private List<SlotConfiguration> slotConfigurations = new List<SlotConfiguration>();

    [SerializeField] private List<SlotItem> rewards = new List<SlotItem>();
    private bool isSpinning;
    private Action<SlotItem> rewardCallback;
    private List<bool> bombStatuses;
    private Vector3 wheelStartRotation;

    public static WheelManager Instance { get; private set; }

    public bool IsSpinning => isSpinning;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            DestroyImmediate(gameObject);
        }
    }

    public void InitializeWheel(int zone, Action<SlotItem> rewardCallback)
    {
        this.rewardCallback = rewardCallback;
        GenerateWheelRewards(zone);
        AssignBombs(zone);
        WheelUI.Instance.UpdateUI(rewards);
        wheelStartRotation = WheelUI.Instance.wheel.rotation.eulerAngles;
    }
    
    private void AssignBombs(int zone)
    {
        bombStatuses = new List<bool>();
        float bombChance = Mathf.Min(zone * 0.025f, 0.7f);

        int safeIndex = Random.Range(0, rewards.Count);

        for (int i = 0; i < rewards.Count; i++)
        {
            if (i == safeIndex)
            {
                bombStatuses.Add(false);
            }
            else
            {
                bombStatuses.Add(0 < bombChance);
            }
            rewards[i].isBomb = true;
        }
    }

    public void SpinWheel()
    {
        if (isSpinning) return;
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        int slotCount = WheelUI.Instance.SlotCount;
        int targetIndex = Random.Range(0, slotCount);
        float slotAngle = -45f;
        float targetRotation = targetIndex * slotAngle + 360 * Random.Range(2, 5);
        int time = Random.Range(2, 4);
        WheelUI.Instance.Rotate(targetRotation,time);
        yield return new WaitForSeconds(time);
        StartCoroutine(RevealReward(targetIndex));
    }



    private IEnumerator RevealReward(int targetIndex)
    {
        var reward = rewards[targetIndex];
        WheelUI.Instance.ShowReward(reward);

        if (reward.isBomb)
        {
            StartCoroutine(AnimateBomb(targetIndex));
        }
        else
        {
            StartCoroutine(AnimateChosenItem(targetIndex));
        }

        yield return new WaitForSeconds(1f);

        if (bombStatuses[targetIndex])
        {
            reward.isBomb = true;
            rewardCallback?.Invoke(reward);
        }
        else
        {
            rewardCallback?.Invoke(reward);
            StartCoroutine(MoveGoldToCorner(targetIndex));
        }
    }

    private IEnumerator AnimateBomb(int index)
    {
        var slot = WheelUI.Instance.Slots[index];
        var iconRenderer = slot.transform.GetChild(1).GetComponent<Image>();
        Color originalColor = iconRenderer.color;

        for (int i = 0; i < 6; i++)
        {
            iconRenderer.color = i % 2 == 0 ? Color.red : originalColor;
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    private IEnumerator AnimateChosenItem(int index)
    {
        var slot = WheelUI.Instance.Slots[index];
        var iconRenderer = slot.transform.GetChild(1).GetComponent<Image>();
        Color originalColor = iconRenderer.color;
        Vector3 originalScale = slot.transform.localScale;

        for (int i = 0; i < 6; i++)
        {
            iconRenderer.color = i % 2 == 0 ? Color.green : originalColor;
            slot.transform.localScale = i % 2 == 0 ? originalScale * 1.2f : originalScale;
            yield return new WaitForSeconds(0.2f);
        }

        slot.transform.localScale = originalScale;
        iconRenderer.color = originalColor;
    }
    
    private IEnumerator MoveGoldToCorner(int index)
    {
        var slot = WheelUI.Instance.Slots[index];
        var goldIcon = slot.transform.GetChild(1).gameObject;
        RectTransform goldTransform = goldIcon.GetComponent<RectTransform>();
        RectTransform goldTextTransform = WheelUI.Instance.goldText.rectTransform;

        Vector3 startPosition = goldTransform.position;
        Vector3 endPosition = goldTextTransform.position;

        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            goldTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        goldTransform.position = startPosition;
        isSpinning = false;
        GenerateWheelRewards(GameManager.Instance.GetCurrentZone());
        WheelUI.Instance.UpdateUI(rewards);
    }
    
    private void GenerateWheelRewards(int zone)
    {
        rewards = GenerateRewards(8);
        AddSpecialRewards(zone);
    }

    private List<SlotItem> GenerateRewards(int count)
    {
        var generatedRewards = new List<SlotItem>();
        for (int i = 0; i < count; i++)
        {
            Sprite icon = WheelUI.Instance.availableIcons[Random.Range(0, WheelUI.Instance.availableIcons.Count)];
            int quantity = Mathf.Max(Mathf.CeilToInt(Random.Range(1, 20) * GameManager.Instance.GetCurrentZone() * 0.2f), 1);
            generatedRewards.Add(SlotItem.CreateInstance(icon, quantity));
        }
        return generatedRewards;
    }

    private void AddSpecialRewards(int zone)
    {
        if (zone % 5 != 0 && zone % 30 != 0 && Random.Range(0,1) == 1)
        {
            var bombIndex = Random.Range(0, rewards.Count - 1);
            rewards[bombIndex].isBomb = true;
        }
    }
}

public class SlotConfiguration
{
    public bool Randomize;
    public int Quantity;
    public Sprite Icon;

    public Sprite GetSelectedIcon(List<Sprite> availableIcons)
    {
        return availableIcons.Find(icon => icon.name == Icon.name);
    }
}
