using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameHelper : NetworkBehaviour
{
    public Vector2 MapSize = new Vector2(100, 100);
    private PlayerHelper _playerHelper;
    public  PlayerHelper CurrentPlayer
    {
        get 
        {
            return _playerHelper; 
        }
        set 
        {
            _playerHelper = value;
        }
    }

    public float Speed = 0.3f;
    public int foodCount = 0;
    public int greenThingsCount = 0;

    [Server]
    void Start()
    {
        InvokeRepeating("GenerateFood", 0, Speed);
        InvokeRepeating("GenerateGreenThings", 0, Speed);
    }
    [Server]
    void GenerateFood()
    {
        if (foodCount < 1500)
        {
            int x = UnityEngine.Random.Range(-100, 100);
            int y = UnityEngine.Random.Range(-100, 100);

            Vector2 Target = new Vector2(x, y);

            GameObject point = Instantiate<GameObject>(Resources.Load<GameObject>("Point"));
            point.transform.position = Target;

            NetworkServer.Spawn(point);
            foodCount++;
        }
    }

    [Server]
    void GenerateGreenThings()
    {
        if (greenThingsCount < 20)
        {
            Debug.Log("GameHelper GenerateFood()");
            int x = UnityEngine.Random.Range(-100, 100);
            int y = UnityEngine.Random.Range(-100, 100);

            Vector2 Target = new Vector2(x, y);

            GameObject greenThing = Instantiate<GameObject>(Resources.Load<GameObject>("GreenThing"));
            greenThing.transform.position = Target;

            NetworkServer.Spawn(greenThing);
            greenThingsCount++;
        }
    }


}
