using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class DamageDealer : MonoBehaviour
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
}
