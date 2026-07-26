using TMPro;
using UnityEngine;


public class CurrentFruitUI : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] private TMP_Text fruitNameText;
    [SerializeField] private TMP_Text weightText;
    [SerializeField] private TMP_Text damageText;



    public void DisplayFruit(ProjectileData fruit)
    {
        if (fruit == null)
        {
            ClearUI();
            return;
        }


        if (fruitNameText != null)
        {
            fruitNameText.text =
                fruit.FruitName;
        }


        if (weightText != null)
        {
            weightText.text =
                $"Weight: {fruit.Weight}";
        }


        if (damageText != null)
        {
            damageText.text =
                $"Damage: {fruit.Damage}";
        }
    }



    private void ClearUI()
    {
        if (fruitNameText != null)
        {
            fruitNameText.text = "";
        }


        if (weightText != null)
        {
            weightText.text = "";
        }


        if (damageText != null)
        {
            damageText.text = "";
        }
    }
}