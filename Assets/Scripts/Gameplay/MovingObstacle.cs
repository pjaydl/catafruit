using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingObstacle : MonoBehaviour
{
    public enum MovementAxis
    {
        Horizontal,
        Vertical
    }

    [Header("Movement Direction")]
    [SerializeField]
    private MovementAxis movementAxis =
        MovementAxis.Horizontal;

    [Tooltip(
        "Move using the obstacle's local right/up direction. " +
        "Disable this to use the world's X/Y direction."
    )]
    [SerializeField] private bool useLocalDirection = true;

    [Header("Movement Settings")]
    [Min(0f)]
    [SerializeField] private float movementDistance = 5f;

    [Min(0f)]
    [SerializeField] private float movementSpeed = 2f;

    [Tooltip("How long the obstacle waits at each endpoint.")]
    [Min(0f)]
    [SerializeField] private float waitAtEnds = 0.5f;

    [Header("Physics Settings")]
    [SerializeField] private bool configureRigidbodyAutomatically = true;

    private Rigidbody obstacleBody;

    private Vector3 startingPosition;
    private Vector3 endingPosition;
    private Vector3 movementDirection;

    private bool movingToEnd = true;
    private float waitTimer;

    private const float ArrivalDistance = 0.001f;

    private void Awake()
    {
        obstacleBody = GetComponent<Rigidbody>();

        ConfigureRigidbody();
        CacheMovementPath();
    }

    private void FixedUpdate()
    {
        if (obstacleBody == null)
            return;

        if (movementSpeed <= 0f ||
            movementDistance <= 0f)
        {
            return;
        }

        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector3 targetPosition =
            movingToEnd
                ? endingPosition
                : startingPosition;

        Vector3 nextPosition =
            Vector3.MoveTowards(
                obstacleBody.position,
                targetPosition,
                movementSpeed * Time.fixedDeltaTime
            );

        obstacleBody.MovePosition(nextPosition);

        if ((nextPosition - targetPosition).sqrMagnitude
            <= ArrivalDistance * ArrivalDistance)
        {
            movingToEnd = !movingToEnd;
            waitTimer = waitAtEnds;
        }
    }

    private void ConfigureRigidbody()
    {
        if (!configureRigidbodyAutomatically)
            return;

        obstacleBody.isKinematic = true;
        obstacleBody.useGravity = false;
        obstacleBody.detectCollisions = true;

        obstacleBody.interpolation =
            RigidbodyInterpolation.Interpolate;

        /*
         * Continuous Speculative works with kinematic
         * moving bodies and helps prevent missed collisions.
         */
        obstacleBody.collisionDetectionMode =
            CollisionDetectionMode.ContinuousSpeculative;
    }

    private void CacheMovementPath()
    {
        startingPosition = obstacleBody.position;

        movementDirection =
            GetMovementDirection();

        endingPosition =
            startingPosition +
            movementDirection * movementDistance;
    }

    private Vector3 GetMovementDirection()
    {
        if (movementAxis == MovementAxis.Horizontal)
        {
            return useLocalDirection
                ? transform.right.normalized
                : Vector3.right;
        }

        return useLocalDirection
            ? transform.up.normalized
            : Vector3.up;
    }

    public void ResetObstaclePosition()
    {
        if (obstacleBody == null)
            return;

        movingToEnd = true;
        waitTimer = 0f;

        obstacleBody.position =
            startingPosition;

        obstacleBody.rotation =
            transform.rotation;

        Physics.SyncTransforms();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 previewDirection;

        if (movementAxis == MovementAxis.Horizontal)
        {
            previewDirection =
                useLocalDirection
                    ? transform.right.normalized
                    : Vector3.right;
        }
        else
        {
            previewDirection =
                useLocalDirection
                    ? transform.up.normalized
                    : Vector3.up;
        }

        Vector3 previewStart =
            Application.isPlaying
                ? startingPosition
                : transform.position;

        Vector3 previewEnd =
            previewStart +
            previewDirection * movementDistance;

        Gizmos.DrawLine(
            previewStart,
            previewEnd
        );

        Gizmos.DrawWireSphere(
            previewStart,
            0.25f
        );

        Gizmos.DrawWireSphere(
            previewEnd,
            0.25f
        );
    }
}