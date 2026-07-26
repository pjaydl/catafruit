using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigationManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Enter the exact name of your main menu scene.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Tooltip(
        "When the player finishes the final level, " +
        "load the main menu instead."
    )]
    [SerializeField]
    private bool returnToMainMenuAfterFinalLevel = true;

    [Header("Loading Settings")]
    [SerializeField] private bool useAsyncLoading = true;

    [Tooltip(
        "Prevents rapid button presses from loading " +
        "multiple scenes."
    )]
    [SerializeField] private bool preventMultipleLoads = true;

    private bool isLoading;

    /// <summary>
    /// Loads a scene using its exact scene name.
    /// Useful for Main Menu Play and Level Select buttons.
    /// </summary>
    public void LoadSceneByName(string sceneName)
    {
        if (!CanBeginLoading())
            return;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                "Cannot load the scene because the scene name is empty."
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"Scene '{sceneName}' cannot be loaded. " +
                "Check its spelling and make sure it is included " +
                "in the Build Profile scene list."
            );

            return;
        }

        BeginSceneLoad(sceneName);
    }

    /// <summary>
    /// Loads a scene using its Build Profile index.
    /// Useful for level-selection buttons.
    /// </summary>
    public void LoadSceneByBuildIndex(int buildIndex)
    {
        if (!CanBeginLoading())
            return;

        int sceneCount =
            SceneManager.sceneCountInBuildSettings;

        if (buildIndex < 0 || buildIndex >= sceneCount)
        {
            Debug.LogError(
                $"Build index {buildIndex} is invalid. " +
                $"The current Build Profile contains " +
                $"{sceneCount} scenes."
            );

            return;
        }

        BeginSceneLoad(buildIndex);
    }

    /// <summary>
    /// Reloads the currently active scene.
    /// Use this for Restart buttons.
    /// </summary>
    public void RestartCurrentLevel()
    {
        if (!CanBeginLoading())
            return;

        int currentBuildIndex =
            SceneManager.GetActiveScene().buildIndex;

        BeginSceneLoad(currentBuildIndex);
    }

    /// <summary>
    /// Loads the next scene according to the
    /// Build Profile scene order.
    /// </summary>
    public void LoadNextLevel()
    {
        if (!CanBeginLoading())
            return;

        int currentBuildIndex =
            SceneManager.GetActiveScene().buildIndex;

        int nextBuildIndex =
            currentBuildIndex + 1;

        int sceneCount =
            SceneManager.sceneCountInBuildSettings;

        if (nextBuildIndex < sceneCount)
        {
            BeginSceneLoad(nextBuildIndex);
            return;
        }

        if (returnToMainMenuAfterFinalLevel)
        {
            LoadMainMenu();
            return;
        }

        Debug.LogWarning(
            "There is no next level in the Build Profile."
        );
    }

    /// <summary>
    /// Loads the scene assigned as the main menu.
    /// </summary>
    public void LoadMainMenu()
    {
        if (!CanBeginLoading())
            return;

        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Debug.LogError(
                "Main Menu Scene Name is empty."
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(
                mainMenuSceneName))
        {
            Debug.LogError(
                $"Main menu scene '{mainMenuSceneName}' " +
                "cannot be loaded. Check its spelling and " +
                "Build Profile inclusion."
            );

            return;
        }

        BeginSceneLoad(mainMenuSceneName);
    }

    /// <summary>
    /// Pauses gameplay.
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Continues gameplay.
    /// Use this for the Continue button.
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Closes the built game.
    /// Also stops Play Mode when testing in the Editor.
    /// </summary>
    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private bool CanBeginLoading()
    {
        if (preventMultipleLoads && isLoading)
        {
            Debug.LogWarning(
                "A scene is already being loaded."
            );

            return false;
        }

        return true;
    }

    private void BeginSceneLoad(string sceneName)
    {
        PrepareForSceneLoad();

        if (useAsyncLoading)
        {
            StartCoroutine(
                LoadSceneByNameRoutine(sceneName)
            );
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void BeginSceneLoad(int buildIndex)
    {
        PrepareForSceneLoad();

        if (useAsyncLoading)
        {
            StartCoroutine(
                LoadSceneByIndexRoutine(buildIndex)
            );
        }
        else
        {
            SceneManager.LoadScene(buildIndex);
        }
    }

    private void PrepareForSceneLoad()
    {
        isLoading = true;

        // Victory, game-over, and pause panels may have
        // stopped gameplay by setting timeScale to zero.
        Time.timeScale = 1f;
    }

    private IEnumerator LoadSceneByNameRoutine(
        string sceneName)
    {
        AsyncOperation loadingOperation =
            SceneManager.LoadSceneAsync(sceneName);

        if (loadingOperation == null)
        {
            isLoading = false;

            Debug.LogError(
                $"Unity failed to begin loading scene " +
                $"'{sceneName}'."
            );

            yield break;
        }

        while (!loadingOperation.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator LoadSceneByIndexRoutine(
        int buildIndex)
    {
        AsyncOperation loadingOperation =
            SceneManager.LoadSceneAsync(buildIndex);

        if (loadingOperation == null)
        {
            isLoading = false;

            Debug.LogError(
                $"Unity failed to begin loading scene " +
                $"at build index {buildIndex}."
            );

            yield break;
        }

        while (!loadingOperation.isDone)
        {
            yield return null;
        }
    }
}