using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Pages")]
    [SerializeField] private GameObject[] pages;

    [Header("Navigation Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;

    [Header("Final Page Button")]
    [SerializeField] private GameObject gotItButton;

    [Header("Menu References")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject mainButtons;

    private int currentPage;

    private void OnEnable()
    {
        ShowPage(0);
    }

    public void NextPage()
    {
        if (!PagesAreValid())
            return;

        if (currentPage < pages.Length - 1)
        {
            ShowPage(currentPage + 1);
        }
    }

    public void PreviousPage()
    {
        if (!PagesAreValid())
            return;

        if (currentPage > 0)
        {
            ShowPage(currentPage - 1);
        }
        else
        {
            ReturnToMainMenu();
        }
    }

    public void GotIt()
    {
        ReturnToMainMenu();
    }

    private void ShowPage(int pageIndex)
    {
        if (!PagesAreValid())
            return;

        currentPage = Mathf.Clamp(
            pageIndex,
            0,
            pages.Length - 1
        );

        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
            {
                pages[i].SetActive(i == currentPage);
            }
        }

        UpdateNavigationButtons();

        Debug.Log(
            $"Tutorial page {currentPage + 1} of {pages.Length}"
        );
    }

    private void UpdateNavigationButtons()
    {
        bool isLastPage =
            currentPage == pages.Length - 1;

        if (backButton != null)
        {
            backButton.gameObject.SetActive(true);
            backButton.interactable = true;
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(!isLastPage);
            nextButton.interactable = !isLastPage;
        }

        if (gotItButton != null)
        {
            gotItButton.SetActive(isLastPage);
        }
    }

    private void ReturnToMainMenu()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "Tutorial Panel is not assigned."
            );
        }

        if (mainButtons != null)
        {
            mainButtons.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "Main Buttons is not assigned."
            );
        }
    }

    private bool PagesAreValid()
    {
        if (pages == null || pages.Length == 0)
        {
            Debug.LogError(
                "No tutorial pages are assigned."
            );

            return false;
        }

        return true;
    }
}