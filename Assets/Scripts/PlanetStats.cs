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

    public int Ships { set => _ships = value; get => _ships; }

    // Start is called before the first frame update
    void Start()
    {
        Transform sprites = transform.Find("Sprites");
        _first = sprites.Find("First").GetComponent<SpriteRenderer>();
        _second = sprites.Find("Second").GetComponent<SpriteRenderer>();
        _text = transform.Find("Canvas").Find("Ships").GetComponent<Text>();
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
}
