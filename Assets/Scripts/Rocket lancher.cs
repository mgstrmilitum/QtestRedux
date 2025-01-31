using Unity.VisualScripting;
using UnityEngine;

public class Rocketlancher : MonoBehaviour
{
    public GameObject rocketPrehaber;
    private Transform rocketTransform;
    public float rocketMovingForce;
    [SerializeField] Transform shootPos;
    public int boomDamage;
    public int rocketLoad;
    public GunScript1 rocketGun;
    
    EnemyAI enemy;

    GameObject rocket;
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
        if(rocketLoad >0)
        { rocket =  Instantiate(rocketPrehaber, shootPos.position, shootPos.rotation);
        Rigidbody body = rocket.GetComponent<Rigidbody>();
        body.isKinematic=false;

        rocket.GetComponent<Rigidbody>().AddForce(rocketTransform.right *rocketMovingForce, ForceMode.Impulse);
        if(rocket.GetComponent<Rigidbody>() != null)
        {
            Destroy(rocket, 1);
        }

        Destroy(rocket, 3);

        }
        rocketLoad--;

    }
    void Setinitalreference()
    {
        rocketTransform= transform;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(rocket);
    }

}
