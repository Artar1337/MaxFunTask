using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EShipOwner
{
    None,
    Player,
    Computer
}

public class Resources : MonoBehaviour
{

    #region Singleton
    public static Resources instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Resources instance error!");
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField]
    private Sprite[] _planetSprites;
    [SerializeField]
    private Sprite[] _backgroundImages;
    [SerializeField]
    private int _planetCount;
    [SerializeField]
    private GameObject _planet;
    [SerializeField]
    private float _minRadius, _maxRadius, _minSpeed, _maxSpeed, _xBorder = 8.8f, _yBorder = 5f;

    private System.Random _rng;
    private Transform _spawnPoint;

    public System.Random Rng { get => _rng; }
    public Sprite[] PlanetSprites { get => _planetSprites; }

    public float GetRandomFloat(float min, float max)
    {
        float range = max - min;
        float sample = (float)Rng.NextDouble();
        return (sample * range) + min;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rng = new System.Random();

        // set random background
        transform.Find("Background").GetComponent<Image>().sprite =
            _backgroundImages[Rng.Next(0, _backgroundImages.Length)];

        // get default spawn point (0;0)
        _spawnPoint =  GameObject.Find("Planets").transform;

        // set borders according to max planet radius
        _xBorder -= _maxRadius;
        _yBorder -= _maxRadius;

        // set some planets
        SpawnPlanets();
    }

    private float DistanceBetweenTwoVectors(Vector2 v1, Vector2 v2)
    {
        return Mathf.Sqrt((v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y));
    }



    private void SpawnPlanets()
    {
        // key - radius, value - coords
        var coords = new Dictionary<float, KeyValuePair<float, float>>();
        int i = 1;
        while(i <= _planetCount)
        {
            while (true)
            {
                float rad = GetRandomFloat(_minRadius, _maxRadius), x, y;
                bool canBeSpawned;

                while (coords.ContainsKey(rad))
                {
                    rad = GetRandomFloat(_minRadius, _maxRadius);
                }

                canBeSpawned = true;
                x = GetRandomFloat(-_xBorder, _xBorder);
                y = GetRandomFloat(-_yBorder, _yBorder);

                foreach (var val in coords)
                {
                    if (DistanceBetweenTwoVectors(new Vector2(x, y),
                        new Vector2(val.Value.Key, val.Value.Value)) - (val.Key + rad) < 0.0001f)
                    {
                        canBeSpawned = false;
                        break;
                    }
                }  

                if (canBeSpawned)
                {
                    coords.Add(rad, new KeyValuePair<float, float>(x, y));
                    PlanetRotation planet = Instantiate(_planet, new Vector3(x, y, _spawnPoint.position.z),
                        Quaternion.identity, _spawnPoint).GetComponent<PlanetRotation>();
                    StartCoroutine(PlanetSortOrderCoroutine(planet, rad, i));
                    break;
                }
            }
            i++;
        }
    }

    private IEnumerator PlanetSortOrderCoroutine(PlanetRotation planet, float rad, int index)
    {
        yield return null;
        planet.RotationSpeed = GetRandomFloat(_minSpeed, _maxSpeed);
        planet.transform.localScale = new Vector3(rad, rad, rad);
        planet.GetComponent<SpriteMask>().frontSortingOrder = index;
        planet.SetSortingFrontLayerIDs(index);
        planet.UpdateSpriteWidth();
    }
}
