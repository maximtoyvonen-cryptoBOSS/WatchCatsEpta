using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class PhoneUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject phonePanel;
    [SerializeField] private Text objectivesText;
    [SerializeField] private Text energyText;
    [SerializeField] private Button restartContractButton;

    [Header("Terminal UI")]
    [SerializeField] private GameObject downloadProgressPanel;
    [SerializeField] private Text downloadProgressText;

    private IInputService inputService;
    private HackingEnergy energyManager;

    private void Awake()
    {
        // Ensure UI is hidden initially
        if (phonePanel != null)
        {
            phonePanel.SetActive(false);
        }

        if (restartContractButton != null)
        {
            restartContractButton.onClick.AddListener(RestartContract);
        }

        if (downloadProgressPanel != null)
        {
            downloadProgressPanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (inputService == null)
        {
            if (YandexGamesManager.Instance != null && YandexGamesManager.Instance.IsMobile())
            {
                var inputProvider = Object.FindObjectOfType<MobileInputProvider>();
                if (inputProvider != null) inputService = inputProvider.GetInputService();
            }

            if (inputService == null)
            {
                inputService = new DesktopInputService();
            }
        }

        energyManager = UnityEngine.Object.Object.FindObjectOfType<HackingEnergy>();
        if (energyManager != null)
        {
            energyManager.OnEnergyChanged += UpdateEnergyUI;
            UpdateEnergyUI(energyManager.CurrentEnergy, energyManager.MaxEnergy);
        }

        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionStarted += UpdateMissionUI;
            MissionManager.Instance.OnObjectiveUpdated += UpdateObjectiveUI;
            MissionManager.Instance.OnMissionCompleted += MissionEndedUI;
            MissionManager.Instance.OnMissionFailed += MissionEndedUI;
        }

        // Find all terminals to subscribe to progress.
        // In a real project, this might be handled via a dynamic registry or interaction event to avoid UnityEngine.Object.FindObjectsOfType
        DataTerminal[] terminals = UnityEngine.Object.FindObjectsOfType<DataTerminal>();
        foreach (var terminal in terminals)
        {
            terminal.OnDownloadProgress += HandleDownloadProgress;
        }
    }

    private void Update()
    {
        if (inputService.IsPhoneToggleDown())
        {
            TogglePhone();
        }
    }

    public void TogglePhone()
    {
        if (phonePanel != null)
        {
            bool isActive = !phonePanel.activeSelf;
            phonePanel.SetActive(isActive);

            // Optionally pause the game or unlock cursor here if needed
        }
    }

    private void UpdateEnergyUI(float current, float max)
    {
        if (energyText != null)
        {
            energyText.text = $"Energy: {Mathf.RoundToInt(current)} / {Mathf.RoundToInt(max)}";
        }
    }

    private void UpdateMissionUI(MissionData data)
    {
        RefreshObjectivesText();
    }

    private void UpdateObjectiveUI(ObjectiveData obj)
    {
        RefreshObjectivesText();
    }

    private void MissionEndedUI(MissionData data)
    {
        if (objectivesText != null)
        {
            objectivesText.text = $"Mission: {data.missionName}\nStatus: Ended";
        }
    }

    private void HandleDownloadProgress(float progress)
    {
        if (downloadProgressPanel != null && downloadProgressText != null)
        {
            if (progress > 0f && progress < 1f)
            {
                downloadProgressPanel.SetActive(true);
                downloadProgressText.text = $"Downloading: {Mathf.RoundToInt(progress * 100f)}%";
            }
            else
            {
                downloadProgressPanel.SetActive(false);
            }
        }
    }

    private void RefreshObjectivesText()
    {
        if (objectivesText == null || MissionManager.Instance == null) return;

        MissionData currentMission = MissionManager.Instance.CurrentMission;
        if (currentMission == null)
        {
            objectivesText.text = "No active contract.";
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Contract: {currentMission.missionName}");
        sb.AppendLine("--------------------");

        foreach (var obj in MissionManager.Instance.CurrentObjectives)
        {
            string status = obj.isCompleted ? "[X]" : "[ ]";
            string optionalMark = obj.isOptional ? "(Optional) " : "";
            sb.AppendLine($"{status} {optionalMark}{obj.description}");
        }

        if (MissionManager.Instance.IsExtractionUnlocked)
        {
            sb.AppendLine("\n[!] Proceed to Extraction Zone.");
        }

        objectivesText.text = sb.ToString();
    }

    private void RestartContract()
    {
        if (MissionManager.Instance != null && MissionManager.Instance.CurrentMission != null)
        {
            // Simple restart by re-starting the same template.
            // In a real game, you might reload the scene or reset game state.
            MissionData templateToRestart = MissionManager.Instance.OriginalTemplate;
            MissionManager.Instance.FailMission();
            MissionManager.Instance.StartMission(templateToRestart);
            TogglePhone(); // Close phone
        }
    }

    private void OnDestroy()
    {
        if (energyManager != null)
        {
            energyManager.OnEnergyChanged -= UpdateEnergyUI;
        }

        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionStarted -= UpdateMissionUI;
            MissionManager.Instance.OnObjectiveUpdated -= UpdateObjectiveUI;
            MissionManager.Instance.OnMissionCompleted -= MissionEndedUI;
            MissionManager.Instance.OnMissionFailed -= MissionEndedUI;
        }

        DataTerminal[] terminals = UnityEngine.Object.FindObjectsOfType<DataTerminal>();
        if (terminals != null)
        {
            foreach (var terminal in terminals)
            {
                if (terminal != null) terminal.OnDownloadProgress -= HandleDownloadProgress;
            }
        }
    }
}
