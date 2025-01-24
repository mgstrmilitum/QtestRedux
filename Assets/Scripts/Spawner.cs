using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;

    int spawnCount;

    bool startSpawning;
    float spawnTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.UpdateGameGoal(numToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        if(startSpawning)
        {
            spawnTimer += Time.deltaTime;

            if(spawnCount < numToSpawn && spawnTimer > timeBetweenSpawns)
            {
                Spawn();
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

    void Spawn()
    {
        int spawnInt = Random.Range(0, spawnPos.Length);

        spawnTimer = 0;
        Instantiate(objectToSpawn, spawnPos[spawnInt].position, spawnPos[spawnInt].rotation);
        spawnCount++;
    }
}
