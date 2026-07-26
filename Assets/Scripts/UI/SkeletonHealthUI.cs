using TMPro;
using UnityEngine;

public class SkeletonHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hpText;


    [Header("Settings")]
    [SerializeField] private string enemyName = "Skeleton";
    [SerializeField] private float hideDelay = 3f;


    private Camera mainCamera;
    private Coroutine hideRoutine;



    private void Awake()
    {
        mainCamera = Camera.main;

        nameText.text = enemyName;

        HideUI();
    }



    public void UpdateHP(int current, int max)
    {
        if (nameText != null)
        {
            nameText.text = enemyName;
        }


        if (hpText != null)
        {
            hpText.text =
                $"HP: {current}";
        }


        ShowUI();
    }



    private void ShowUI()
    {
        gameObject.SetActive(true);


        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }


        hideRoutine =
            StartCoroutine(
                HideAfterDelay()
            );
    }



    private System.Collections.IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(
            hideDelay
        );


        HideUI();
    }



    private void HideUI()
    {
        gameObject.SetActive(false);
    }



    private void LateUpdate()
    {
        if (mainCamera == null)
            return;


        transform.LookAt(
            transform.position +
            mainCamera.transform.rotation *
            Vector3.forward
        );
    }
}