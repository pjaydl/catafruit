using System.Collections;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;

    [Header("UI")]
    [SerializeField] private TargetCounterUI targetCounterUI;

    [Header("Settings")]
    [SerializeField] private bool pauseGame = true;

    [Tooltip("Time given for the final skeleton's death animation.")]
    [SerializeField] private float victoryDelay = 2f;

    private bool gameEnded;
    private bool victoryPending;

    private Coroutine victoryRoutine;

    public void Initialize()
    {
        Time.timeScale = 1f;

        gameEnded = false;
        victoryPending = false;

        if (victoryRoutine != null)
        {
            StopCoroutine(victoryRoutine);
            victoryRoutine = null;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        UpdateTargetCount();
    }

    public void UpdateTargetCount()
    {
        SkeletonHealth[] skeletons =
            FindObjectsByType<SkeletonHealth>(
                FindObjectsSortMode.None
            );

        int remaining = 0;

        foreach (SkeletonHealth skeleton in skeletons)
        {
            if (skeleton != null && !skeleton.IsDead)
            {
                remaining++;
            }
        }

        if (targetCounterUI != null)
        {
            targetCounterUI.UpdateTargetCount(
                remaining
            );
        }
    }

    public void CheckVictory()
    {
        if (gameEnded || victoryPending)
            return;

        if (!AreEnemiesAlive())
        {
            BeginVictoryDelay();
        }
    }

    public void CheckGameOver()
    {
        if (gameEnded || victoryPending)
            return;

        if (AreEnemiesAlive())
        {
            ShowGameOver();
        }
        else
        {
            BeginVictoryDelay();
        }
    }

    private bool AreEnemiesAlive()
    {
        SkeletonHealth[] skeletons =
            FindObjectsByType<SkeletonHealth>(
                FindObjectsSortMode.None
            );

        foreach (SkeletonHealth skeleton in skeletons)
        {
            if (skeleton != null && !skeleton.IsDead)
            {
                return true;
            }
        }

        return false;
    }

    private void BeginVictoryDelay()
    {
        if (gameEnded || victoryPending)
            return;

        victoryPending = true;

        victoryRoutine =
            StartCoroutine(
                VictoryDelayRoutine()
            );
    }

    private IEnumerator VictoryDelayRoutine()
    {
        Debug.Log(
            $"Victory detected. Waiting {victoryDelay} seconds."
        );

        // Uses game time so the skeleton animation can finish normally.
        yield return new WaitForSeconds(
            Mathf.Max(0f, victoryDelay)
        );

        victoryRoutine = null;

        ShowVictory();
    }

    private void ShowVictory()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        victoryPending = false;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        PauseGame();

        Debug.Log("VICTORY!");
    }

    private void ShowGameOver()
    {
        if (gameEnded || victoryPending)
            return;

        gameEnded = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        PauseGame();

        Debug.Log("GAME OVER!");
    }

    private void PauseGame()
    {
        if (pauseGame)
        {
            Time.timeScale = 0f;
        }
    }
}