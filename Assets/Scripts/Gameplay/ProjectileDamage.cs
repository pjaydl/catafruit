using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;

    [Header("Impact FX")]
    [SerializeField] private GameObject impactFX;
    [SerializeField] private float impactDestroyTime = 3f;

    private bool hasHit;

    public int Damage => damage;

    public bool CanDamage()
    {
        if (hasHit)
            return false;

        hasHit = true;
        return true;
    }

    // Backward-compatible version.
    // Existing scripts can still call SpawnImpactFX().
    public void SpawnImpactFX()
    {
        SpawnImpactFX(
            transform.position,
            Vector3.up
        );
    }

    // Improved version that uses the collision point.
    public void SpawnImpactFX(
        Vector3 impactPosition,
        Vector3 impactNormal)
    {
        if (impactFX == null)
            return;

        Vector3 normal =
            impactNormal.sqrMagnitude > 0.001f
                ? impactNormal.normalized
                : Vector3.up;

        Quaternion impactRotation =
            Quaternion.LookRotation(normal);

        GameObject fx =
            Instantiate(
                impactFX,
                impactPosition,
                impactRotation
            );

        Destroy(
            fx,
            impactDestroyTime
        );
    }
}