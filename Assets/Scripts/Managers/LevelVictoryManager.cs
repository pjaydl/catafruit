using UnityEngine;

public class LevelVictoryManager : MonoBehaviour
{
    [Header("Victory UI")]
    [SerializeField] private GameObject victoryPanel;

    [Header("Settings")]
    [SerializeField] private bool pauseGameOnVictory = true;

    private bool victoryTriggered;


    private void Start()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }


    public void CheckVictory()
    {
        if (victoryTriggered)
            return;


        SkeletonHealth[] skeletons =
            FindObjectsByType<SkeletonHealth>(
                FindObjectsSortMode.None
            );


        if (skeletons.Length == 0)
        {
            ShowVictory();
        }
    }


    private void ShowVictory()
    {
        victoryTriggered = true;


        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }


        if (pauseGameOnVictory)
        {
            Time.timeScale = 0f;
        }


        Debug.Log("VICTORY!");
    }
}