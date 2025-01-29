using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] float spawnRate;
    [SerializeField] Transform[] spawnPoints;

    float spawnTimer;
    int spawnCount;

    bool startSpawning;

    void Start()
    {
        spawnTimer = 0f;
        GameManager.Instance.UpdateGameGoal(numToSpawn);
    }

    void Update()
    {
        if(startSpawning)
        {
            spawnTimer += Time.deltaTime;

            if(spawnCount < numToSpawn && spawnTimer >= spawnRate)
            {
                Spawn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    void Spawn()
    {
        int spawnInt = Random.Range(0, spawnPoints.Length);

        Instantiate(objectToSpawn, spawnPoints[spawnInt].position, spawnPoints[spawnInt].rotation);
        spawnCount++;
        spawnTimer = 0f;
    }
}
