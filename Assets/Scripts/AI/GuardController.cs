using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class GuardController : MonoBehaviour, IStunnable
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float alertSpeed = 4f;

    [Header("Suspicion Settings")]
    [SerializeField] private float inspectionTime = 4f;

    private NavMeshAgent agent;
    private GuardState currentState;
    private int currentPatrolIndex;

    private float stateTimer;
    private Vector3 investigationTarget;

    // Events for UI
    public event Action<GuardState> OnStateChanged;

    public GuardState State => currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // Initialize state vars manually for first pass to bypass the early out check
        currentState = GuardState.Patrol;
        agent.speed = patrolSpeed;
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[0].position);
        }
    }

    private void OnEnable()
    {
        NoiseManager.OnNoiseMade += HandleNoise;
    }

    private void OnDisable()
    {
        NoiseManager.OnNoiseMade -= HandleNoise;
    }

    private void Update()
    {
        if (currentState == GuardState.Stunned) return;

        switch (currentState)
        {
            case GuardState.Patrol:
                UpdatePatrol();
                break;
            case GuardState.Suspicious:
                UpdateSuspicious();
                break;
            case GuardState.Alert:
                UpdateAlert();
                break;
        }
    }

    private void ChangeState(GuardState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        stateTimer = 0f;

        switch (currentState)
        {
            case GuardState.Patrol:
                agent.speed = patrolSpeed;
                if (patrolPoints.Length > 0)
                {
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }
                break;
            case GuardState.Alert:
                agent.speed = alertSpeed;
                break;
        }

        OnStateChanged?.Invoke(currentState);
    }

    private void UpdatePatrol()
    {
        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    private void UpdateSuspicious()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Reached noise source, inspect for a while
            stateTimer += Time.deltaTime;
            if (stateTimer >= inspectionTime)
            {
                ChangeState(GuardState.Patrol);
            }
        }
    }

    private void UpdateAlert()
    {
        // Follow player logic would go here.
        // For now, if we lose sight, we might drop back to suspicious.
        // Assuming FieldOfView updates the destination when alert.
    }

    private void HandleNoise(Vector3 sourcePosition, float radius)
    {
        if (currentState == GuardState.Stunned || currentState == GuardState.Alert) return;

        float distance = Vector3.Distance(transform.position, sourcePosition);
        if (distance <= radius)
        {
            Investigate(sourcePosition);
        }
    }

    public void Investigate(Vector3 targetPosition)
    {
        if (currentState == GuardState.Stunned || currentState == GuardState.Alert) return;

        investigationTarget = targetPosition;
        agent.SetDestination(investigationTarget);

        // Ensure state timer resets if already suspicious
        if (currentState == GuardState.Suspicious)
        {
            stateTimer = 0f;
        }
        else
        {
            ChangeState(GuardState.Suspicious);
        }
    }

    public void BecomeAlert(Vector3 playerPosition)
    {
        if (currentState == GuardState.Stunned) return;

        agent.SetDestination(playerPosition);
        ChangeState(GuardState.Alert);
    }

    public void Stun(float duration)
    {
        if (currentState == GuardState.Stunned) return;

        ChangeState(GuardState.Stunned);
        agent.isStopped = true;

        // Reset state after duration
        Invoke(nameof(RecoverFromStun), duration);
    }

    private void RecoverFromStun()
    {
        agent.isStopped = false;
        ChangeState(GuardState.Patrol);
    }

    // Takedown logic from stealth
    public void Takedown()
    {
        if (currentState == GuardState.Alert) return; // Cannot takedown in combat

        CancelInvoke(nameof(RecoverFromStun));
        ChangeState(GuardState.Stunned);
        agent.isStopped = true;

        // Disable AI completely for perm takedown
        enabled = false;

        // Optional: Play takedown animation, disable collider, etc.
        Debug.Log("Guard taken down via stealth.");
    }
}
