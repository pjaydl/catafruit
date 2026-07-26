using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField] private float launchAngle = 45f;
    [SerializeField] private float fallbackLaunchSpeed = 15f;

    public GameObject Launch(
        GameObject projectile,
        Vector3 targetPosition)
    {
        if (projectile == null)
        {
            Debug.LogError("Projectile is missing.");
            return null;
        }

        Rigidbody projectileBody =
            projectile.GetComponent<Rigidbody>();

        if (projectileBody == null)
        {
            projectileBody =
                projectile.GetComponentInChildren<Rigidbody>();
        }

        if (projectileBody == null)
        {
            Debug.LogError(
                $"{projectile.name} needs a Rigidbody."
            );

            return null;
        }

        Collider[] projectileColliders =
            projectile.GetComponentsInChildren<Collider>(true);

        if (projectileColliders.Length == 0)
        {
            Debug.LogError(
                $"{projectile.name} needs at least one Collider."
            );

            return null;
        }

        // Enable the projectile colliders when launching.
        foreach (Collider projectileCollider in projectileColliders)
        {
            if (projectileCollider != null)
            {
                projectileCollider.enabled = true;
            }
        }

        // Detach the fruit from the catapult.
        projectile.transform.SetParent(null, true);

        // Synchronize the transform change with Unity physics.
        Physics.SyncTransforms();

        projectileBody.isKinematic = false;
        projectileBody.useGravity = true;
        projectileBody.detectCollisions = true;

        // Prevent fast projectiles from passing through objects.
        projectileBody.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        projectileBody.interpolation =
            RigidbodyInterpolation.Interpolate;

        projectileBody.linearVelocity = Vector3.zero;
        projectileBody.angularVelocity = Vector3.zero;

        float speedMultiplier = 1f;

        ProjectileData data =
            projectile.GetComponent<ProjectileData>();

        if (data == null)
        {
            data =
                projectile.GetComponentInChildren<ProjectileData>();
        }

        if (data != null)
        {
            speedMultiplier =
                Mathf.Max(0.01f, data.SpeedMultiplier);
        }

        Vector3 launchVelocity;

        bool calculated =
            BallisticCalculator.TryCalculateVelocity(
                projectileBody.position,
                targetPosition,
                launchAngle,
                out launchVelocity
            );

        if (!calculated)
        {
            launchVelocity =
                CalculateFallbackVelocity(
                    projectileBody.position,
                    targetPosition
                );

            Debug.LogWarning(
                $"Ballistic calculation failed for " +
                $"{projectile.name}. Using fallback velocity."
            );
        }

        projectileBody.linearVelocity =
            launchVelocity * speedMultiplier;

        projectileBody.WakeUp();

        Debug.Log(
            $"Projectile launched: {projectile.name}\n" +
            $"Velocity: {projectileBody.linearVelocity}\n" +
            $"Collision mode: " +
            $"{projectileBody.collisionDetectionMode}"
        );

        return projectile;
    }

    private Vector3 CalculateFallbackVelocity(
        Vector3 start,
        Vector3 target)
    {
        Vector3 direction =
            target - start;

        if (direction.sqrMagnitude < 0.001f)
        {
            direction =
                transform.forward +
                Vector3.up;
        }

        return direction.normalized *
               fallbackLaunchSpeed;
    }
}