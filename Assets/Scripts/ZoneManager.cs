using UnityEngine;

public class ZoneManager : MonoBehaviour
{
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