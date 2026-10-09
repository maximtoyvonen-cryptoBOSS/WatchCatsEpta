using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class YandexGamesManager : MonoBehaviour
{
    public static YandexGamesManager Instance { get; private set; }

    [DllImport("__Internal")] private static extern void YandexGames_Init();
    [DllImport("__Internal")] private static extern void YandexGames_GameplayStart();
    [DllImport("__Internal")] private static extern void YandexGames_GameplayStop();
    [DllImport("__Internal")] private static extern void YandexGames_ShowFullscreenAdv();
    [DllImport("__Internal")] private static extern void YandexGames_ShowRewardedVideo();
    [DllImport("__Internal")] private static extern int YandexGames_GetDeviceType();

    public event Action OnSdkInitialized;
    private bool isInitialized = false;

    private Action interstitialCloseCallback;
    private Action rewardedSuccessCallback;
    private Action rewardedCloseCallback;

    private float previousTimeScale;
    private bool previousAudioPause;
    private int pauseDepth = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        YandexGames_Init();
        #else
        isInitialized = true;
        // Delaying initialization invocation to avoid race conditions with other Start methods
        Invoke(nameof(OnYandexSdkInitialized), 0.1f);
        #endif
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // Window blur/focus handling (Yandex requirements)
        if (!hasFocus)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    // Called from JSLib
    public void OnYandexSdkInitialized()
    {
        isInitialized = true;
        OnSdkInitialized?.Invoke();
    }

    public void GameplayStart()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        if (isInitialized) YandexGames_GameplayStart();
        #else
        Debug.Log("Yandex Gameplay Start");
        #endif
    }

    public void GameplayStop()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        if (isInitialized) YandexGames_GameplayStop();
        #else
        Debug.Log("Yandex Gameplay Stop");
        #endif
    }

    public void ShowInterstitialAd(Action onClosed = null)
    {
        interstitialCloseCallback = onClosed;
        PauseGame();

        #if UNITY_WEBGL && !UNITY_EDITOR
        YandexGames_ShowFullscreenAdv();
        #else
        Debug.Log("Simulated Interstitial Ad Shown");
        OnInterstitialClosed();
        #endif
    }

    public void ShowRewardedAd(Action onRewarded, Action onClosed = null)
    {
        rewardedSuccessCallback = onRewarded;
        rewardedCloseCallback = onClosed;
        PauseGame();

        #if UNITY_WEBGL && !UNITY_EDITOR
        YandexGames_ShowRewardedVideo();
        #else
        Debug.Log("Simulated Rewarded Ad Shown & Rewarded");
        OnRewarded();
        OnRewardedClosed();
        #endif
    }

    // Callbacks from JSLib
    public void OnInterstitialClosed()
    {
        ResumeGame();
        interstitialCloseCallback?.Invoke();
        interstitialCloseCallback = null;
    }

    public void OnRewarded()
    {
        rewardedSuccessCallback?.Invoke();
        rewardedSuccessCallback = null;
    }

    public void OnRewardedClosed()
    {
        ResumeGame();
        rewardedCloseCallback?.Invoke();
        rewardedCloseCallback = null;
    }

    public bool IsMobile()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        return YandexGames_GetDeviceType() == 1;
        #else
        // In editor, test by toggling a mock or check SystemInfo
        return SystemInfo.deviceType == DeviceType.Handheld;
        #endif
    }

    private void PauseGame()
    {
        if (pauseDepth == 0)
        {
            // Yandex moderation requires audio to be muted and time paused on blur/ads
            if (Time.timeScale > 0)
            {
                previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }

            previousAudioPause = AudioListener.pause;
            AudioListener.pause = true;
        }
        pauseDepth++;
    }

    private void ResumeGame()
    {
        pauseDepth--;
        if (pauseDepth <= 0)
        {
            pauseDepth = 0;
            Time.timeScale = previousTimeScale > 0 ? previousTimeScale : 1f;
            AudioListener.pause = previousAudioPause;
        }
    }
}
