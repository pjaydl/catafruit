using UnityEngine;

public class ObstacleHealth : MonoBehaviour
{
    public enum ObstacleType
    {
        Wood,
        Stone,
        Steel
    }

    [Header("Obstacle Settings")]
    [SerializeField] private ObstacleType obstacleType;

    [Header("Health UI")]
    [SerializeField] private ObstacleHealthUI healthUI;

    [Header("Destroy Settings")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 0.5f;

    [Header("Obstacle Destroy FX")]
    [SerializeField] private GameObject destroyFX;
    [SerializeField] private float destroyFXLifetime = 3f;

    [Header("Debugging")]
    [SerializeField] private bool showCollisionLogs = true;

    private int currentHP;
    private int maxHP;
    private bool destroyed;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;
    public bool IsDestroyed => destroyed;

    private void Awake()
    {
        SetInitialHealth();
        maxHP = currentHP;
    }

    private void Start()
    {
        UpdateHealthUI();
    }

    private void SetInitialHealth()
    {
        switch (obstacleType)
        {
            case ObstacleType.Wood:
                currentHP = 20;
                break;

            case ObstacleType.Stone:
                currentHP = 50;
                break;

            case ObstacleType.Steel:
                currentHP = 100;
                break;

            default:
                currentHP = 20;
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (destroyed || collision == null)
            return;

        ProjectileDamage projectile =
            FindProjectileDamage(collision);

        if (projectile == null)
        {
            if (showCollisionLogs)
            {
                Debug.Log(
                    $"{gameObject.name} collided with " +
                    $"{collision.collider.name}, but it was not a projectile."
                );
            }

            return;
        }

        if (!projectile.CanDamage())
            return;

        GetImpactInformation(
            collision,
            out Vector3 impactPosition,
            out Vector3 impactNormal
        );

        // Spawn the fruit impact particles before destroying the fruit.
        projectile.SpawnImpactFX(
            impactPosition,
            impactNormal
        );

        TakeDamage(projectile.Damage);

        GameObject projectileObject =
            GetProjectileRoot(
                collision,
                projectile
            );

        if (projectileObject != null)
        {
            Destroy(
                projectileObject,
                0.2f
            );
        }
    }

    private void GetImpactInformation(
        Collision collision,
        out Vector3 impactPosition,
        out Vector3 impactNormal)
    {
        if (collision.contactCount > 0)
        {
            ContactPoint contact =
                collision.GetContact(0);

            impactPosition =
                contact.point;

            impactNormal =
                contact.normal;

            return;
        }

        impactPosition =
            collision.collider.ClosestPoint(
                transform.position
            );

        impactNormal =
            (
                impactPosition -
                transform.position
            ).normalized;

        if (impactNormal.sqrMagnitude < 0.001f)
        {
            impactNormal = Vector3.up;
        }
    }

    private ProjectileDamage FindProjectileDamage(
        Collision collision)
    {
        ProjectileDamage projectile =
            collision.collider
                .GetComponent<ProjectileDamage>();

        if (projectile != null)
            return projectile;

        projectile =
            collision.collider
                .GetComponentInParent<ProjectileDamage>();

        if (projectile != null)
            return projectile;

        Rigidbody otherBody =
            collision.rigidbody;

        if (otherBody == null)
            return null;

        projectile =
            otherBody.GetComponent<ProjectileDamage>();

        if (projectile != null)
            return projectile;

        return otherBody
            .GetComponentInParent<ProjectileDamage>();
    }

    private GameObject GetProjectileRoot(
        Collision collision,
        ProjectileDamage projectile)
    {
        if (collision.rigidbody != null)
        {
            return collision.rigidbody.gameObject;
        }

        Rigidbody projectileBody =
            projectile.GetComponent<Rigidbody>();

        if (projectileBody == null)
        {
            projectileBody =
                projectile.GetComponentInParent<Rigidbody>();
        }

        if (projectileBody != null)
        {
            return projectileBody.gameObject;
        }

        return projectile.gameObject;
    }

    private void TakeDamage(int projectileDamage)
    {
        if (destroyed)
            return;

        int finalDamage =
            Mathf.Max(
                0,
                projectileDamage
            ) * 10;

        currentHP -= finalDamage;

        currentHP =
            Mathf.Max(
                currentHP,
                0
            );

        Debug.Log(
            $"{gameObject.name} received {finalDamage} damage. " +
            $"HP: {currentHP}/{maxHP}"
        );

        UpdateHealthUI();

        if (currentHP <= 0)
        {
            DestroyObstacle();
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

    private void DestroyObstacle()
    {
        if (destroyed)
            return;

        destroyed = true;

        SpawnDestroyFX();

        Debug.Log(
            $"{gameObject.name} was destroyed."
        );

        if (destroyOnDeath)
        {
            Destroy(
                gameObject,
                destroyDelay
            );
        }
    }

    private void SpawnDestroyFX()
    {
        if (destroyFX == null)
            return;

        GameObject fx =
            Instantiate(
                destroyFX,
                transform.position,
                transform.rotation
            );

        Destroy(
            fx,
            destroyFXLifetime
        );
    }
}