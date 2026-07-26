using UnityEngine;

public class SkeletonHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int health = 3;
    [SerializeField] private int hpMultiplier = 10;


    [Header("Health UI")]
    [SerializeField] private SkeletonHealthUI healthUI;



    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 2f;



    [Header("Animation")]
    [SerializeField] private Animator animator;



    [Header("Death FX")]
    [SerializeField] private GameObject deathFX;
    [SerializeField] private float deathFXDestroyTime = 5f;



    private int currentHP;
    private int maxHP;


    private bool isDead;


    public bool IsDead => isDead;



    private static readonly int DeathTrigger =
        Animator.StringToHash("Death");


    private static readonly int DamageTrigger =
        Animator.StringToHash("TakeDamage");




    private void Awake()
    {
        currentHP = health * hpMultiplier;
        maxHP = currentHP;


        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>();
        }
    }





    private void Start()
    {
        UpdateHealthUI();
        UpdateTargetUI();
    }





    private void OnCollisionEnter(Collision collision)
    {
        if (isDead)
            return;



        if (collision.collider.CompareTag("Projectile"))
        {
            ProjectileDamage projectile =
                collision.gameObject.GetComponent<ProjectileDamage>();



            if (projectile != null &&
               projectile.CanDamage())
            {
                TakeDamage(
                    projectile.Damage
                );


                projectile.SpawnImpactFX();
            }



            Destroy(
                collision.gameObject,
                0.2f
            );
        }
    }






    public void TakeDamage(int damage)
    {
        if (isDead)
            return;



        currentHP -=
            damage * hpMultiplier;



        currentHP =
            Mathf.Max(
                currentHP,
                0
            );



        Debug.Log(
            $"{gameObject.name} HP: {currentHP}/{maxHP}"
        );



        UpdateHealthUI();




        if (animator != null &&
           currentHP > 0)
        {
            if (animator.HasParameterOfType(
                DamageTrigger,
                AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(
                    DamageTrigger
                );
            }
        }




        if (currentHP <= 0)
        {
            Die();
        }
    }







    private void UpdateHealthUI()
    {
        if (healthUI != null)
        {
            healthUI.UpdateHP(
                currentHP,
                maxHP
            );
        }
    }







    private void Die()
    {
        if (isDead)
            return;



        isDead = true;



        SpawnDeathFX();



        if (animator != null)
        {
            if (animator.HasParameterOfType(
                DeathTrigger,
                AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(
                    DeathTrigger
                );
            }
        }



        UpdateTargetUI();



        CheckVictory();




        if (destroyOnDeath)
        {
            Destroy(
                gameObject,
                destroyDelay
            );
        }
    }







    private void SpawnDeathFX()
    {
        if (deathFX == null)
            return;



        GameObject fx =
            Instantiate(
                deathFX,
                transform.position + Vector3.up,
                Quaternion.identity
            );



        Destroy(
            fx,
            deathFXDestroyTime
        );
    }







    private void UpdateTargetUI()
    {
        GameOverController controller =
            FindFirstObjectByType<GameOverController>();


        if (controller != null)
        {
            controller.UpdateTargetCount();
        }
    }







    private void CheckVictory()
    {
        GameOverController controller =
            FindFirstObjectByType<GameOverController>();


        if (controller != null)
        {
            controller.CheckVictory();
        }
    }
}