using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private float _minRadius, _maxRadius, _minSpeed, _maxSpeed;

    private System.Random _rng;

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

        // set some planets
        SpawnPlanets();
    }

    private void SpawnPlanets()
    {
        // key - radius, value - coords
        var coords = new Dictionary<float, KeyValuePair<float, float>>();
        
        for(int i = 0; i < _planetCount; i++)
        {
            float rad = GetRandomFloat(_minRadius, _maxRadius);

            foreach(var val in coords)
            {

            }
        }
    }
}
