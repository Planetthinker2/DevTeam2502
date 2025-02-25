using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{

    [SerializeField] Renderer model;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gamemanager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(flasColor());
        }
    }

    IEnumerator flasColor()
    {
        model.material.color = Color.red;
        gamemanager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        model.material.color = Color.white;
        gamemanager.instance.checkpointPopup.SetActive(false);
    }


}
