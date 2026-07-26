using UnityEngine;
using UnityEngine.InputSystem;

public class AimCursor : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private float minX = -300f;
    [SerializeField] private float maxX = 300f;
    [SerializeField] private float horizontalSpeed = 200f;

    [Header("Vertical Movement")]
    [SerializeField] private float minY = -192f;
    [SerializeField] private float maxY = 192f;
    [SerializeField] private float verticalSpeed = 200f;

    [Header("Boundary Padding")]
    [Tooltip("Keeps the cursor away from the left and right edges.")]
    [SerializeField] private float horizontalPadding = 15f;

    [Tooltip("Keeps the cursor away from the top and bottom edges.")]
    [SerializeField] private float verticalPadding = 15f;

    private Vector3 originalPosition;

    private float lockedX;
    private float lockedY;

    /*
     * False after Spacebar is pressed.
     * The cursor starts moving again after ResetCursor() is called.
     */
    private bool movementEnabled = true;

    /*
     * Padding is added to the minimum limits and subtracted
     * from the maximum limits. This stops the cursor before
     * it reaches the visible border.
     */
    private float MinimumAllowedX =>
        minX + Mathf.Max(0f, horizontalPadding);

    private float MaximumAllowedX =>
        maxX - Mathf.Max(0f, horizontalPadding);

    private float MinimumAllowedY =>
        minY + Mathf.Max(0f, verticalPadding);

    private float MaximumAllowedY =>
        maxY - Mathf.Max(0f, verticalPadding);

    private void Awake()
    {
        originalPosition = transform.localPosition;

        ValidateLimits();

        // Ensure the starting position is also inside the limits.
        Vector3 position = originalPosition;

        position.x = Mathf.Clamp(
            position.x,
            MinimumAllowedX,
            MaximumAllowedX
        );

        position.y = Mathf.Clamp(
            position.y,
            MinimumAllowedY,
            MaximumAllowedY
        );

        position.z = originalPosition.z;

        transform.localPosition = position;

        lockedX = position.x;
        lockedY = position.y;
    }

    private void Update()
    {
        if (!movementEnabled)
            return;

        if (Keyboard.current == null)
            return;

        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        // Horizontal controls.
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            horizontalInput -= 1f;
        }

        if (Keyboard.current.rightArrowKey.isPressed)
        {
            horizontalInput += 1f;
        }

        // Vertical controls.
        if (Keyboard.current.downArrowKey.isPressed)
        {
            verticalInput -= 1f;
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            verticalInput += 1f;
        }

        Vector2 inputDirection = new Vector2(
            horizontalInput,
            verticalInput
        );

        /*
         * When two keys are held, such as Up and Right,
         * the vector's magnitude becomes greater than 1.
         *
         * Normalizing it prevents diagonal movement from
         * being faster than straight movement.
         */
        if (inputDirection.sqrMagnitude > 1f)
        {
            inputDirection.Normalize();
        }

        Vector3 position = transform.localPosition;

        position.x +=
            inputDirection.x *
            horizontalSpeed *
            Time.deltaTime;

        position.y +=
            inputDirection.y *
            verticalSpeed *
            Time.deltaTime;

        /*
         * Clamp prevents the cursor from moving outside
         * its allowed rectangular aiming area.
         */
        position.x = Mathf.Clamp(
            position.x,
            MinimumAllowedX,
            MaximumAllowedX
        );

        position.y = Mathf.Clamp(
            position.y,
            MinimumAllowedY,
            MaximumAllowedY
        );

        // The cursor should not move forward or backward.
        position.z = originalPosition.z;

        transform.localPosition = position;
    }

    /// <summary>
    /// Stops the cursor at its current position.
    /// Called immediately when Spacebar launches the fruit.
    /// </summary>
    public void LockTarget()
    {
        Vector3 position = transform.localPosition;

        lockedX = Mathf.Clamp(
            position.x,
            MinimumAllowedX,
            MaximumAllowedX
        );

        lockedY = Mathf.Clamp(
            position.y,
            MinimumAllowedY,
            MaximumAllowedY
        );

        position.x = lockedX;
        position.y = lockedY;
        position.z = originalPosition.z;

        transform.localPosition = position;

        movementEnabled = false;

        Debug.Log(
            $"Cursor locked at X: {lockedX}, Y: {lockedY}."
        );
    }

    /// <summary>
    /// Returns the cursor to its starting position and
    /// enables arrow-key movement again.
    /// </summary>
    public void ResetCursor()
    {
        Vector3 resetPosition = originalPosition;

        resetPosition.x = Mathf.Clamp(
            resetPosition.x,
            MinimumAllowedX,
            MaximumAllowedX
        );

        resetPosition.y = Mathf.Clamp(
            resetPosition.y,
            MinimumAllowedY,
            MaximumAllowedY
        );

        resetPosition.z = originalPosition.z;

        transform.localPosition = resetPosition;

        lockedX = resetPosition.x;
        lockedY = resetPosition.y;

        movementEnabled = true;

        Debug.Log(
            "Aim cursor reset. Arrow-key movement enabled."
        );
    }

    public Vector2 GetLockedPosition()
    {
        return new Vector2(
            lockedX,
            lockedY
        );
    }

    /// <summary>
    /// Converts the cursor's X position into a value from 0 to 1.
    /// AimRotation uses this value to rotate the catapult.
    /// </summary>
    public float GetHorizontalNormalized()
    {
        return Mathf.InverseLerp(
            MinimumAllowedX,
            MaximumAllowedX,
            transform.localPosition.x
        );
    }

    /*
     * These methods remain available because AimRotation may
     * still call IsHorizontal().
     */
    public bool IsHorizontal()
    {
        return movementEnabled;
    }

    public bool IsVertical()
    {
        return movementEnabled;
    }

    public bool IsLocked()
    {
        return !movementEnabled;
    }

    public bool IsMovementEnabled()
    {
        return movementEnabled;
    }

    private void ValidateLimits()
    {
        if (minX >= maxX)
        {
            Debug.LogError(
                "AimCursor: Min X must be lower than Max X."
            );
        }

        if (minY >= maxY)
        {
            Debug.LogError(
                "AimCursor: Min Y must be lower than Max Y."
            );
        }

        if (MinimumAllowedX >= MaximumAllowedX)
        {
            Debug.LogError(
                "AimCursor: Horizontal padding is too large."
            );
        }

        if (MinimumAllowedY >= MaximumAllowedY)
        {
            Debug.LogError(
                "AimCursor: Vertical padding is too large."
            );
        }
    }
}