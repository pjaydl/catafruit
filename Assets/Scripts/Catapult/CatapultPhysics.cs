using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultPhysics : MonoBehaviour
{
    [Header("Catapult Parts")]
    [SerializeField] private Transform armRoot;
    [SerializeField] private Rigidbody weight;
    [SerializeField] private Rigidbody[] additionalBodies;

    private Vector3 armStartingPosition;
    private Quaternion armStartingRotation;

    private readonly List<BodyState> bodyStates = new();

    private sealed class BodyState
    {
        public Rigidbody Body { get; }

        public Vector3 LocalPosition { get; }
        public Quaternion LocalRotation { get; }

        public bool UseGravity { get; }
        public bool DetectCollisions { get; }

        public CollisionDetectionMode CollisionMode { get; }
        public RigidbodyInterpolation Interpolation { get; }

        public BodyState(Rigidbody body)
        {
            Body = body;

            LocalPosition =
                body.transform.localPosition;

            LocalRotation =
                body.transform.localRotation;

            UseGravity =
                body.useGravity;

            DetectCollisions =
                body.detectCollisions;

            CollisionMode =
                body.collisionDetectionMode;

            Interpolation =
                body.interpolation;
        }
    }

    private void Awake()
    {
        CacheStartingState();
    }

    public void SetActive(bool active)
    {
        foreach (BodyState state in bodyStates)
        {
            Rigidbody body = state.Body;

            if (body == null)
                continue;

            /*
             * Velocity can only be changed while the Rigidbody
             * is non-kinematic.
             */
            StopBodyIfDynamic(body);

            if (active)
            {
                EnablePhysicsBody(body, state);
            }
            else
            {
                DisablePhysicsBody(body, state);
            }
        }
    }

    private void EnablePhysicsBody(
        Rigidbody body,
        BodyState state)
    {
        /*
         * Set isKinematic to false before restoring
         * normal physics settings.
         */
        body.isKinematic = false;

        body.useGravity =
            state.UseGravity;

        body.detectCollisions =
            state.DetectCollisions;

        body.collisionDetectionMode =
            state.CollisionMode;

        body.interpolation =
            state.Interpolation;

        body.linearVelocity =
            Vector3.zero;

        body.angularVelocity =
            Vector3.zero;

        body.WakeUp();
    }

    private void DisablePhysicsBody(
        Rigidbody body,
        BodyState state)
    {
        /*
         * Stop the body before making it kinematic.
         */
        StopBodyIfDynamic(body);

        body.useGravity =
            state.UseGravity;

        body.detectCollisions =
            state.DetectCollisions;

        body.interpolation =
            state.Interpolation;

        body.isKinematic = true;
    }

    private void StopBodyIfDynamic(
        Rigidbody body)
    {
        if (body == null)
            return;

        if (body.isKinematic)
            return;

        body.linearVelocity =
            Vector3.zero;

        body.angularVelocity =
            Vector3.zero;
    }

    public IEnumerator ResetPhysics()
    {
        /*
         * This stops all dynamic bodies first,
         * then makes them kinematic.
         */
        SetActive(false);

        yield return new WaitForFixedUpdate();

        if (armRoot != null)
        {
            armRoot.SetLocalPositionAndRotation(
                armStartingPosition,
                armStartingRotation
            );
        }

        foreach (BodyState state in bodyStates)
        {
            Rigidbody body = state.Body;

            if (body == null)
                continue;

            /*
             * Do not set linearVelocity or angularVelocity here.
             * The Rigidbody is already kinematic.
             */
            body.transform.SetLocalPositionAndRotation(
                state.LocalPosition,
                state.LocalRotation
            );

            body.position =
                body.transform.position;

            body.rotation =
                body.transform.rotation;
        }

        Physics.SyncTransforms();

        yield return new WaitForFixedUpdate();
    }

    private void CacheStartingState()
    {
        bodyStates.Clear();

        if (armRoot != null)
        {
            armStartingPosition =
                armRoot.localPosition;

            armStartingRotation =
                armRoot.localRotation;
        }

        HashSet<Rigidbody> registeredBodies = new();

        if (armRoot != null)
        {
            Rigidbody[] armBodies =
                armRoot.GetComponentsInChildren<Rigidbody>(
                    true
                );

            foreach (Rigidbody body in armBodies)
            {
                RegisterBody(
                    body,
                    registeredBodies
                );
            }
        }

        if (additionalBodies != null)
        {
            foreach (Rigidbody body in additionalBodies)
            {
                RegisterBody(
                    body,
                    registeredBodies
                );
            }
        }

        RegisterBody(
            weight,
            registeredBodies
        );
    }

    private void RegisterBody(
        Rigidbody body,
        HashSet<Rigidbody> registeredBodies)
    {
        if (body == null)
            return;

        /*
         * Prevent the loaded fruit Rigidbody from being
         * controlled by the catapult reset system.
         */
        ProjectileDamage projectile =
            body.GetComponent<ProjectileDamage>();

        if (projectile == null)
        {
            projectile =
                body.GetComponentInParent<ProjectileDamage>();
        }

        if (projectile != null)
        {
            Debug.Log(
                $"Projectile excluded from CatapultPhysics: " +
                $"{body.name}"
            );

            return;
        }

        if (!registeredBodies.Add(body))
            return;

        bodyStates.Add(
            new BodyState(body)
        );
    }
}