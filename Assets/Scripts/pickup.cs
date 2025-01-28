using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] GunStats gun;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickup pick = other.GetComponent<IPickup>();

        if(pick != null)
        {
            //transfer gun to the IPickup object
            pick.GetGunStats(gun);
            Destroy(gameObject);
        }
    }
}
