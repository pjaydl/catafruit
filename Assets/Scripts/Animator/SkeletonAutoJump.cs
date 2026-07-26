using System.Collections;
using UnityEngine;

public class SkeletonAutoJump : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private float jumpInterval = 5f;

    private readonly int jumpHash =
        Animator.StringToHash("Jump");


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void Start()
    {
        StartCoroutine(JumpRoutine());
    }


    private IEnumerator JumpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(jumpInterval);

            animator.SetTrigger(jumpHash);
        }
    }
}