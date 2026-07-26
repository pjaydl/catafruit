using UnityEngine;

public static class BallisticCalculator
{
    public static bool TryCalculateVelocity(
        Vector3 start,
        Vector3 target,
        float angle,
        out Vector3 velocity)
    {
        velocity = Vector3.zero;

        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 horizontalDifference = new Vector3(
            target.x - start.x,
            0f,
            target.z - start.z
        );

        float distance = horizontalDifference.magnitude;
        float heightDifference = target.y - start.y;

        if (distance < 0.01f)
            return false;

        float angleRadians = angle * Mathf.Deg2Rad;
        float cosine = Mathf.Cos(angleRadians);
        float tangent = Mathf.Tan(angleRadians);

        float denominator =
            2f * cosine * cosine *
            (distance * tangent - heightDifference);

        if (denominator <= 0.001f)
            return false;

        float velocitySquared =
            gravity * distance * distance / denominator;

        if (velocitySquared <= 0f ||
            float.IsNaN(velocitySquared) ||
            float.IsInfinity(velocitySquared))
        {
            return false;
        }

        float launchSpeed = Mathf.Sqrt(velocitySquared);

        velocity =
            horizontalDifference.normalized *
            launchSpeed *
            cosine +
            Vector3.up *
            launchSpeed *
            Mathf.Sin(angleRadians);

        return true;
    }
}