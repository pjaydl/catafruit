using UnityEngine;
using UnityEngine.InputSystem;

public class CatapultController : MonoBehaviour
{

    public Rigidbody weight;
    public GameObject munition;
    public float releaseDelay = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            //release the weight
            weight.isKinematic = false;
            StartCoroutine(ReleaseMunitionSequence());
        }
    }

    private System.Collections.IEnumerator ReleaseMunitionSequence()
    {
        yield return new WaitForSeconds(releaseDelay);

        //launch the munition
        HingeJoint hingeToDestroy = munition.GetComponent<HingeJoint>();
        if (hingeToDestroy != null)
        {
            Destroy(hingeToDestroy);
        }
    }
}
