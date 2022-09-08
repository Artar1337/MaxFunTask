using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ShipAI : MonoBehaviour
{
    private bool _ended = false;
    private Path _path;
    private PlanetStats _sender, _target;
    private int _currentWaypoint = 0;
    private float _nextWaypointDistance = 0.1f;
    private Seeker _seeker;

    private void Start()
    {
        _seeker = GetComponent<Seeker>();
    }

    public void SetStats(PlanetStats sender, PlanetStats target, float distance)
    {
        _sender = sender;
        _target = target;
        _nextWaypointDistance = distance * 0.5f;
        if(_seeker == null)
            _seeker = GetComponent<Seeker>();
        _seeker.StartPath(transform.position, _target.transform.position, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_path == null)
            return;

        if(_currentWaypoint >= _path.vectorPath.Count)
        {
            _ended = true;
            _sender.OnPathComplete(_target, gameObject, _sender);
            return;
        }
        _ended = false;

        if (Vector2.Distance(transform.position, _path.vectorPath[_currentWaypoint]) < _nextWaypointDistance)
        {
            _currentWaypoint++;
        }
    }
}
