using UnityEngine;

public static class Events
{
    public static event System.Action<float> ShakeCamera;
    public static event System.Action<float> ZoomCamera;

    public static void TriggerCameraShake(float magnitude)
    {
        ShakeCamera?.Invoke(magnitude);
    }
    public static void AddCameraZoom(float amount)
    {
        ZoomCamera?.Invoke(amount);
    }
}
