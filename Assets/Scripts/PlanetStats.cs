using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class PlanetStats : MonoBehaviour
{

    private int _ships = 0;
    private EShipOwner _owner = EShipOwner.None;
    private SpriteRenderer _first, _second;
    private Text _text;
    private GameObject _highlight;
    private LineRenderer _lineRenderer;
    private Camera _mainCam;
    [SerializeField]
    private GameObject _ship;
    private float _radius;

    private bool _inDragMode = false, _onPlanet = false;

    public int Ships { set => _ships = value; get => _ships; }
    public float Radius { set => _radius = value; get => _radius; }

    // Start is called before the first frame update
    void Start()
    {
        Transform sprites = transform.Find("Sprites");
        _first = sprites.Find("First").GetComponent<SpriteRenderer>();
        _second = sprites.Find("Second").GetComponent<SpriteRenderer>();
        _text = transform.Find("Canvas").Find("Ships").GetComponent<Text>();
        _highlight = transform.Find("Highlight").gameObject;
        _lineRenderer = transform.Find("LineRenderer").GetComponent<LineRenderer>();
        _mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        // set line position to zero (center of the planet)
        _lineRenderer.SetPositions(new Vector3[] {
            new Vector3(transform.position.x, transform.position.y, 0),
            new Vector3(transform.position.x, transform.position.y, 0)
        });

        InvokeRepeating(nameof(UpdateShip), 1, 1);
    }

    private void FixedUpdate()
    {
        _text.text = _ships.ToString();
    }

    public void UpdateShip()
    {
        if(_owner == EShipOwner.None)
        {
            return;
        }

        _ships += Resources.instance.ShipsAddition;
    }

    public void SetPlanetOwner(EShipOwner to)
    {
        _owner = to;

        if (_owner == EShipOwner.Player)
        {
            _first.color = Resources.instance.PlayerColor;
            _second.color = Resources.instance.PlayerColor;
        }
        else if (_owner == EShipOwner.Computer)
        {
            _first.color = Resources.instance.ComputerColor;
            _second.color = Resources.instance.ComputerColor;
        }
    }

    // set planet highlighted
    private void OnMouseEnter()
    {
        _highlight.SetActive(true);
        _onPlanet = true;
        Resources.instance.LastLightedPlanet = this;
    }

    // set planet not highlighted
    private void OnMouseExit()
    {
        _onPlanet = false;
        if (!_inDragMode)
            _highlight.SetActive(false);
    }

    // start unit drag
    private void OnMouseDrag()
    {
        if (_owner != EShipOwner.Player)
        {
            return;
        }
        _inDragMode = true;
        Vector3 pos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        _lineRenderer.SetPosition(1, new Vector3(pos.x, pos.y, 0));
    }

    private bool IsPointTouchingObject(Vector2 point, GameObject obj)
    {
        return obj.GetComponent<Collider2D>().OverlapPoint(point);
    }

    private bool VectorsAreEqual(Vector3 v1, Vector3 v2, double EPSILON = 0.0000001)
    {
        return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y) + Mathf.Abs(v1.z - v2.z) < EPSILON;
    }

    //end unit drag - if stopped on planet - send units to destination
    private void OnMouseUp()
    {
        _inDragMode = false;
        if (_owner != EShipOwner.Player)
        {
            return;
        }
        // if not the same planet
        // and if on the planet which is not a player's planet btw
        if (Resources.instance.LastLightedPlanet._onPlanet && 
            Resources.instance.LastLightedPlanet._owner != EShipOwner.Player)
        {
            //spawn 50% of the total ship count
            int count = _ships / 2;
            float x, y;
            _ships -= count;
            for(int i = 0; i < count; i++)
            {
                x = Resources.instance.GetRandomFloat(0, Radius);
                if (Resources.instance.Rng.Next() % 2 > 0)
                    x = -x;
                y = Resources.instance.GetRandomFloat(0, Radius);
                if (Resources.instance.Rng.Next() % 2 > 0)
                    y = -y;

                Instantiate(_ship, new Vector3(
                    transform.position.x + x,
                    transform.position.y + y,
                    transform.position.z), Quaternion.identity).
                    GetComponent<ShipAI>().SetStats(this, 
                    Resources.instance.LastLightedPlanet, Resources.instance.LastLightedPlanet.Radius);
            }
        }
        OnMouseExit();
        _lineRenderer.SetPosition(1, _lineRenderer.GetPosition(0));
    }

    public void OnPathComplete(PlanetStats target, GameObject ship, PlanetStats sender)
    {
        Destroy(ship);
        if(target._owner == sender._owner)
        {
            target._ships++;
            return;
        }

        target._ships--;
        // planet have been captured
        if (target._ships <= 0)
        {
            target.SetPlanetOwner(sender._owner);
        }
    }
}
