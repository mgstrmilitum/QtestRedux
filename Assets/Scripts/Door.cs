using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject model;
    [SerializeField] string buttonInfo;

    bool inTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger)
        {
            if(Input.GetButtonDown("Interact"))
            {

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IOpen open = other.GetComponent<IOpen>();

        if (open != null)
        {

        }
    }
}
