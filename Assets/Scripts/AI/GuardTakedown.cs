using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GuardTakedown : InteractableBase
{
    private GuardController guardController;

    private void Awake()
    {
        guardController = GetComponentInParent<GuardController>();
        if (guardController == null)
        {
            Debug.LogError("GuardTakedown needs to be a child of an object with GuardController!");
        }

        interactionPrompt = "Takedown";
    }

    public override bool CanInteract()
    {
        // Can only takedown if not in combat and not already stunned
        return base.CanInteract() &&
               guardController != null &&
               guardController.State != GuardState.Alert &&
               guardController.State != GuardState.Stunned;
    }

    public override void Interact(GameObject instigator)
    {
        if (CanInteract())
        {
            guardController.Takedown();
            canInteract = false; // Disable further interactions
        }
    }
}
