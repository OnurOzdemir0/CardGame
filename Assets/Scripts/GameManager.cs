using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Game Configurations")]
    [SerializeField] private int startingGold = 100;

    [SerializeField] private int currentGold;
    [SerializeField] private int currentZone;
    
    public int GetCurrentZone() => currentZone;
    public int GetCurrentGold() => currentGold;
    
    // how to 

    public static GameManager Instance { get; private set; }

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

    private void Start()
    {
        currentGold = startingGold;
        UpdateUI();
        WheelManager.Instance.InitializeWheel(currentZone, HandleReward);
        WheelUI.Instance.AddButtonListeners(OnGiveUpClicked, OnGoldReviveClicked,OnAdReviveClicked,OnSpinClicked);
    }

    public void OnSpinClicked()
    {
        if (WheelManager.Instance.IsSpinning) return;

        WheelManager.Instance.SpinWheel();
        currentZone++;
    }

    private void HandleReward(SlotItem reward)
    {
        if (reward.isBomb)
        {
            Debug.Log("Bomb hit! Resetting gold.");
            currentGold = 0;
            RestartWheel();
        }
        else
        {
            currentGold += reward.itemCount;
            // Debug.Log($"Reward collected: {reward.itemName}, Total Gold: {currentGold}");
        }

        UpdateUI();
    }

    private void RestartWheel()
    {
        WheelManager.Instance.InitializeWheel(currentZone, HandleReward);
    }

    public void OnGiveUpClicked()
    {
        Debug.Log($"Player left the game with {currentGold} gold.");
        currentGold = 0;
        currentZone = 1;
        UpdateUI();
    }

    public void OnGoldReviveClicked()
    {
        Debug.Log("Revive logic triggered.");
        if (currentGold < 25)
        {
            StartCoroutine(WheelUI.Instance.ChangeButtonColor(WheelUI.Instance.coinReviveButton, Color.red, 0.5f));
            return;
        };
        currentGold -= 25;
        RestartWheel();
    }
    
    public void OnAdReviveClicked()
    {
        Debug.Log("Ad revive logic triggered.");
        RestartWheel();
    }

    private void UpdateUI()
    {
        WheelUI.Instance.UpdateGold(currentGold);
        WheelUI.Instance.HandleZoneUI(currentZone);
    }
}