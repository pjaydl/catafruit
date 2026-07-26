using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainButtons;

    [Header("Panels")]
    [SerializeField] private GameObject selectLevelPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        if (mainButtons != null)
        {
            mainButtons.SetActive(true);
        }

        if (selectLevelPanel != null)
        {
            selectLevelPanel.SetActive(false);
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void SelectLevel()
    {
        if (mainButtons != null)
        {
            mainButtons.SetActive(false);
        }

        if (selectLevelPanel != null)
        {
            selectLevelPanel.SetActive(true);
        }
    }

    public void Tutorial()
    {
        if (mainButtons != null)
        {
            mainButtons.SetActive(false);
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }
    }

    public void Settings()
    {
        if (mainButtons != null)
        {
            mainButtons.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }
}