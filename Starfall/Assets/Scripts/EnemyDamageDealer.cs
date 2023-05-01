using UnityEngine;

public class EnemyDamageDealer : MonoBehaviour
{
    [SerializeField] private int damage;

    public int GetDamage()
    {
        return damage;
    }

    public void Hit()
    {
        if (gameObject.CompareTag("Laser"))
        {
            Destroy(gameObject);
            //Debug.Log("D-Dealer Hit " + gameObject);
        }
        else
        {
            //Debug.Log("D-Dealer Hit Else" + gameObject);

        }
    }
}