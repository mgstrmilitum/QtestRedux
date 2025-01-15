using UnityEngine;

public class Items : MonoBehaviour
{

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
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
}
