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
    private HashSet<EShipOwner> _owners;

    // returns last parsed list of total owners on map
    public HashSet<EShipOwner> Owners { get => _owners; }

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
        _owners = new HashSet<EShipOwner>();
        foreach (var planet in _planets)
            _owners.Add(planet.Owner);
        return !(_owners.Contains(EShipOwner.Player) && _owners.Contains(EShipOwner.Computer));
    }

    private void ComputerTick()
    {           
        int index, reciever, lowestShipsCount = System.Int32.MaxValue, maxShipsCount = 0;
        float shortestDistance = 100000f;
        List<int> indexes;
        if (Resources.instance.Difficulty._easyMode)
        {
            // check every planet - pick random which is not a computer's property
            if (!Resources.instance.IsGameOver)
            {
                // picking random target
                indexes = new List<int>();
                for(int i = 0; i < _planets.Length; i++)
                {
                    if (_planets[i].Owner != EShipOwner.Computer)
                        indexes.Add(i);
                }
                index = indexes[Resources.instance.Rng.Next(0, indexes.Count)];
                indexes = new List<int>();
                // picking random sender planet 
                for (int number = 0; number < _planets.Length; number++)
                {
                    if (_planets[number].Owner == EShipOwner.Computer)
                        indexes.Add(number);
                }
                // go to planet
                _planets[indexes[Resources.instance.Rng.Next(0, indexes.Count)]].GoToPlanet(_planets[index]);
            }
            return;
        }



        // SUPREME HARD KILLER bot
        // checking if bot can even capture something 
        indexes = new List<int>();
        // key - sender planet, value - hashset, value-key - target, value-value - distance
        var distances = new Dictionary<int, List<KeyValuePair<int, float>>>();
        // picking all sender planets
        for (int number = 0; number < _planets.Length; number++)
        {
            if (_planets[number].Owner == EShipOwner.Computer)
                indexes.Add(number);
        }

        // for each sender planet picking all not captured which can be captured with current ships count
        foreach(var element in indexes)
        {
            index = -1;
            var nonCaptured = new List<KeyValuePair<int, float>>();
            // pick all non captured planets
            foreach(var planet in _planets)
            {
                index++;
                // which have less or eq than half of sender's ships
                if (planet.Owner != EShipOwner.None ||
                    planet.Ships > _planets[element].Ships / 2)
                    continue;

                nonCaptured.Add(new KeyValuePair<int, float>
                    (index, _planets[element].DistanceBetween(planet)));
            }
            distances.Add(element, nonCaptured);
        }

        // check every planet - pick closest with less ships count
        index = -1;
        reciever = -1;
        foreach(var element in distances)
        {
            foreach(var hashsetMember in element.Value)
            {
                if (hashsetMember.Value < shortestDistance)
                {
                    shortestDistance = hashsetMember.Value;
                    index = element.Key;
                    reciever = hashsetMember.Key;
                }
            }
        }

        // if index == -1 - do nothing or goto blue planet, 
        // else - send the ships to optimal planet
        if (!(index == -1 || reciever == -1))
        {
            _planets[index].GoToPlanet(_planets[reciever]);
            return;
        }

        // checking blue planets now!
        index = -1;
        reciever = -1;
        // choosing max red planet
        foreach (var element in indexes)
        {
            if (_planets[element].Owner == EShipOwner.Computer &&
                _planets[element].Ships > maxShipsCount)
            {
                maxShipsCount = _planets[element].Ships;
                index = element;
            }
        }
        // choosing min blue planet
        for (int i = 0; i < _planets.Length; i++)
        {
            if (_planets[i].Owner == EShipOwner.Player && 
                _planets[i].Ships < lowestShipsCount)
            {
                lowestShipsCount = _planets[i].Ships;
                reciever = i;
            }
        }
        // sending ships
        if (!(index == -1 || reciever == -1))
        {
            _planets[index].GoToPlanet(_planets[reciever]);
        }
    }
}
