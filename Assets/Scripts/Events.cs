using UnityEngine;
using static UpgradeManager;

public static class Events
{
    public static event System.Action<float> ShakeCamera;
    public static event System.Action<float> ZoomCamera;
    public static event System.Action<UpgradeTypes, float> TriggerUpgrade;
    public static event System.Action<float> TriggerPlayerHealth;

    public static void TriggerCameraShake(float magnitude)
    {
        ShakeCamera?.Invoke(magnitude);
    }
    public static void AddCameraZoom(float amount)
    {
        ZoomCamera?.Invoke(amount);
    }
    public static void IncreaseUpgrade(UpgradeTypes id, float amount)
    {
        TriggerUpgrade?.Invoke(id, amount);
    }
    public static void PlayerTakeDamage(float amount)
    {
        TriggerPlayerHealth?.Invoke(amount);
    }
}
