using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRadius = 3.5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform interactionCenter;

    private IInputService inputService;
    private IInteractable currentInteractable;

    // Event for UI to subscribe to
    public event Action<IInteractable> OnInteractableChanged;

    private void Awake()
    {
        inputService = new DesktopInputService();
        if (interactionCenter == null)
        {
            interactionCenter = transform;
        }
    }

    private void Update()
    {
        FindInteractables();

        if (inputService.IsInteractDown() && currentInteractable != null)
        {
            if (currentInteractable.CanInteract())
            {
                currentInteractable.Interact(gameObject);
            }
        }
    }

    private void FindInteractables()
    {
        Collider[] colliders = Physics.OverlapSphere(interactionCenter.position, interactionRadius, interactableLayer);

        IInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;

        foreach (Collider collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract())
            {
                float distance = Vector3.Distance(interactionCenter.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        if (currentInteractable != closestInteractable)
        {
            currentInteractable = closestInteractable;
            OnInteractableChanged?.Invoke(currentInteractable);
        }
    }

    public void SetInputService(IInputService newService)
    {
        inputService = newService;
    }

    private void OnDrawGizmosSelected()
    {
        if (interactionCenter != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactionCenter.position, interactionRadius);
        }
    }
}
