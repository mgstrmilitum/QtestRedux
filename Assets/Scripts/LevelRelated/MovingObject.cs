using System.Collections;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    enum ObjectType
    {

    }
    [SerializeField] float speed;
    [SerializeField] Vector3 targetPos;
    Vector3 pointA;
    Vector3 pointB;


    float min;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointA = transform.position;
        pointB = targetPos;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Mathf.PingPong(Time.time * speed, 1);
        transform.position = Vector3.Lerp(pointA, pointB, time);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.transform.SetParent(transform);
        //NEXT MONTH PLATFORM AND LAVA NEED TO HAVE DIFFERENT SCRIPTS IF WE'RE GOING TO PARENT OBJECT TO PLATFORM
        //Maybe scrap moving platforms for just jump pads?
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.transform.SetParent(null);
    }
}
