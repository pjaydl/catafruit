using UnityEngine;

public class AimRotation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AimCursor aimCursor;

    [Header("Rotation Settings")]
    [SerializeField] private float minimumAngle = -45f;
    [SerializeField] private float maximumAngle = 45f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Rotation Axis")]
    [Tooltip(
        "Enable for forward/backward rotation. " +
        "Disable for left/right rotation."
    )]
    [SerializeField] private bool rotateOnXAxis = false;

    [Header("Direction")]
    [SerializeField] private bool invertDirection = false;

    private Quaternion startingRotation;
    private bool rotationLocked;

    private void Awake()
    {
        /*
         * Store the TurnPivot's original local rotation.
         * ResetRotation() restores this orientation after a shot.
         */
        startingRotation = transform.localRotation;
    }

    private void Update()
    {
        if (aimCursor == null)
            return;

        if (rotationLocked)
            return;

        /*
         * The catapult follows the cursor while the player
         * is still allowed to move it.
         */
        if (!aimCursor.IsMovementEnabled())
            return;

        RotateTowardsCursor();
    }

    private void RotateTowardsCursor()
    {
        /*
         * GetHorizontalNormalized returns:
         *
         * 0 when the cursor is at the left limit.
         * 1 when the cursor is at the right limit.
         */
        float cursorValue =
            aimCursor.GetHorizontalNormalized();

        if (invertDirection)
        {
            cursorValue = 1f - cursorValue;
        }

        /*
         * Convert the normalized cursor position into
         * the configured catapult rotation range.
         */
        float targetAngle = Mathf.Lerp(
            minimumAngle,
            maximumAngle,
            cursorValue
        );

        Quaternion angleRotation;

        if (rotateOnXAxis)
        {
            // Forward and backward tilt.
            angleRotation = Quaternion.Euler(
                targetAngle,
                0f,
                0f
            );
        }
        else
        {
            // Left and right turning.
            angleRotation = Quaternion.Euler(
                0f,
                targetAngle,
                0f
            );
        }

        /*
         * startingRotation is multiplied by angleRotation
         * so the object's original orientation is preserved.
         */
        Quaternion targetRotation =
            startingRotation * angleRotation;

        /*
         * Slerp creates smooth rotation instead of snapping
         * immediately to the target angle.
         */
        transform.localRotation =
            Quaternion.Slerp(
                transform.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    /// <summary>
    /// Stops the catapult at its current horizontal angle.
    /// Called when Spacebar launches the projectile.
    /// </summary>
    public void LockRotation()
    {
        rotationLocked = true;

        Debug.Log(
            "Catapult rotation locked at: " +
            transform.localEulerAngles
        );
    }

    /// <summary>
    /// Returns TurnPivot to its original rotation and allows
    /// it to follow the cursor again.
    /// </summary>
    public void ResetRotation()
    {
        transform.localRotation = startingRotation;
        rotationLocked = false;

        Debug.Log(
            "Catapult rotation reset."
        );
    }

    public void UnlockRotation()
    {
        rotationLocked = false;
    }
}