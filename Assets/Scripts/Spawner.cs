using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;

    float spawnTimer;

    int spawnCount;

    bool startSpawning;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.UpdateGameGoal(numToSpawn);
    }
    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (startSpawning)
        {


            if (spawnCount < numToSpawn && spawnTimer >= timeBetweenSpawns)
            {
                spawn();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    void spawn()
    {

        int spawnInt = Random.Range(0, spawnPos.Length);

        Instantiate(objectToSpawn, spawnPos[spawnInt].position, spawnPos[spawnInt].rotation);
        spawnCount++;
        spawnTimer = 0;
    }
}