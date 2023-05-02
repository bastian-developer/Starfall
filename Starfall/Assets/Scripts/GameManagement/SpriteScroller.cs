using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement
{
    public class SpriteScroller : MonoBehaviour
    {
        [SerializeField] private Vector2 moveSpeed;
        
        private Material _material;

        private void Awake()
        {
            _material = GetComponent<SpriteRenderer>().material;
        }

        private void Update()
        {
            var offset = moveSpeed * Time.deltaTime;
            _material.mainTextureOffset += offset;
        }
    }
}
