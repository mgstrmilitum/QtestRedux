using UnityEngine;

public class Weaponswitching : MonoBehaviour
{
    public int selectWeapon_ = 0;
    [SerializeField] QMove playerObj;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectedWeapon();
    }

    // Update is called once per frame
    void Update()
    {

        int prevoiusSelectedweapon = selectWeapon_;


        if (Input.GetAxis("Mouse ScrollWheel")>0f)
        {
            if (selectWeapon_>= transform.childCount-1)
            {
                selectWeapon_ = 0;
            }
            else
            {
                selectWeapon_++;
            }

        }
        if (Input.GetAxis("Mouse ScrollWheel")<0f)
        {
            if (selectWeapon_ <= 0)
            {
                selectWeapon_ = transform.childCount-1;
            }
            else
            {
                selectWeapon_--;
            }

        }

        if (prevoiusSelectedweapon!=selectWeapon_)
        {
            SelectedWeapon();
        }
    }
    void SelectedWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if ((i==selectWeapon_))
            {
                weapon.gameObject.SetActive(true);
                playerObj.activeGun = weapon.gameObject.GetComponent<GunScript1>();
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
