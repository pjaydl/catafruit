using TMPro;
using UnityEngine;


public class FruitCounterUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text fruitText;


    public void UpdateFruitCount(
        int remaining,
        int total)
    {
        if (fruitText != null)
        {
            fruitText.text =
                $"Fruits Left: {remaining}/{total}";
        }
    }
}