using System.Collections;
using UnityEngine;

public class destroyableObject : MonoBehaviour, IDamage
{

    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] Transform[] spawnPos;

    Color colorOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takeDamage(int amount)
    {
        
        HP -= amount;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            Destroy(gameObject);
            if (objectToSpawn != null)
            {
                int arrayPos = Random.Range(0, spawnPos.Length);
                Instantiate(objectToSpawn, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
            }
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
