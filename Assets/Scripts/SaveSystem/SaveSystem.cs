using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [DllImport("__Internal")] private static extern void YandexGames_SaveData(string jsonData);
    [DllImport("__Internal")] private static extern void YandexGames_LoadData();

    private const string LocalSaveKey = "WatchCats_SaveData";

    public SaveData CurrentData { get; private set; }

    public event Action OnDataLoadedEvent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CurrentData = new SaveData();
    }

    private void Start()
    {
        // Subscribe to SDK init to ensure we load cloud data only when ready
        if (YandexGamesManager.Instance != null)
        {
            YandexGamesManager.Instance.OnSdkInitialized += LoadData;
        }
        else
        {
            LoadData(); // Fallback if no manager
        }
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(CurrentData);

        #if UNITY_WEBGL && !UNITY_EDITOR
        YandexGames_SaveData(json);
        #else
        PlayerPrefs.SetString(LocalSaveKey, json);
        PlayerPrefs.Save();
        Debug.Log("Simulated Cloud Save (PlayerPrefs): " + json);
        #endif
    }

    public void LoadData()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        YandexGames_LoadData(); // This will trigger OnDataLoaded callback from JS
        #else
        string json = PlayerPrefs.GetString(LocalSaveKey, "{}");
        OnDataLoaded(json);
        #endif
    }

    // Called from JSLib
    public void OnDataLoaded(string json)
    {
        if (!string.IsNullOrEmpty(json) && json != "{}")
        {
            try
            {
                CurrentData = JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing save data: " + e.Message);
                CurrentData = new SaveData();
            }
        }
        else
        {
            CurrentData = new SaveData(); // Defaults
        }

        Debug.Log("Data Loaded!");
        OnDataLoadedEvent?.Invoke();
    }
}
