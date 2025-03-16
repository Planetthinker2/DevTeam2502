using UnityEngine;

public class EnemyHighlight : MonoBehaviour
{
    private Light enemyLight;

    public void SetEnemyLight(Light light)
    {
        enemyLight = light;
    }

    private void OnDestroy()
    {
        if (enemyLight != null)
        {
            Destroy(enemyLight.gameObject);
        }
    }
}
