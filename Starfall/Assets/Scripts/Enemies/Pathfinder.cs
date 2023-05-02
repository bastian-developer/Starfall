using System.Collections.Generic;
using UnityEngine;
using Characters;

namespace Enemies
{
    public class Pathfinder : MonoBehaviour
    {
        private EnemySpawner _enemySpawner;
        private Player _player;
        private WaveConfigSO _waveConfig;
        private List<Transform> _waypoints;
        private int _waypointIndex = 0;
    
        [SerializeField] private float rotationModifier;
        [SerializeField] private float rotationSpeed;

        private void Awake()
        {
            _enemySpawner = FindObjectOfType<EnemySpawner>();
        
        }

        private void Start()
        {
            _waveConfig = _enemySpawner.GetCurrentWave();
            _waypoints = _waveConfig.GetWaypoints();
            transform.position = _waypoints[_waypointIndex].position;
            _player = FindObjectOfType<Player>();
        }
    
        private void Update()
        {
            FollowPath();
            RotateTowardsPlayer();
        }
    
        private void RotateTowardsPlayer()
        {
            if (!_player) return;
            Vector3 playerPosition = _player.transform.position;
            Vector3 vectorToTarget = playerPosition - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotationModifier;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationSpeed);

        }

        private void FollowPath()
        {
            if (_waypointIndex < _waypoints.Count)
            {
                var targetPosition = _waypoints[_waypointIndex].position;
                var delta = _waveConfig.GetMoveSpeed() * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, delta);
                if (transform.position == targetPosition)
                {
                    _waypointIndex++;
                }
            }
            else
            {
                //_waypointIndex--;
                //Debug.Log(gameObject);
                Destroy(gameObject);
            }
        }
    }

}