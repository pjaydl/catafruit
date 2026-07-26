using UnityEngine;

public class CatapultFollowCursor : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private AimCursor aimCursor;

    [Header("Turning Settings")]
    [SerializeField] private float minimumTurnAngle = -45f;
    [SerializeField] private float maximumTurnAngle = 45f;
    [SerializeField] private float turnSpeed = 8f;
    [SerializeField] private bool invertDirection = false;

    private Quaternion startingRotation;
    private bool isLocked;

    private void Start()
    {
        startingRotation = transform.localRotation;
    }

    private void Update()
    {
        if (aimCursor == null)
            return;

        if (isLocked)
            return;

        // Only turn while the cursor moves horizontally.
        if (!aimCursor.IsHorizontal())
            return;

        float cursorPosition =
            aimCursor.GetHorizontalNormalized();

        if (invertDirection)
        {
            cursorPosition = 1f - cursorPosition;
        }

        float turnAngle = Mathf.Lerp(
            minimumTurnAngle,
            maximumTurnAngle,
            cursorPosition
        );

        Quaternion targetRotation =
            startingRotation *
            Quaternion.Euler(
                0f,
                turnAngle,
                0f
            );

        transform.localRotation =
            Quaternion.Slerp(
                transform.localRotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
    }

    public void LockTurning()
    {
        isLocked = true;
    }

    public void UnlockTurning()
    {
        isLocked = false;
    }
}