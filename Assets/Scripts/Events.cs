using UnityEngine;

public static class Events
{
    public static event System.Action<float> ShakeCamera;

    public static void TriggerCameraShake(float magnitude)
    {
        ShakeCamera?.Invoke(magnitude);
    }
}
