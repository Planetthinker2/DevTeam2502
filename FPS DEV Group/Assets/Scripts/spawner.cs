using MathNet.Numerics.RootFinding;
using System.Collections;
using UnityEngine;

public class spawner : MonoBehaviour, IDamage
{
    enum spawnerType { required, destroyable}
    [SerializeField] spawnerType type;
    [SerializeField] Renderer model;
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] int HP;


    float spawnTimer;
    int spawnCount;

    Color colorOrig;

    bool startSpawning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(type == spawnerType.required)
        {
            gamemanager.instance.updateGameGoal(numToSpawn);
        }
        colorOrig = model.material.color;
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
        spawnCount++;
        spawnTimer = 0;
    }

    public void takeDamage(int amount)
    {
        if(type == spawnerType.required)
        {
            return;
        }
        HP -= amount;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

}