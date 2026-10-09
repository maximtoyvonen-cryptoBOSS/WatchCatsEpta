using UnityEngine;
using System;

public static class NoiseManager
{
    public static event Action<Vector3, float> OnNoiseMade;

    public static void MakeNoise(Vector3 position, float radius)
    {
        OnNoiseMade?.Invoke(position, radius);
    }
}
