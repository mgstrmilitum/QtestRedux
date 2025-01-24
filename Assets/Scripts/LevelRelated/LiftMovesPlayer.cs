using UnityEngine;

public class LiftMovesPlayer : MonoBehaviour
{
    [SerializeField] string playertag = "Player";
    [SerializeField] Transform platform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(playertag))
        {
            other.gameObject.transform.parent = platform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag.Equals(playertag))
        {
            other.gameObject.transform.parent = null;
        }
    }
}
