using UnityEngine;
using System;

[RequireComponent(typeof(GuardController))]
public class FieldOfView : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float viewRadius = 12f;
    [SerializeField] [Range(0, 360)] private float viewAngle = 75f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Suspicion Settings")]
    [SerializeField] private float suspicionDrainRate = 10f; // Suspicion per second lost
    [SerializeField] private float runningSuspicionRate = 50f;
    [SerializeField] private float crouchingSuspicionRate = 15f;
    [SerializeField] private float walkingSuspicionRate = 30f;

    private GuardController guardController;
    private float currentSuspicion = 0f;
    private Transform playerTransform; // Cached reference
    private PlayerController cachedPlayerController;

    private Collider[] targetsInViewRadius = new Collider[1]; // Pre-allocated array for NonAlloc

    public event Action<float> OnSuspicionChanged;

    private void Awake()
    {
        guardController = GetComponent<GuardController>();
    }

    private void Update()
    {
        if (guardController.State == GuardState.Stunned) return;

        bool canSeePlayer = CheckFOV();

        if (canSeePlayer && playerTransform != null)
        {
            if (cachedPlayerController == null || cachedPlayerController.transform != playerTransform)
            {
                cachedPlayerController = playerTransform.GetComponent<PlayerController>();
            }

            float rate = walkingSuspicionRate;

            if (cachedPlayerController != null)
            {
                if (cachedPlayerController.IsSprinting) rate = runningSuspicionRate;
                else if (cachedPlayerController.IsCrouching) rate = crouchingSuspicionRate;
            }

            currentSuspicion += rate * Time.deltaTime;

            if (currentSuspicion >= 100f)
            {
                currentSuspicion = 100f;
                guardController.BecomeAlert(playerTransform.position);
            }
            else if (guardController.State != GuardState.Alert)
            {
                // Optionally investigate the last seen position if they get a glimpse
                if (currentSuspicion > 30f && guardController.State != GuardState.Suspicious)
                {
                     guardController.Investigate(playerTransform.position);
                }
            }
        }
        else
        {
            if (currentSuspicion > 0f)
            {
                currentSuspicion -= suspicionDrainRate * Time.deltaTime;
                currentSuspicion = Mathf.Max(0f, currentSuspicion);
            }
        }

        OnSuspicionChanged?.Invoke(currentSuspicion);

        // If already alert and see player, update path
        if (guardController.State == GuardState.Alert && canSeePlayer)
        {
            guardController.BecomeAlert(playerTransform.position);
        }
    }

    private bool CheckFOV()
    {
        int numTargets = Physics.OverlapSphereNonAlloc(transform.position, viewRadius, targetsInViewRadius, targetMask);

        if (numTargets > 0)
        {
            Transform target = targetsInViewRadius[0].transform;
            playerTransform = target;

            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2f)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
