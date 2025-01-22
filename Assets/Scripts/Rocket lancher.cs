using UnityEngine;

public class Rocketlancher : MonoBehaviour
{
    public GameObject rocketPrehaber;
    private Transform rocketTransform;
    public float rocketMovingForce;
    [SerializeField] Transform shootPos;
    void Start()
    {
        Setinitalreference();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            LaunchRocket();
        }
    }
    void LaunchRocket()
    {
        GameObject rocket = (GameObject) Instantiate(rocketPrehaber, shootPos.position, shootPos.rotation);
        Rigidbody body = rocket.GetComponent<Rigidbody>();
        body.isKinematic=false;

        rocket.GetComponent<Rigidbody>().AddForce(rocketTransform.right *rocketMovingForce, ForceMode.Impulse);

        Destroy(rocket, 3);

    }
    void Setinitalreference()
    {
        rocketTransform= transform;
    }
    
}
