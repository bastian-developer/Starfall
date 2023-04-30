using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class EnemyDropper: MonoBehaviour
    {
        // Class that defines the structure of the items
        [Serializable] public class ItemDrop
        {
            public GameObject itemPrefab;
            public float lifespan;
            public float dropRate;
        }
        
        // Public list that stores every item
        [SerializeField] public List<ItemDrop> itemDrops;
        // Speed the item will take when spawned
        [SerializeField] private float itemSpeed;

        // This method drops items from the list of itemDrops with the specified
        // Drop rate when a dead creature game object is passed in
        public void DropItems(GameObject deadCreature)
        {
            // Loop through all the item drops in the list
            foreach (var itemDrop in itemDrops)
            {
                // If the item drop's drop rate is less than a random value between 0 and 100, skip it
                if (Random.Range(0, 100) > itemDrop.dropRate) continue;
                // Instantiate the item prefab at the position and rotation of the dead creature game object
                var itemInstance = Instantiate(itemDrop.itemPrefab, deadCreature.transform.position, deadCreature.transform.rotation);
                // If the item instance has a Rigidbody2D component, set its velocity to a random direction with the specified speed
                if (!itemInstance.TryGetComponent(out Rigidbody2D itemRigidbody)) return;
                itemRigidbody.velocity = Random.insideUnitCircle.normalized * itemSpeed;
                // Destroy the item instance after the specified lifespan
                Destroy(itemInstance.gameObject, itemDrop.lifespan);
            }
        }
    }
}