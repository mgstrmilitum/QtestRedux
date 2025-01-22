using UnityEngine;

public class HealthPack : MonoBehaviour , IPickup 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter(Collider other)
    {
        IPickup item = other.GetComponent<IPickup>();
        if (item != null)
        {
            OnPickup(other);
        }

    }
    public void OnPickup(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
            QMove player = other.transform.GetComponent<QMove>();

            if (player != null)
            {
                player.AddHealth(50);
                Destroy(gameObject);
            }

        }
    
}

