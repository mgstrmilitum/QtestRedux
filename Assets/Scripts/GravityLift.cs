using UnityEngine;

public class GravityLift : MonoBehaviour
{
    //direction lift applies its force
    [SerializeField] Vector3 liftDirection;
    //strength of lift
    [SerializeField] float forceMagnitude;

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
        if(other.CompareTag("Player"))
        {
            other.GetComponent<CharacterController>().Move(liftDirection * forceMagnitude * Time.deltaTime);
        }
    }
}
