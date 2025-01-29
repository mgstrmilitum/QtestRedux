using TMPro;
using UnityEngine;

public class Adjustablegunscript : MonoBehaviour
{
    //bullet
    public GameObject bullet;

    //bullet force
    public ForceMode shootForce, upwardForce;

    //gunstats

    public float timebetweenshooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtondownHold;

    int bulletsLeft, bulletsShot;

    //bools
    bool shooting, readyToShoot, reloading;

    //reference
    public Camera fpsCam;
    public Transform attackPoint;

    //Graphics
    public GameObject muzzleflash;
    public TextMeshProUGUI ammodsplay;

    //bug fixing
    public bool allowInvoke = true;


    private void Awake()
    {
        bulletsLeft=magazineSize;
        readyToShoot=true;
    }


    void Update()
    {
        MyInput();
        //Set ammo display
        if(ammodsplay != null )
        {
            ammodsplay.SetText(bulletsLeft/bulletsPerTap+" / "+magazineSize/bulletsPerTap);
        }
    }

    private void MyInput()
    {
        if(allowButtondownHold)//auto
        {
            shooting=Input.GetKey(KeyCode.Mouse0);
        }
        else//semi
        {
            shooting= Input.GetKeyDown(KeyCode.Mouse0);
        }
        //Reloding
        if(Input.GetKeyDown(KeyCode.R) && bulletsLeft<magazineSize &&!reloading)
        {
            Reloading();
        }


        //Shooting
        if(readyToShoot && shooting && !reloading && bulletsLeft >0)
        {
            //set bullets shot to 0
            bulletsShot=0;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Where the raycast is pointing to
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPointed;
        if(Physics.Raycast(ray,out hit))
        {
            targetPointed = hit.point;
        }
        else
        {
            targetPointed=ray.GetPoint(75);
        }

        //Calulating direction
        Vector3 directionWNOspread = targetPointed-attackPoint.position;

        //Calulating spread
        float x= Random.Range(-spread,spread);
        float y= Random.Range(-spread, spread);

        //Calulating new direction with spread
        Vector3 directionWspread=directionWNOspread+new Vector3(x,y,0);

        //Instantiate bullet
        GameObject currentBullet = Instantiate(bullet,attackPoint.position,Quaternion.identity );//store bullet

        //rotate bullet
        currentBullet.transform.forward=directionWspread.normalized;

        //Add force in bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWspread.normalized, ForceMode.Impulse);
        //currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up, ForceMode.Impulse);

        //muzzleFlash
        if(muzzleflash!=null)
        {
            Instantiate(muzzleflash,attackPoint.position,Quaternion.identity);
        }

        bulletsLeft--;
        bulletsShot--;

        //Invoke resetSHot
        if(allowInvoke)
        {
            Invoke("ResetShot", timebetweenshooting);
            allowInvoke = false;
        }
    }

    private void ResetShot()
    {
        readyToShoot=true;
        allowInvoke=true;
    }
    private void Reloading()
    {
        reloading =true;
        Invoke("ReloadFinished",reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft=magazineSize;
        reloading=false;
    }

}
