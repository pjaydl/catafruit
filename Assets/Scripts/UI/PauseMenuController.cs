using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [Header("Pause Panel")]
    [SerializeField] private GameObject pausePanel;

    [Header("Scene Navigation")]
    [SerializeField]
    private SceneNavigationManager sceneNavigationManager;

    private bool isPaused;

    private void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "Pause Panel is not assigned."
            );
        }
    }

    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void ContinueGame()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (sceneNavigationManager != null)
        {
            sceneNavigationManager.RestartCurrentLevel();
        }
        else
        {
            Debug.LogError(
                "SceneNavigationManager is not assigned."
            );
        }
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (sceneNavigationManager != null)
        {
            sceneNavigationManager.LoadMainMenu();
        }
        else
        {
            Debug.LogError(
                "SceneNavigationManager is not assigned."
            );
        }
    }
}