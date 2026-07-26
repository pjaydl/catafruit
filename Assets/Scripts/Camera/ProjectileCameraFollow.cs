using UnityEngine;


public class ProjectileCameraFollow : MonoBehaviour
{
    [Header("Camera Target")]
    [SerializeField] private Transform catapultTarget;


    [Header("Follow Settings")]
    [SerializeField] private float positionSmooth = 5f;
    [SerializeField] private float rotationSmooth = 3f;

    [SerializeField]
    private Vector3 followOffset =
        new Vector3(0, 2, -6);



    private Transform currentProjectile;

    private Vector3 originalPosition;
    private Quaternion originalRotation;


    private bool followingProjectile;



    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }





    private void LateUpdate()
    {
        if (followingProjectile &&
           currentProjectile != null)
        {
            FollowProjectile();
        }
        else
        {
            ReturnCamera();
        }
    }





    public void FollowProjectile(GameObject projectile)
    {
        if (projectile == null)
            return;


        currentProjectile =
            projectile.transform;


        followingProjectile = true;
    }






    public void StopFollowing()
    {
        followingProjectile = false;

        currentProjectile = null;
    }







    private void FollowProjectile()
    {
        Vector3 targetPosition =
            currentProjectile.position +
            followOffset;



        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                positionSmooth *
                Time.deltaTime
            );



        Quaternion targetRotation =
            Quaternion.LookRotation(
                currentProjectile.position -
                transform.position
            );



        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmooth *
                Time.deltaTime
            );
    }








    private void ReturnCamera()
    {
        if (catapultTarget == null)
            return;



        transform.position =
            Vector3.Lerp(
                transform.position,
                originalPosition,
                positionSmooth *
                Time.deltaTime
            );



        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                originalRotation,
                rotationSmooth *
                Time.deltaTime
            );
    }
}