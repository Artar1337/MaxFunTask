using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    #region Singleton
    public static EnemyController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Enemy instance error!");
            return;
        }
        instance = this;
    }
    #endregion

    private PlanetStats[] _planets;

    // Start is called before the first frame update
    void Start()
    {
        _planets = GameObject.Find("Planets").GetComponentsInChildren<PlanetStats>();
        InvokeRepeating(nameof(ComputerTick),
            Resources.instance.Difficulty._reactionTime, Resources.instance.Difficulty._reactionTime);
    }

    public bool CheckIfGameIsOver()
    {
        if (Resources.instance.IsGameOver)
            return true;
        HashSet<EShipOwner> owners = new HashSet<EShipOwner>();
        foreach (var planet in _planets)
            owners.Add(planet.Owner);
        return !(owners.Contains(EShipOwner.Player) && owners.Contains(EShipOwner.Computer));
    }

    private void ComputerTick()
    {           
        int index;
        if (Resources.instance.Difficulty._easyMode)
        {
            // check every planet - pick random which is not a computer's property
            while (!Resources.instance.IsGameOver)
            {
                // picking random target
                index = Resources.instance.Rng.Next(0, _planets.Length);
                if(_planets[index].Owner != EShipOwner.Computer)
                {
                    List<int> indexes = new List<int>();
                    // picking random sender planet 
                    for (int number = 0; number < _planets.Length; number++)
                    {
                        if (_planets[number].Owner == EShipOwner.Computer)
                            indexes.Add(number);
                    }
                    // go to planet
                    _planets[indexes[Resources.instance.Rng.Next(0, indexes.Count)]].GoToPlanet(_planets[index]);
                    break;
                }
            }
            return;
        }
        // checking if bot can even capture something 
        // check every planet - pick closest with less ships count
    }
}
