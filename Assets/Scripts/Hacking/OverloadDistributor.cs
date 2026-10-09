using UnityEngine;
using System.Collections;

public class OverloadDistributor : HackableNode
{
    [Header("Overload Settings")]
    [SerializeField] private float detonationTime = 1.5f;
    [SerializeField] private float stunRadius = 6f;
    [SerializeField] private float stunDuration = 6f;
    [SerializeField] private float noiseRadius = 15f;
    [SerializeField] private LayerMask guardLayer;

    private void Awake()
    {
        hackType = HackType.PowerDistributor;
    }

    public override void ExecuteHack(HackManager manager)
    {
        canInteract = false; // Prevent multiple hacks
        StartCoroutine(OverloadRoutine());
    }

    private IEnumerator OverloadRoutine()
    {
        EmitNoise();

        // Visual/Audio cue for countdown could go here
        yield return new WaitForSeconds(detonationTime);

        Detonate();
    }

    private void EmitNoise()
    {
        // Logic to alert AI within noiseRadius.
        // In a full implementation, this would interact with an AI perception system.
        // E.g., GameManager.Instance.AlertEnemies(transform.position, noiseRadius);
        Debug.Log($"[Hack] Distributor emitting noise in {noiseRadius}m radius");
    }

    private void Detonate()
    {
        Debug.Log("[Hack] Distributor Detonating!");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, stunRadius, guardLayer);
        foreach (var hitCollider in hitColliders)
        {
            // Assuming guards have some sort of IStunnable interface or GuardComponent
            // IStunnable stunnable = hitCollider.GetComponent<IStunnable>();
            // stunnable?.Stun(stunDuration);

            Debug.Log($"[Hack] Stunned guard: {hitCollider.gameObject.name} for {stunDuration}s");
        }

        // Disable visuals/components of the distributor if destroyed
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, noiseRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stunRadius);
    }
}
