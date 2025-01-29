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
                model.SetActive(false);
                GameManager.Instance.interactButton.SetActive(false);
                GameManager.Instance.buttonInfo.text = buttonInfo;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        inTrigger = true;

        if (other.isTrigger)
        {
            return;
        }

        IOpen open = other.GetComponent<IOpen>();

        if (open != null)
        {
            GameManager.Instance.interactButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        inTrigger = false;

        if (other.isTrigger)
        {
            return;
        }

        IOpen open = other.GetComponent<IOpen>();

        if (open != null)
        {
            GameManager.Instance.interactButton.SetActive(false);
            GameManager.Instance.buttonInfo.text = null;
        }
    }
}
