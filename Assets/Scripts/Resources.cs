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
    private int _planetCount;
    [SerializeField]
    private GameObject _planet;
    [SerializeField]
    private float _minRadius, _maxRadius, _minSpeed, _maxSpeed, _xBorder = 8.8f, _yBorder = 5f;
    [SerializeField]
    private int _shipsAddition = 5, _minStartShipsCount = 1, _maxStartShipsCount = 50;
    [SerializeField]
    private Color _playerColor, _computerColor;
    [SerializeField]
    private ScriptableDifficulty _difficulty;

    public ScriptableDifficulty Difficulty { get => _difficulty; }
    public PlanetStats LastLightedPlanet { get => _lastLightedPlanet; set => _lastLightedPlanet = value; }
    public int ShipsAddition { get => _shipsAddition; }
    public Color PlayerColor { get => _playerColor; }
    public Color ComputerColor { get => _computerColor; }
    public KeyValuePair<float, float> Border { get => new KeyValuePair<float, float>(_xBorder + _maxRadius, _yBorder + _maxRadius); }
    public bool IsGameOver { get => _gameOver; set => _gameOver = value; }

    private bool _gameOver = false, _gameOverMessageShown = false;
    private System.Random _rng;
    private Transform _spawnPoint;
    private PlanetStats _lastLightedPlanet;
    private Dictionary<float, KeyValuePair<float, float>> _planetsInfo;

    public System.Random Rng { get => _rng; }
    public Sprite[] PlanetSprites { get => _planetSprites; }

    public Dictionary<float, KeyValuePair<float, float>> Planets { get => _planetsInfo; }

    public float GetRandomFloat(float min, float max)
    {
        float range = max - min;
        float sample = (float)Rng.NextDouble();
        return (sample * range) + min;
    }

    // Start is called before the first frame update
    void Start()
    {
        _planetCount = _difficulty._planetsCount;
        _rng = new System.Random();

        // set random background
        GameObject.Find("Background").GetComponent<SpriteRenderer>().sprite =
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
        // remember coords
        _planetsInfo = coords;
        // all planets spawned - wait for 2 frames than build a A* graph according to planets
        StartCoroutine(PathFindingCoroutine());
    }

    private IEnumerator PathFindingCoroutine()
    {
        yield return null; 
        yield return null;
        GameObject.Find("Pathfinding Manager").GetComponent<AstarPath>().Scan();
    }

    private IEnumerator PlanetSortOrderCoroutine(PlanetRotation planet, float rad, int index)
    {
        yield return null;
        planet.RotationSpeed = GetRandomFloat(_minSpeed, _maxSpeed);
        planet.transform.localScale = new Vector3(rad, rad, rad);
        planet.GetComponent<SpriteMask>().frontSortingOrder = index;
        planet.SetSortingFrontLayerIDs(index);
        planet.UpdateSpriteWidth();
        PlanetStats stats = planet.GetComponent<PlanetStats>();
        stats.Radius = rad;
        if (index == 1)
        {
            stats.SetPlanetOwner(EShipOwner.Player);
            stats.Ships = _maxStartShipsCount;
        }  
        else if (index == 2)
        {
            stats.SetPlanetOwner(EShipOwner.Computer);
            stats.Ships = _maxStartShipsCount;
        }
        else
        {
            stats.Ships = Rng.Next(_minStartShipsCount, _maxStartShipsCount);
        }
        stats.UpdateShip();
    }

    public void GameOver()
    {
        if (_gameOverMessageShown)
            return;

        Debug.Log("GAME OVER lol");

        _gameOverMessageShown = true;
    }
}
