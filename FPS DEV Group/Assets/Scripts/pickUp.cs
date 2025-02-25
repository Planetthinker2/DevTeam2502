using UnityEngine;

public class pickUp : MonoBehaviour
{

    [SerializeField] gunStats gun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickup pick = other.GetComponent<IPickup>();
        if(pick != null)
        {
            pick.getGunStats(gun);
            Destroy(gameObject);
        }
    }

}
