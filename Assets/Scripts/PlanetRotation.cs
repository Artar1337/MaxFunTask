using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed;

    private int _currentIndex;
    private RectTransform _first, _second, _sprites;
    private float _spriteWidth;
    private bool _onFirstSprite = true;

    public float RotationSpeed { get => _rotationSpeed; set => _rotationSpeed = value; }

    // Start is called before the first frame update
    void Start()
    {
        _currentIndex = Resources.instance.Rng.Next(0, Resources.instance.PlanetSprites.Length);
        _sprites = transform.Find("Sprites").GetComponent<RectTransform>();
        _first = _sprites.Find("First").GetComponent<RectTransform>();
        _first.GetComponent<SpriteRenderer>().sprite = Resources.instance.PlanetSprites[_currentIndex];
        UpdateSpriteWidth();
        _second = _sprites.Find("Second").GetComponent<RectTransform>();
        _second.GetComponent<SpriteRenderer>().sprite = Resources.instance.PlanetSprites[_currentIndex];
        _second.position = new Vector3(_first.position.x + _spriteWidth, _first.position.y);
    }

    // set sorting id
    public void SetSortingFrontLayerIDs(int to)
    {
        _second.GetComponent<SpriteRenderer>().sortingOrder = to;
        _first.GetComponent<SpriteRenderer>().sortingOrder = to;
    }

    public void UpdateSpriteWidth()
    {
        _spriteWidth = _first.GetComponent<SpriteRenderer>().bounds.extents.x * 2;
    }

    void FixedUpdate()
    {
        _sprites.position = new Vector3(_sprites.position.x - _rotationSpeed * Time.fixedDeltaTime,
            _sprites.position.y, _sprites.position.z);

        // if currently on first - then left sprite is first, right is second
        if (_onFirstSprite)
        {
            // if gotten to a center of a next sprite - moving other
            if (-_sprites.localPosition.x > _second.localPosition.x)
            {
                _first.position = new Vector3(_first.position.x + 2 * _spriteWidth, _first.position.y, _first.position.z);
                _onFirstSprite = !_onFirstSprite;
            }
        }
        // else - left sprite is second, right is first
        else
        {
            // if gotten to a center of a next sprite - moving other
            if( -_sprites.localPosition.x > _first.localPosition.x)
            {
                _second.position = new Vector3(_second.position.x + 2 * _spriteWidth, _second.position.y, _second.position.z);
                _onFirstSprite = !_onFirstSprite;
            }
        }
    }
}
