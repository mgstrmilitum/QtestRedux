using UnityEngine;

public class LiftMovesPlayer : MonoBehaviour
{
    [SerializeField] string playertag = "Player";
    [SerializeField] Transform platform;

    private Vector3 previousPosition;
    private Vector3 platformVelocity;
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        platformVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }

    void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.tag.Equals(playertag))
        {
            other.gameObject.GetComponent<QMove>().playerVelocity += platformVelocity;
        }
    }

    /*void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag.Equals(playertag))
        {
            
        }
    }*/
}
