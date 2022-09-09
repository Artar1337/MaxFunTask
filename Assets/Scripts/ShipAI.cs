using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ShipAI : MonoBehaviour
{
    private Path _path;
    private PlanetStats _sender, _target;
    private int _currentWaypoint = 0;
    [SerializeField]
    private float _nextWaypointDistance = 0.05f;
    private Seeker _seeker;
    private SpriteRenderer _rend;
    private EShipOwner _owner = EShipOwner.None;

    public EShipOwner Owner { get => _owner; }

    private void Start()
    {
        _seeker = GetComponent<Seeker>();
        _rend = transform.Find("Graphics").GetComponent<SpriteRenderer>();
    }

    public void SetStats(PlanetStats sender, PlanetStats target, Color shipCol, EShipOwner owner)
    {
        _sender = sender;
        _target = target;
        _owner = owner;
        if (_seeker == null || _rend == null)
            Start();
        _rend.color = shipCol;
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

        // path ended
        if(_currentWaypoint >= _path.vectorPath.Count)
        {
            _sender.OnPathComplete(_target, gameObject, _sender.Owner);
            return;
        }

        if (Vector2.Distance(transform.position, _path.vectorPath[_currentWaypoint]) < _nextWaypointDistance)
        {
            _currentWaypoint++;
        }
    }
}
