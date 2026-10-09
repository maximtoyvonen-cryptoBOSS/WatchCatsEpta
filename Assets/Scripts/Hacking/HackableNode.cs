using UnityEngine;

public abstract class HackableNode : InteractableBase
{
    [Header("Hacking Properties")]
    [SerializeField] protected HackType hackType;
    [SerializeField] protected int energyCost = 10;

    public HackType Type => hackType;
    public int Cost => energyCost;

    // Optional override of Interact if we want physical interaction to also trigger hack
    public override void Interact(GameObject instigator)
    {
        // By default, trying to physically interact might attempt a hack
        var manager = instigator.GetComponent<HackManager>();
        if (manager != null)
        {
            manager.AttemptHack(this);
        }
    }

    public abstract void ExecuteHack(HackManager manager);
}
