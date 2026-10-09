using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] protected string interactionPrompt = "Interact";
    [SerializeField] protected bool canInteract = true;

    public virtual string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

    public abstract void Interact(GameObject instigator);

    public virtual bool CanInteract()
    {
        return canInteract;
    }
}
