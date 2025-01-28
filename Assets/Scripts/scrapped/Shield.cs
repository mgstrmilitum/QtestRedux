/*using UnityEngine;

public class Shield : MonoBehaviour, IPickup
{

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
        if ((other.isTrigger))
        {
            return;
        }

        if (other.tag == "Player")
        {

            QMove player = other.transform.GetComponent<QMove>();

            if (player != null)
            {
                player.AddShield(100);
                Destroy(gameObject);
            }

        }
    }
}*/
