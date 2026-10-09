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

    private Collider[] overlapResults = new Collider[10];

    // Event for UI to subscribe to
    public event Action<IInteractable> OnInteractableChanged;

    private void Awake()
    {
        if (interactionCenter == null)
        {
            interactionCenter = transform;
        }
    }

    private void Start()
    {
        if (inputService == null)
        {
            if (YandexGamesManager.Instance != null && YandexGamesManager.Instance.IsMobile())
            {
                var inputProvider = UnityEngine.Object.FindAnyObjectByType<MobileInputProvider>();
                if (inputProvider != null) inputService = inputProvider.GetInputService();
            }

            if (inputService == null)
            {
                inputService = new DesktopInputService();
            }
        }
    }

    private void Update()
    {
        FindInteractables();

        if (currentInteractable != null)
        {
            if (inputService.IsInteractDown() && currentInteractable.CanInteract())
            {
                currentInteractable.Interact(gameObject);
            }

            // Continuous interaction handling
            if (currentInteractable is DataTerminal terminal)
            {
                if (inputService.IsInteractHeld())
                {
                    terminal.ContinueDownload();
                }
                else if (inputService.IsInteractUp() || !terminal.CanInteract())
                {
                    terminal.StopDownload();
                }
            }
        }
    }

    private void FindInteractables()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(interactionCenter.position, interactionRadius, overlapResults, interactableLayer);

        IInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < numColliders; i++)
        {
            Collider collider = overlapResults[i];
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
