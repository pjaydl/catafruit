using TMPro;
using UnityEngine;


public class TargetCounterUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text targetText;


    public void UpdateTargetCount(int remaining)
    {
        if (targetText != null)
        {
            targetText.text =
                $"Targets Left: {remaining}";
        }
    }
}