using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Rendering;

public class Items : MonoBehaviour , IPickup
{
    [SerializeField] ItemIDS ID;
    public Material material;
    enum ItemIDS
    {
        Health,
        Shield,
        Quad,
        AmmoBullets
    }


    public void OnPickup(Collider other)
    {
        //checking if the object that entered is the player
        if (other.isTrigger) { return; }
        QMove player = other.transform.GetComponent<QMove>();
        GunScript1 gun= other.transform.GetComponent<GunScript1>();
        if (player != null || gun!=null)
        {
            switch (ID)
            {
                case ItemIDS.Health:

                    player.AddHealth(50);
                    Destroy(gameObject);
                    break;

                case ItemIDS.Shield:

                    player.AddShield(100);
                    Destroy(gameObject);
                    break;
                case ItemIDS.Quad:
                    //To-Do
                    OnPickup(other);
                    break;

                case ItemIDS.AmmoBullets:
                    gun.AddAmmo(50);
                    Destroy(gameObject);
                    break;

            }
        }

    }
    private void OnTriggerEnter(Collider other)
        {
            //if the object is a child of IPickup then execute OnPickup()
            IPickup item = other.GetComponent<IPickup>();
            if (item != null)
            {
                OnPickup(other);
            }
        }
    
}
