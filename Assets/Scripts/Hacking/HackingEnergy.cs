using UnityEngine;
using System;

public class HackingEnergy : MonoBehaviour
{
    [Header("Energy Settings")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float regenRatePerSecond = 5f;

    private float currentEnergy;

    public event Action<float, float> OnEnergyChanged;

    private void Awake()
    {
        currentEnergy = maxEnergy;
    }

    private void Update()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += regenRatePerSecond * Time.unscaledDeltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
        }
    }

    public bool HasEnoughEnergy(int cost)
    {
        return currentEnergy >= cost;
    }

    public bool ConsumeEnergy(int cost)
    {
        if (HasEnoughEnergy(cost))
        {
            currentEnergy -= cost;
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            return true;
        }
        return false;
    }

    public float CurrentEnergy => currentEnergy;
    public float MaxEnergy => maxEnergy;
}
