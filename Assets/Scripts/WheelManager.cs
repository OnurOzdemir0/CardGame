using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelManager : MonoBehaviour
{
    [Header("Game Configurations")]
    [SerializeField] private int initialGold = 100;
    [SerializeField] private List<Sprite> itemIcons;
    [SerializeField] private SlotItem bombItem;
    [SerializeField] private SlotItem goldItem;
    [SerializeField] private WheelUI wheelUi;

    private int _currentZone = 1;
    private int _currentGold;
    private bool _isSpinning;
    [SerializeField] private List<SlotItem> currentRewards = new List<SlotItem>();

    private void Start()
    {
        _isSpinning = false;
        _currentGold = initialGold;
        LoadItemIcons();
        wheelUi.UpdateGoldUI(_currentGold);
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
        wheelUi.HandleZoneImage(_currentZone);
        currentRewards = GenerateRewardsForZone(8);

        currentRewards.RemoveAt(Random.Range(0, currentRewards.Count));
        goldItem.itemCount = Mathf.CeilToInt((_currentZone * 1.3f) * Random.Range(1, 5));
        currentRewards.Add(goldItem);
        if (_currentZone is not 5 and not 30)
        {
            currentRewards.RemoveAt(Random.Range(0, currentRewards.Count));
            currentRewards.Add(bombItem);
        }
        
        wheelUi.UpdateWheelUI(currentRewards);
    }

    private List<SlotItem> GenerateRewardsForZone(int itemCount)
    {
        List<SlotItem> rewards = new List<SlotItem>();
        for (int i = 0; i < itemCount; i++)
        {
            Sprite randomIcon = itemIcons[Random.Range(0, itemIcons.Count)];
            SlotItem slotItem = SlotItem.CreateInstance(randomIcon);
            slotItem.itemCount += _currentZone;
            rewards.Add(slotItem);
        }
        return rewards;
    }

    private void SpinWheel()
    {
        if (_isSpinning) return;
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        _isSpinning = true;
        int slotCount = wheelUi.GetSlotCount();
        float fullRotation = 360f;
        float slotAngle = fullRotation / slotCount;
        int extraSpins = 3;

        int targetIndex = Random.Range(0, slotCount);
        float targetAngle = slotAngle * targetIndex;
        float totalRotation = (extraSpins * fullRotation) + targetAngle;
        float spinTime = Random.Range(2f, 4f);
        wheelUi.RotateWheel(totalRotation, spinTime);

        yield return new WaitForSeconds(spinTime);
        float normalizedAngle = (totalRotation % fullRotation) + (slotAngle / 2);
        int selectedIndex = Mathf.FloorToInt(normalizedAngle / slotAngle) % slotCount;

        selectedIndex = (slotCount - selectedIndex) % slotCount;
        SlotItem result = currentRewards[selectedIndex];
        HandleReward(result);
        wheelUi.SetWheelRotation(slotAngle * selectedIndex);
        _isSpinning = false;
    }


    private void HandleReward(SlotItem result)
    {
        
        if (result.isBomb)
        {
            HandleBomb();
        }
        else
        {
            _currentZone++;
            
            if (result.isGold)
            {
                _currentGold += result.itemCount;
                wheelUi.UpdateGoldUI(_currentGold);
            }
            ResetWheel();
        }

        wheelUi.ShowReward(result);
    }

    private void HandleBomb()
    {
        _currentZone = 1;
        ResetWheel();
    }

    private void HandleGiveUp()
    {
        Debug.Log("Player gave up. Returning to main menu...");
    }

    private void HandleCoinRevive()
    {
        if (_currentGold >= 25)
        {
            _currentGold -= 25;
            wheelUi.UpdateGoldUI(_currentGold);
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
