using UnityEngine;

public class LevelSettingsManager : MonoBehaviour
{
    [Header("Settings UI")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Pause Game")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    private bool settingsOpen;

    private void Start()
    {
        Time.timeScale = 1f;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        settingsOpen = false;
    }

    public void OpenSettings()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning(
                "Settings Panel is not assigned.",
                this
            );

            return;
        }

        settingsPanel.SetActive(true);
        settingsOpen = true;

        if (pauseGameWhenOpen)
        {
            Time.timeScale = 0f;
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel == null)
            return;

        settingsPanel.SetActive(false);
        settingsOpen = false;

        Time.timeScale = 1f;
    }

    private void OnDisable()
    {
        if (settingsOpen)
        {
            Time.timeScale = 1f;
            settingsOpen = false;
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}