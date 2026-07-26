using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CatapultPhysics))]
[RequireComponent(typeof(ProjectileManager))]
[RequireComponent(typeof(ProjectileLauncher))]
[RequireComponent(typeof(GameOverController))]
public class CatapultGameController : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private CatapultPhysics catapultPhysics;
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private ProjectileLauncher projectileLauncher;
    [SerializeField] private GameOverController gameOverController;

    [Header("Camera")]
    [SerializeField]
    private ProjectileCameraFollow projectileCameraFollow;

    [Header("Aiming")]
    [SerializeField] private AimCursor aimCursor;
    [SerializeField] private AimRotation aimRotation;
    [SerializeField] private Transform aimTarget;

    [Header("Timing")]
    [SerializeField] private float releaseDelay = 0.5f;
    [SerializeField] private float resetDelay = 5f;

    [Header("Cleanup")]
    [SerializeField] private bool destroySpentProjectile = true;

    private bool canLaunch;
    private Coroutine launchRoutine;

    private void Awake()
    {
        FindRequiredComponents();
    }

    private void Start()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        gameOverController.Initialize();
        projectileManager.Initialize();

        catapultPhysics.SetActive(false);

        canLaunch = true;
    }

    private void FindRequiredComponents()
    {
        if (catapultPhysics == null)
        {
            catapultPhysics =
                GetComponent<CatapultPhysics>();
        }

        if (projectileManager == null)
        {
            projectileManager =
                GetComponent<ProjectileManager>();
        }

        if (projectileLauncher == null)
        {
            projectileLauncher =
                GetComponent<ProjectileLauncher>();
        }

        if (gameOverController == null)
        {
            gameOverController =
                GetComponent<GameOverController>();
        }

        if (projectileCameraFollow == null)
        {
            projectileCameraFollow =
                FindFirstObjectByType<ProjectileCameraFollow>();
        }
    }

    private bool ValidateReferences()
    {
        bool valid = true;

        if (catapultPhysics == null)
        {
            Debug.LogError(
                "CatapultPhysics is missing."
            );

            valid = false;
        }

        if (projectileManager == null)
        {
            Debug.LogError(
                "ProjectileManager is missing."
            );

            valid = false;
        }

        if (projectileLauncher == null)
        {
            Debug.LogError(
                "ProjectileLauncher is missing."
            );

            valid = false;
        }

        if (gameOverController == null)
        {
            Debug.LogError(
                "GameOverController is missing."
            );

            valid = false;
        }

        if (aimTarget == null)
        {
            Debug.LogError(
                "Aim Target is missing."
            );

            valid = false;
        }

        return valid;
    }

    private void Update()
    {
        if (!canLaunch)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            BeginLaunch();
        }
    }

    private void BeginLaunch()
    {
        if (launchRoutine != null)
            return;

        if (Time.timeScale <= 0f)
            return;

        if (projectileManager.CurrentProjectile == null)
        {
            Debug.Log(
                "No fruit is currently loaded."
            );

            return;
        }

        canLaunch = false;

        aimCursor?.LockTarget();
        aimRotation?.LockRotation();

        Vector3 targetPosition =
            aimTarget.position;

        launchRoutine =
            StartCoroutine(
                LaunchSequence(targetPosition)
            );
    }

    private IEnumerator LaunchSequence(
        Vector3 targetPosition)
    {
        catapultPhysics.SetActive(true);

        yield return new WaitForSeconds(
            releaseDelay
        );

        GameObject loadedProjectile =
            projectileManager.RemoveCurrentProjectile();

        if (loadedProjectile == null)
        {
            Debug.LogError(
                "The loaded projectile is missing."
            );

            FinishFailedLaunch();
            yield break;
        }

        GameObject launchedProjectile =
            projectileLauncher.Launch(
                loadedProjectile,
                targetPosition
            );

        if (launchedProjectile == null)
        {
            FinishFailedLaunch();
            yield break;
        }

        if (projectileCameraFollow != null)
        {
            projectileCameraFollow.FollowProjectile(
                launchedProjectile
            );
        }

        projectileManager.RegisterLaunch();

        yield return new WaitForSeconds(
            resetDelay
        );

        projectileCameraFollow?.StopFollowing();

        if (projectileManager.AllAmmunitionUsed)
        {
            DestroySpentProjectile(
                launchedProjectile
            );

            catapultPhysics.SetActive(false);

            gameOverController.CheckGameOver();

            launchRoutine = null;
            yield break;
        }

        yield return catapultPhysics.ResetPhysics();

        aimRotation?.ResetRotation();
        aimCursor?.ResetCursor();

        DestroySpentProjectile(
            launchedProjectile
        );

        projectileManager.LoadNextProjectile();

        canLaunch = true;
        launchRoutine = null;
    }

    private void FinishFailedLaunch()
    {
        projectileCameraFollow?.StopFollowing();

        catapultPhysics.SetActive(false);

        aimRotation?.ResetRotation();
        aimCursor?.ResetCursor();

        canLaunch = true;
        launchRoutine = null;
    }

    private void DestroySpentProjectile(
        GameObject projectile)
    {
        if (!destroySpentProjectile)
            return;

        if (projectile != null)
        {
            Destroy(projectile);
        }
    }
}