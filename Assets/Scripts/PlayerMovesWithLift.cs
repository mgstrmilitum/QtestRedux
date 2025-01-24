using UnityEngine;

public class LiftMovesPlayer : MonoBehaviour
{
    [SerializeField] string playertag = "Player";
    [SerializeField] Transform platform;
    GameObject player;
    Rigidbody vRigidBody;
    Vector3 previousPosition;
    Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        previousPosition = platform.position;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = (platform.position - previousPosition) / Time.deltaTime;
        previousPosition = platform.position;

        if (player != null)
        {
            vRigidBody = player.GetComponent<Rigidbody>();
            vRigidBody.AddForce(velocity * 10, ForceMode.Acceleration);
        }
        else
        {
            vRigidBody = null;
        }
    }

    Vector3 GetVelocity()
    {
        return velocity;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(playertag))
        {
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals(playertag))
        {
            player = null;
        }
    }
}
