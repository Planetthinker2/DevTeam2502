using MathNet.Numerics.RootFinding;
using System.Collections;
using UnityEngine;

public class spawner : MonoBehaviour
{
    enum spawnerType { required, destroyable}
    [SerializeField] spawnerType type;
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
        if(type == spawnerType.required)
        {
            gamemanager.instance.updateGameGoal(numToSpawn);
        }
    }

    // Update is called once per frame
    void Update()
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
        int arrayPos = Random.Range(0, spawnPos.Length);
        Instantiate(objectToSpawn, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        if (type == spawnerType.destroyable)
        {
            gamemanager.instance.updateGameGoal(1);
        }
        spawnCount++;
        spawnTimer = 0;
    }
}