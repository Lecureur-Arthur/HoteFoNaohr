using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject tilePlain;
    public GameObject tileLake;
    public GameObject tileDesert;

    private RestAPI restAPI;

    private float _tileWidth = 7f;
    [SerializeField] public Tile[,] Map = new Tile[33, 33];
    public static MapManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void GenerateMap(Vector2Int position)
    {
        StartCoroutine(restAPI.GetMap(new Vector2Int(position.x - 3, position.x + 3), new Vector2Int(position.y - 3, position.y + 3)));

        foreach(Tile tile in Map)
        {
            if(tile != null)
            {
                switch(tile.GetBiome().GetName())
                {
                    case "Plaine":
                        break;
                    case "Lac":
                        break;
                    case "Désert":
                        break;
                }
            }
        }
    }

}
