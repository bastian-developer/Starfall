using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class EnemyDropper: MonoBehaviour
    {
        [Serializable] public class ItemDrop
        {
            public GameObject itemPrefab;
            public float lifespan;
            public float dropRate;
        }

        
        public List<ItemDrop> itemDrops;
    
        public void DropItems(GameObject deadCreature)
        {
            foreach (var itemDrop in itemDrops)
            {
                if (Random.Range(0, 100) > itemDrop.dropRate) continue;
                var itemInstance = Instantiate(itemDrop.itemPrefab, deadCreature.transform.position, deadCreature.transform.rotation);
                Destroy(itemInstance.gameObject, itemDrop.lifespan);
            }
        }
    }
}