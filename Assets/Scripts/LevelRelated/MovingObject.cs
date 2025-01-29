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

<<<<<<< HEAD:Assets/Scripts/LevelRelated/MovingObject.cs
    private void OnTriggerEnter(Collider other)
=======
    /*private void OnCollisionEnter(Collision collision)
>>>>>>> LightChristinzioBranch:Assets/Scripts/MovingObject.cs
    {
        if (other.gameObject.CompareTag("Player"))
            other.transform.SetParent(transform);
        //NEXT MONTH PLATFORM AND LAVA NEED TO HAVE DIFFERENT SCRIPTS IF WE'RE GOING TO PARENT OBJECT TO PLATFORM
        //Maybe scrap moving platforms for just jump pads?
    }

    private void OnTriggerExit(Collider other)
    {
<<<<<<< HEAD:Assets/Scripts/LevelRelated/MovingObject.cs
        if (other.gameObject.CompareTag("Player"))
            other.transform.SetParent(null);
    }
=======
        collision.transform.SetParent(null);
    }*/
>>>>>>> LightChristinzioBranch:Assets/Scripts/MovingObject.cs
}
