using UnityEngine;

public class AutoEnableMobileUI : MonoBehaviour
{
    [SerializeField] private GameObject mobileUIPanel;

    private void Start()
    {
        if (YandexGamesManager.Instance != null && YandexGamesManager.Instance.IsMobile())
        {
            if (mobileUIPanel != null)
            {
                mobileUIPanel.SetActive(true);
            }
        }
        else
        {
            if (mobileUIPanel != null)
            {
                mobileUIPanel.SetActive(false);
            }
        }
    }
}
