using UnityEngine;

public class damage : MonoBehaviour
{

    enum damageType { moving, stationary, depleting }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    void Start()
    {
        if (type == damageType.moving)
        {
            rb.linearVelocity = (gamemanager.instance.player.transform.position - transform.position).normalized * speed; // Shoot in direciton of player

            Destroy(gameObject, destroyTime);
        }

        if(type == damageType.depleting)
        {
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(damageAmount);
        }

        if (type == damageType.moving)
        {
            Destroy(gameObject);
        }
    }
}
