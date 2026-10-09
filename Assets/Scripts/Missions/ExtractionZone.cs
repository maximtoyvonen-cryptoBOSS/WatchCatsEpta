using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ExtractionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Simple check for player. In real game, check tag or component.
        if (other.GetComponent<PlayerController>() != null)
        {
            if (MissionManager.Instance != null && MissionManager.Instance.IsExtractionUnlocked)
            {
                MissionManager.Instance.CompleteMission();
            }
            else
            {
                Debug.Log("Extraction Zone: Complete all mandatory objectives first.");
            }
        }
    }
}
