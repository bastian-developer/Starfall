using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private WaveConfigSO waveConfig;
    private List<Transform> _waypoints;
    private int _waypointIndex = 0;
    void Start()
    {
        _waypoints = waveConfig.GetWaypoints();
        transform.position = _waypoints[_waypointIndex].position;
    }
    
    void Update()
    {
        FollowPath();
    }

    void FollowPath()
    {
        if (_waypointIndex < _waypoints.Count)
        {

            Vector3 targetPosition = _waypoints[_waypointIndex].position;
            float delta = waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, delta);
            if (transform.position == targetPosition)
            {
                _waypointIndex++;
            }

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
