using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetStats : MonoBehaviour
{

    private int _ships = 0;
    private EShipOwner _owner = EShipOwner.None;
    private SpriteRenderer _first, _second;
    private Text _text;
    private GameObject _highlight;
    private LineRenderer _lineRenderer;
    private Camera _mainCam;

    private bool _inDragMode = false;

    public int Ships { set => _ships = value; get => _ships; }

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

    public void UpdateShip()
    {
        _text.text = _ships.ToString();

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
    }

    // set planet not highlighted
    private void OnMouseExit()
    {
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

    //end unit drag - if stopped on planet - send units to destination
    private void OnMouseUp()
    {
        _inDragMode = false;
        OnMouseExit();
        // if not the same planet
        if(_lineRenderer.GetPosition(0) != _lineRenderer.GetPosition(1))
        {
            // and if on the planet
        }
        _lineRenderer.SetPosition(1, _lineRenderer.GetPosition(0));
    }

    private void OnMouseUpAsButton()
    {
        _lineRenderer.SetPosition(1, _lineRenderer.GetPosition(0));
    }

}
