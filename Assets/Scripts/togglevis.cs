using UnityEngine;

public class togglevis : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Togglevis()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    
    }
    public void TurnOn(GameObject gameobject)
    {
        gameobject.SetActive(true);
    }

    public void TurnOff(GameObject gameobject)
    {
        gameobject.SetActive(false);
    }
}
