using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] GunStats gun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickup pick = other.GetComponent<IPickup>();

        if (other.CompareTag("Player")) 
        {
<<<<<<< Updated upstream:Assets/Scripts/pickup.cs
            pick.GetGunStats(gun);
=======
            //transfer gun to the IPickup object
            pick.OnPickup(other);
>>>>>>> Stashed changes:Assets/Scripts/Pickup.cs
            Destroy(gameObject);
        }
    }
}
