using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform projectileHolder;

    [SerializeField] private GameObject[] ammunitionPrefabs;


    [Header("UI")]
    [SerializeField] private CurrentFruitUI fruitUI;
    [SerializeField] private FruitCounterUI fruitCounterUI;



    [Header("Ammunition Settings")]
    [SerializeField] private int ammoMultiplier = 2;


    public GameObject CurrentProjectile { get; private set; }


    public int UsedCount { get; private set; }


    private int ammunitionIndex;



    public int TotalAmmunition =>
        (ammunitionPrefabs?.Length ?? 0)
        * ammoMultiplier;



    public bool AllAmmunitionUsed =>
        UsedCount >= TotalAmmunition;



    public void Initialize()
    {
        ammunitionIndex = 0;
        UsedCount = 0;


        SpawnProjectile();


        UpdateFruitCounter();
    }



    public GameObject RemoveCurrentProjectile()
    {
        GameObject projectile =
            CurrentProjectile;


        CurrentProjectile = null;


        return projectile;
    }



    public void RegisterLaunch()
    {
        UsedCount++;


        UpdateFruitCounter();
    }



    public void LoadNextProjectile()
    {
        if (AllAmmunitionUsed)
            return;


        ammunitionIndex++;


        if (ammunitionIndex >= ammunitionPrefabs.Length)
        {
            ammunitionIndex = 0;
        }


        SpawnProjectile();
    }



    private void SpawnProjectile()
    {
        if (!ValidateConfiguration())
            return;


        GameObject prefab =
            ammunitionPrefabs[ammunitionIndex];


        CurrentProjectile =
            Instantiate(
                prefab,
                projectileHolder.position,
                projectileHolder.rotation,
                projectileHolder
            );


        CurrentProjectile.name =
            $"{prefab.name}_Loaded";


        CurrentProjectile.transform.localPosition =
            Vector3.zero;


        CurrentProjectile.transform.localRotation =
            Quaternion.identity;


        PrepareProjectile(
            CurrentProjectile
        );


        UpdateFruitUI();
    }



    private void PrepareProjectile(
        GameObject projectile)
    {
        Rigidbody body =
            projectile.GetComponent<Rigidbody>();


        if (body == null)
        {
            Debug.LogError(
                $"{projectile.name} has no Rigidbody."
            );

            Destroy(projectile);

            return;
        }


        body.linearVelocity =
            Vector3.zero;


        body.angularVelocity =
            Vector3.zero;


        body.useGravity = true;

        body.detectCollisions = true;


        body.isKinematic = true;
    }



    private void UpdateFruitUI()
    {
        if (CurrentProjectile == null)
            return;


        ProjectileData data =
            CurrentProjectile.GetComponent<ProjectileData>();


        if (data != null &&
           fruitUI != null)
        {
            fruitUI.DisplayFruit(data);
        }
    }



    private void UpdateFruitCounter()
    {
        if (fruitCounterUI != null)
        {
            int remaining =
                TotalAmmunition - UsedCount;


            fruitCounterUI.UpdateFruitCount(
                remaining,
                TotalAmmunition
            );
        }
    }



    private bool ValidateConfiguration()
    {
        if (projectileHolder == null)
        {
            Debug.LogError(
                "Projectile Holder is missing."
            );

            return false;
        }


        if (ammunitionPrefabs == null ||
           ammunitionPrefabs.Length == 0)
        {
            Debug.LogError(
                "No ammunition prefabs assigned."
            );

            return false;
        }


        return true;
    }
}