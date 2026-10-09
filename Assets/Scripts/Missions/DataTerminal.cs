using UnityEngine;
using System;

public class DataTerminal : HackableNode
{
    [Header("Terminal Settings")]
    [SerializeField] private string terminalID = "Terminal_01";
    [SerializeField] private float downloadTime = 5f;

    private float currentProgress = 0f;
    private bool isDownloading = false;
    private bool isDownloaded = false;

    public event Action<float> OnDownloadProgress;

    private void Awake()
    {
        hackType = HackType.Terminal;
    }

    // Update is no longer used for download progress. Controlled by caller.

    public override void ExecuteHack(HackManager manager)
    {
        // Terminal hacking through scanner might not be supported if it requires holding interaction.
        // We can route this to the same start interaction.
        if (!isDownloaded)
        {
            isDownloading = true;
            // Provide UI feedback or start visual effects
        }
    }

    public override void Interact(GameObject instigator)
    {
        if (!isDownloaded)
        {
            isDownloading = true;
        }
    }

    public void ContinueDownload()
    {
        if (isDownloading && !isDownloaded)
        {
            currentProgress += Time.deltaTime;
            OnDownloadProgress?.Invoke(currentProgress / downloadTime);

            if (currentProgress >= downloadTime)
            {
                CompleteDownload();
            }
        }
    }

    public void StopDownload()
    {
        isDownloading = false;
        // Optionally lose progress or keep it
    }

    private void CompleteDownload()
    {
        isDownloading = false;
        isDownloaded = true;
        canInteract = false;

        Debug.Log($"Data Downloaded from {terminalID}");

        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.UpdateObjective(terminalID);
        }
    }

    // A continuous interact check could be added if tied to the player's interaction system directly.
}
