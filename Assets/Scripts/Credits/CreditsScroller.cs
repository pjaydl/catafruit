using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroller : MonoBehaviour
{
    [Header("Credits Object")]
    [SerializeField] private RectTransform creditsText;

    [Header("Movement Settings")]
    [SerializeField] private float scrollSpeed = 60f;
    [SerializeField] private float startY = -700f;
    [SerializeField] private float endY = 1200f;

    [Header("Ending Settings")]
    [SerializeField] private float delayAfterCredits = 2f;
    [SerializeField] private bool loopCredits = false;
    [SerializeField] private bool returnToMainMenu = true;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused;
    private bool creditsFinished;
    private float finishedTime;

    private void Start()
    {
        ResetCredits();
    }

    private void Update()
    {
        if (creditsText == null || isPaused)
        {
            return;
        }

        if (creditsFinished)
        {
            HandleCreditsEnding();
            return;
        }

        // Move the credits upward.
        Vector2 newPosition = creditsText.anchoredPosition;

        newPosition.y += scrollSpeed * Time.unscaledDeltaTime;

        creditsText.anchoredPosition = newPosition;

        // Check whether the credits reached the ending position.
        if (creditsText.anchoredPosition.y >= endY)
        {
            creditsFinished = true;
            finishedTime = Time.unscaledTime;
        }
    }

    private void HandleCreditsEnding()
    {
        if (Time.unscaledTime < finishedTime + delayAfterCredits)
        {
            return;
        }

        if (loopCredits)
        {
            ResetCredits();
        }
        else if (returnToMainMenu)
        {
            LoadMainMenu();
        }
    }

    public void ResetCredits()
    {
        if (creditsText == null)
        {
            Debug.LogError("Credits Text has not been assigned.");
            return;
        }

        Vector2 position = creditsText.anchoredPosition;
        position.y = startY;

        creditsText.anchoredPosition = position;

        creditsFinished = false;
        isPaused = false;
    }

    public void PauseCredits()
    {
        isPaused = true;
    }

    public void ResumeCredits()
    {
        isPaused = false;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
    }

    public void SkipCredits()
    {
        if (returnToMainMenu)
        {
            LoadMainMenu();
        }
    }

    private void LoadMainMenu()
    {
        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Debug.LogError("Main Menu scene name is empty.");
            return;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}