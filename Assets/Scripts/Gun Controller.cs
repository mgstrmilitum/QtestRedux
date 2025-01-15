using UnityEngine.Events;
using UnityEngine;
using System.Collections;
public class GunScript1 : MonoBehaviour
{
    int bulletLeft, bulletSHot;
    public UnityEvent OnGunShoot;
    public float FireCooldown;
    public int Damage;
    public float BulletRange;
    public float AmmoShot, Ammoleft;
    [SerializeField] Renderer model;
    Color colorOrginal;
    public GameObject impactEffect;
    //semi or auto
    public ParticleSystem mussleflash;
    public bool Railgun;
    private Transform PlayerCamera;
    private float currentCooldown;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    public int MagaizeSize;
    //bools//
    bool shooting, ReadytoShoot, reloading;
    bool shotGun;
    bool IsShooting;
    public bool allowButtonHold;
    void Start()
    {
        currentCooldown = FireCooldown;
        PlayerCamera = Camera.main.transform;
    }
    public IEnumerator flashred()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrginal;
    }
    // Update is called once per frame
    void Update()
    {

        if (!IsShooting)
        {
            StartCoroutine(trailShoot());
        }
        currentCooldown-=FireCooldown;
        if (MagaizeSize >0) { MiniShoot(); Railshoot(); }
        else { return; }

    }
    public void MyInput()
    {
        if (allowButtonHold)
        { shooting =Input.GetKey(KeyCode.Mouse0); }
        else { shooting=Input.GetKeyDown(KeyCode.Mouse0); }

        // if (Input.GetKeyDown(KeyCode.R) && bulletLeft <MagaizeSize&&!reloading) { Reload(); }

        //Shooting//
        if (ReadytoShoot && shooting && !reloading && bulletLeft>0) { mussleflash.Play(); MiniShoot(); }
    }
    public void MiniShoot()
    {
        mussleflash.Play();

        if (Input.GetButton("Shoot"))
        {
            RaycastHit fired;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out fired, BulletRange))
            {
                IDamage dmg = fired.collider.GetComponent<IDamage>();
                if (dmg!=null)
                {

                    dmg.TakeDamage(Damage);
                }
                GameObject ShotImpact = Instantiate(impactEffect, fired.point, Quaternion.LookRotation(fired.normal));
                Destroy(ShotImpact, 2f);

            }

            // Ray gunRay = new Ray(PlayerCamera.position, PlayerCamera.forward);
            //if (Physics.Raycast(gunRay, out RaycastHit hitInfo, BulletRange))
                //if (hitInfo.collider.gameObject.TryGetComponent(out EnemyAI enemy))
                //{
                //    dmg.TakeDamage(Damage);
                //}
        }

        bulletLeft--;
        bulletSHot++;

    }
    public void Railshoot()
    {
        RaycastHit fired;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out fired, BulletRange))
        {
            IDamage dmg = fired.collider.GetComponent<IDamage>();
            if (dmg!=null)
            {

                dmg.TakeDamage(Damage);
            }


        }
        bulletLeft--;
        bulletSHot++;
    }

    IEnumerator trailShoot()
    {
        IsShooting=true;
        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        IsShooting=false;
    }
}

