using UnityEngine;

public class ProjectileData : MonoBehaviour
{
    [Header("Fruit Information")]
    [SerializeField] private string fruitName;


    [Header("Combat")]
    [SerializeField] private int attackDamage;


    [Header("Projectile Physics")]
    [SerializeField] private ProjectileSpeed speed;
    [SerializeField] private ProjectileWeight weight;



    // ==========================
    // UI DISPLAY
    // ==========================

    public string FruitName => fruitName;


    public string Weight
    {
        get
        {
            return weight.ToString();
        }
    }


    public string Damage
    {
        get
        {
            switch (attackDamage)
            {
                case 1:
                    return "Low";

                case 2:
                    return "Medium";

                case 3:
                    return "High";

                default:
                    return "Unknown";
            }
        }
    }



    // ==========================
    // GAMEPLAY DATA
    // ==========================

    public int AttackDamage => attackDamage;



    // Projectile throwing speed
    public float SpeedMultiplier
    {
        get
        {
            switch (speed)
            {
                case ProjectileSpeed.Fast:
                    return 1.2f;


                case ProjectileSpeed.Medium:
                    return 1.0f;


                case ProjectileSpeed.Slow:
                    return 0.92f;
            }


            return 1f;
        }
    }



    // Used for pushing skeletons/obstacles
    public float ImpactForce
    {
        get
        {
            switch (weight)
            {
                case ProjectileWeight.Light:
                    return 4f;


                case ProjectileWeight.Medium:
                    return 6f;


                case ProjectileWeight.Heavy:
                    return 6.5f;
            }


            return 3f;
        }
    }
}



public enum ProjectileSpeed
{
    Slow,
    Medium,
    Fast
}



public enum ProjectileWeight
{
    Light,
    Medium,
    Heavy
}