using UnityEngine;

public class LiftMovesPlayer : MonoBehaviour
{
    [SerializeField] string playertag = "Player";
    [SerializeField] Transform platform;
    [SerializeField] float velocityDelay;
    [SerializeField] Vector3 gravLiftVelocity;
    [SerializeField] bool gravLift;





    private Vector3 previousPosition;
    private Vector3 platformVelocity;
    Rigidbody rb;
    
    float localVelocityDelay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        previousPosition = transform.position;
        localVelocityDelay = velocityDelay;
    }

    // Update is called once per frame
    void Update()
    {
        platformVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gravLift == false)
        {
            if (other.gameObject.tag.Equals(playertag))
            {
                other.transform.parent = platform;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (gravLift == false)
        {
            if (other.gameObject.tag.Equals(playertag))
            {
                other.transform.parent = null;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.tag.Equals(playertag) && localVelocityDelay <= 0)
        {
            if (gravLift == true)
            {
                other.gameObject.GetComponent<QMove>().playerVelocity += gravLiftVelocity;
            }
            else
            {
                other.gameObject.GetComponent<QMove>().playerVelocity += platformVelocity;
            }
            localVelocityDelay = velocityDelay;
        }
        else
        {
            localVelocityDelay -= Time.deltaTime;
        }
    }
}
