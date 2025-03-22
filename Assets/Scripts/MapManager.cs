using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapManager : MonoBehaviour
{
    [Header("Tuile")]
    public GameObject tilePlain;
    public GameObject tileLake;
    public GameObject tileDesert;

    [Header("Terrain")]
    public GameObject grove;
    public GameObject desertGrove;
    public GameObject forest;
    public GameObject desertForest;
    public GameObject littleWood;
    public GameObject desertLittleWood;
    public GameObject coalDeposit;
    public GameObject oreDeposit;
    public GameObject rocks;
    public GameObject fish;


    public GameObject MapObject;

    private RestAPI restAPI;

    private Vector3 _terrainOffset = new Vector3(0, 1.125f, 0);
    private float _tileWidth = 7f;
    public Tile[,] Map = new Tile[33, 33];
    public GameObject[,] TileMap = new GameObject[33, 33];
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

        restAPI = GetComponent<RestAPI>();
        Vector2Int position = new Vector2Int(28, 10);
        StartCoroutine(restAPI.GetMap(new Vector2Int(position.x - 2, position.x + 3), new Vector2Int(position.y - 2, position.y + 3)));
        //GenerateMap();
    }
    public void GenerateMap()
    {
        foreach(Tile tile in Map)
        {
            if(tile != null)
            {
                Vector2Int pos = tile.GetCoordinates() * 7;
                Vector3 worldPos = new Vector3(pos.x, 0, pos.y);
                TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y] = new GameObject("Tile");
                GameObject biome = new GameObject();
                GameObject terrain = new GameObject();
                switch (tile.GetBiome().GetName())
                {
                    case "Plaine":
                        biome = Instantiate(tilePlain, worldPos, Quaternion.identity);
                        break;
                    case "Lac":
                        biome = Instantiate(tileLake, worldPos, Quaternion.identity);
                        break;
                    case "Désert":
                        biome = Instantiate(tileDesert, worldPos, Quaternion.identity);
                        break;
                }
                biome.transform.SetParent(TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform, false);

                switch (tile.GetTerrain().GetName())
                {
                    case "Bosquet":
                        terrain = Instantiate(grove, worldPos + _terrainOffset, Quaternion.identity);
                        break;
                    case "Desert_bosquet":
                        terrain = Instantiate(desertGrove, worldPos + _terrainOffset, Quaternion.identity);
                        break;
                }
                terrain.transform.SetParent(TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform, false);
            }
        }
    }

}
