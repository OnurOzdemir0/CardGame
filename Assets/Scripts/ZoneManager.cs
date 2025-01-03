using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }

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

    public enum ZoneType { Normal, Safe, Super }

    public ZoneType GetZoneType(int zone)
    {
        if (zone % 30 == 0) return ZoneType.Super;
        if (zone % 5 == 0) return ZoneType.Safe;
        return ZoneType.Normal;
    }

    public bool IsBombAllowed(int zone)
    {
        return GetZoneType(zone) == ZoneType.Normal;
    }
}