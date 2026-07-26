using TMPro;
using UnityEngine;
using System.Collections;


public class ObstacleHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hpText;


    [Header("Settings")]
    [SerializeField] private string obstacleName = "Wood";
    [SerializeField] private float hideDelay = 3f;


    private Camera mainCamera;
    private Coroutine hideRoutine;



    private void Awake()
    {
        mainCamera = Camera.main;

        HideUI();
    }



    public void UpdateHP(int current, int max)
    {
        if (nameText != null)
        {
            nameText.text = obstacleName;
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



    private IEnumerator HideAfterDelay()
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