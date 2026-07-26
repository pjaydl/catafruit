using UnityEngine;

public static class AnimatorExtension
{
    public static bool HasParameterOfType(
        this Animator animator,
        int hash,
        AnimatorControllerParameterType type)
    {
        foreach (
            AnimatorControllerParameter parameter
            in animator.parameters)
        {
            if (parameter.nameHash == hash &&
               parameter.type == type)
            {
                return true;
            }
        }

        return false;
    }
}