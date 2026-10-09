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
        NoiseManager.MakeNoise(transform.position, noiseRadius);
        Debug.Log($"[Hack] Distributor emitting noise in {noiseRadius}m radius");
    }

    private void Detonate()
    {
        Debug.Log("[Hack] Distributor Detonating!");

        // Zero-allocation approach preferred for WebGL
        Collider[] hitColliders = new Collider[10];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, stunRadius, hitColliders, guardLayer);

        for (int i = 0; i < numColliders; i++)
        {
            IStunnable stunnable = hitColliders[i].GetComponent<IStunnable>();
            if (stunnable != null)
            {
                stunnable.Stun(stunDuration);
                Debug.Log($"[Hack] Stunned guard: {hitColliders[i].gameObject.name} for {stunDuration}s");
            }
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
