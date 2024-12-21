using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelManager : MonoBehaviour
{
    [Header("Game Configurations")]
    [SerializeField] private int initialGold = 100;
    [SerializeField] private List<Sprite> itemIcons;
    [SerializeField] private WheelUI wheelUi;

    private int currentZone = 1;
    private int currentGold;
    private bool isSpinning = false;
    private List<SlotItem> currentRewards = new List<SlotItem>();

    private void Start()
    {
        currentGold = initialGold;
        LoadItemIcons();
        wheelUi.UpdateGoldUI(currentGold);
        ResetWheel();
        wheelUi.AddButtonListeners(HandleGiveUp, HandleCoinRevive, HandleAdRevive, SpinWheel);
    }

    private void LoadItemIcons()
    {
        foreach (var itemIcon in Resources.LoadAll<Sprite>("UI/Icons"))
        {
            itemIcons.Add(itemIcon);
        }
    }

    private void ResetWheel()
    {
        currentRewards = GenerateRewardsForZone(wheelUi.GetSlotCount());
        wheelUi.UpdateWheelUI(currentRewards);
    }

    private List<SlotItem> GenerateRewardsForZone(int itemCount)
    {
        List<SlotItem> rewards = new List<SlotItem>();
        for (int i = 0; i < itemCount; i++)
        {
            Sprite randomIcon = itemIcons[Random.Range(0, itemIcons.Count)];
            SlotItem slotItem = SlotItem.CreateInstance(randomIcon);
            slotItem.itemCount += currentZone;
            rewards.Add(slotItem);
        }
        return rewards;
    }

    public void SpinWheel()
    {
        if (isSpinning) return;
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        int slotCount = wheelUi.GetSlotCount();
        float fullRotation = 360f;
        float slotAngle = fullRotation / slotCount;
        int targetIndex = Random.Range(0, currentRewards.Count);
        float targetAngle = slotAngle * targetIndex;
        int extraSpins = 3;
        float totalRotation = (extraSpins * fullRotation) + targetAngle;
        float spinTime = Random.Range(2f, 4f);

        wheelUi.RotateWheel(totalRotation, spinTime);

        yield return new WaitForSeconds(spinTime);

        wheelUi.SetWheelRotation(targetAngle);

        var result = currentRewards[targetIndex];
        HandleReward(result);

        isSpinning = false;
    }

    private void HandleReward(SlotItem result)
    {
        if (result.isBomb)
        {
            HandleBomb();
        }
        else
        {
            currentZone++;
            ResetWheel();
        }

        wheelUi.ShowReward(result);
    }

    private void HandleBomb()
    {
        currentZone = 1;
        ResetWheel();
    }

    private void HandleGiveUp()
    {
        Debug.Log("Player gave up. Returning to main menu...");
    }

    private void HandleCoinRevive()
    {
        if (currentGold >= 25)
        {
            currentGold -= 25;
            wheelUi.UpdateGoldUI(currentGold);
            wheelUi.HideDeathPanel();
            Debug.Log("Revived with coins!");
        }
        else
        {
            Debug.Log("Not enough coins to revive!");
        }
    }

    private void HandleAdRevive()
    {
        wheelUi.HideDeathPanel();
        Debug.Log("Revived by watching an ad!");
    }
}
