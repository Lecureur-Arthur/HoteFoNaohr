using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing.MiniJSON;
using UnityEngine.UIElements;
using UnityEngine.XR;

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

    [Header("Batiment")]
    public GameObject coal_mining_workshop;
    public GameObject stone_cutting_workshop;
    public GameObject ship;
    public GameObject library;
    public GameObject logging_hut;
    public GameObject capitol;
    public GameObject quarry;
    public GameObject biomass_plant;
    public GameObject methane_power_plant;
    public GameObject coal_power_plant;
    public GameObject castle;
    public GameObject windMill;
    public GameObject iron_excavator;
    public GameObject farm;
    public GameObject high_library;
    public GameObject high_statue;
    public GameObject forest_plant;
    public GameObject coal_mining;
    public GameObject iron_mining;
    public GameObject mill;
    public GameObject museum;
    public GameObject observatory;
    public GameObject port;
    public GameObject carbon_well;
    public GameObject nuclear_fusion_reactor;
    public GameObject sawMill;
    public GameObject theater;
    public GameObject guet_tower;
    public GameObject hydro_turbine;
    public GameObject renewal_plant;
    public GameObject build_zone;

    [Header("NPC")]
    public GameObject villagerPrefab;

    [Header("UI")]
    public TextMeshProUGUI woodValue;
    public TextMeshProUGUI coalValue;
    public TextMeshProUGUI ironValue;
    public TextMeshProUGUI foodValue;
    public TextMeshProUGUI rockValue;
    public TextMeshProUGUI pollutionValue;
    public TextMeshProUGUI energieValue;


    [Header("Misc")]
    public GameObject MapObject;
    public float maxPostTimer = 12.5f;
    private float postTimer = 0f;

    public float maxGetTimer = 6f;
    public float getTimer = 0f;

    private RestAPI restAPI;

    public List<Villager> Villagers;
    public Villager VillagerById;
    public float energy;
    public float pollution;

    public Dictionary<Resource, int> activeResources = new Dictionary<Resource, int>();

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
        StartCoroutine(restAPI.GetTeamNpcWithIdEquipe());
        //GenerateMap();
        postTimer = maxPostTimer;
        getTimer = maxGetTimer;
    }

    private void Update()
    {
        postTimer -= Time.deltaTime;
        getTimer -= Time.deltaTime;

        if (postTimer <= 0)
        {
            foreach (Villager villager in Villagers)
            {
                if(villager != null && Map[villager.GetPosition().x, villager.GetPosition().y] != null)
                {
                    Dictionary<Resource, int> resources = Map[villager.GetPosition().x, villager.GetPosition().y].GetResources();
                    if (resources != null)
                    {
                        foreach (Resource resource in resources.Keys)
                        {
                            if (resources[resource] <= 250)
                            {
                                int rand = Random.Range(0, 4);
                                Debug.Log(rand);
                                switch (rand)
                                {
                                    case 0:
                                        restAPI.CastActionAsync(villager.GetId(), "DEPLACEMENT_BAS");
                                        break;
                                    case 1:
                                        restAPI.CastActionAsync(villager.GetId(), "DEPLACEMENT_DROITE");
                                        break;
                                    case 2:
                                        restAPI.CastActionAsync(villager.GetId(), "DEPLACEMENT_HAUT");
                                        break;
                                    case 3:
                                        restAPI.CastActionAsync(villager.GetId(), "DEPLACEMENT_GAUCHE");
                                        break;
                                }
                            }
                            else if (resources[resource] > 0)
                            {
                                restAPI.CastActionAsync(villager.GetId(), "RECOLTER", resource.GetName());
                                StartCoroutine(restAPI.GetTeam());
                            }
                        }
                        StartCoroutine(restAPI.GetMap(new Vector2Int(villager.GetPosition().x - 3, villager.GetPosition().x + 2),
                            new Vector2Int(villager.GetPosition().y - 3, villager.GetPosition().y + 2)));
                    }
                }
            }
            postTimer = maxPostTimer;
        }

        if (getTimer <= 0)
        {
            StartCoroutine(restAPI.GetTeam());
            getTimer = maxGetTimer;
        }

    }


    public void UpdateUI()
    {
        foreach(Resource res in activeResources.Keys)
        {
            switch(res.GetName())
            {
                case "BOIS":
                    woodValue.text = activeResources[res].ToString();
                    break;
                case "FER":
                    ironValue.text = activeResources[res].ToString();
                    break;
                case "PIERRE":
                    rockValue.text = activeResources[res].ToString();
                    break;
                case "CHARBON":
                    coalValue.text = activeResources[res].ToString();
                    break;
                case "NOURRITURE":
                    foodValue.text = activeResources[res].ToString();
                    break;
                case "ENERGIE":
                    energieValue.text = activeResources[res].ToString();
                    break;
                case "POLLUTION":
                    pollutionValue.text = activeResources[res].ToString();
                    break;
            }
        }

    }

    public void GenerateMap()
    {
        foreach(Tile tile in Map)
        {
            //Debug.LogWarning("TILE " + tile?.GetBuiltBuilding());

            if(tile != null)
            {
                tile.existe = true;
                Vector2Int pos = tile.GetCoordinates() * 7;
                Vector3 worldPos = new Vector3(pos.x, 0, pos.y);
                if(TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y] == null)
                    TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y] = new GameObject("Tile");
                TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform.SetParent(MapObject.transform, false);

                GameObject biome = new GameObject("Biome");
                GameObject terrain = new GameObject("Terrain");
                GameObject building = new GameObject("Batiment");

                Transform currentBiome = TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform.Find("Biome");
                Transform currentTerrain = TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform.Find("Terrain");
                Transform currentBuilding = TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform.Find("Batiment");

                if (currentBiome != null)
                {
                    switch (tile.GetBiome().GetName())
                    {
                        case "Plaine":
                            biome = Instantiate(tilePlain, worldPos + tilePlain.transform.position, Quaternion.identity);
                            break;
                        case "Lac":
                            biome = Instantiate(tileLake, worldPos + tileLake.transform.position, Quaternion.identity);
                            break;
                        case "Désert":
                            biome = Instantiate(tileDesert, worldPos + tileDesert.transform.position, Quaternion.identity);
                            break;
                    }
                }

                if (currentTerrain != null)
                {
                    switch (tile.GetTerrain().GetName())
                    {
                        case "Bosquet":
                            terrain = Instantiate(grove, worldPos + grove.transform.position, Quaternion.identity);
                            break;
                        case "Desert_bosquet":
                            terrain = Instantiate(desertGrove, worldPos + desertGrove.transform.position, Quaternion.identity);
                            break;
                        case "Desert_foret":
                            terrain = Instantiate(desertForest, worldPos + desertForest.transform.position, Quaternion.identity);
                            break;
                        case "Desert_petit_bois_et_amas_rocheux":
                            terrain = Instantiate(desertLittleWood, worldPos + desertLittleWood.transform.position, Quaternion.identity);
                            break;
                        case "Foret":
                            terrain = Instantiate(forest, worldPos + forest.transform.position, Quaternion.identity);
                            break;
                        case "Gisement_de_charbon":
                            terrain = Instantiate(coalDeposit, worldPos + coalDeposit.transform.position, Quaternion.identity);
                            break;
                        case "Gisement_de_minerais":
                            terrain = Instantiate(oreDeposit, worldPos + oreDeposit.transform.position, Quaternion.identity);
                            break;
                        case "Petit_bois_et_amas_rocheux":
                            terrain = Instantiate(littleWood, worldPos + littleWood.transform.position, Quaternion.identity);
                            break;
                        case "Poisson":
                            terrain = Instantiate(fish, worldPos + fish.transform.position, Quaternion.identity);
                            break;
                        case "Rochers":
                            terrain = Instantiate(rocks, worldPos + rocks.transform.position, Quaternion.identity);
                            break;
                    }

                }



                if (currentBuilding != null)
                {
                    Destroy(currentBuilding.gameObject);
                    switch (tile.GetBuiltBuilding()?.GetBuild().GetBuildType())
                    {
                        case "CABANE_DE_BUCHERON":
                            Debug.LogWarning("Cabane de buche");
                            Instantiate(logging_hut, worldPos + logging_hut.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "SCIERIE":
                            Instantiate(sawMill, worldPos + sawMill.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "ATELIER_DE_TAILLE_DE_PIERRE":
                            Instantiate(stone_cutting_workshop, worldPos + stone_cutting_workshop.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "CARRIERE":
                            Instantiate(quarry, worldPos + quarry.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "EXCAVATRICE_A_FER":
                            Instantiate(iron_excavator, worldPos + iron_excavator.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "MINE_DE_FER":
                            Instantiate(iron_mining, worldPos + iron_mining.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "ATELIER_DE_CHARBONNIER":
                            Instantiate(coal_mining_workshop, worldPos + coal_mining_workshop.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "MINE_DE_CHARBON":
                            Instantiate(coal_mining, worldPos + coal_mining.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "MOULIN":
                            Instantiate(mill, worldPos + mill.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "FERME":
                            Instantiate(farm, worldPos + farm.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "PORT":
                            Instantiate(port, worldPos + port.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "EOLIENNE":
                            Instantiate(windMill, worldPos + windMill.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "CENTRALE_ELECTRIQUE_AU_CHARBON":
                            Instantiate(coal_power_plant, worldPos + coal_power_plant.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "CENTRALE_AU_METHANE":
                            Instantiate(methane_power_plant, worldPos + methane_power_plant.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "CENTRALE_A_BIOMASSE":
                            Instantiate(biomass_plant, worldPos + biomass_plant.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "TURBINE_HYDRAULIQUE":
                            Instantiate(hydro_turbine, worldPos + hydro_turbine.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "INSTALLATION_FORESTIERE":
                            Instantiate(forest_plant, worldPos + forest_plant.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "USINE_DE_RENOUVELLEMENT":
                            Instantiate(renewal_plant, worldPos + renewal_plant.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "PUITS_DE_CARBONE":
                            Instantiate(carbon_well, worldPos + carbon_well.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "TOUR_DE_GUET":
                            Instantiate(guet_tower, worldPos + guet_tower.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "OBSERVATOIRE":
                            Instantiate(observatory, worldPos + observatory.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "MUSEE":
                            Instantiate(museum, worldPos + museum.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "BIBLIOTHEQUE":
                            Instantiate(library, worldPos + library.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "THEATRE":
                            Instantiate(theater, worldPos + theater.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "GRANDE_STATUE":
                            Instantiate(high_statue, worldPos + high_statue.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "CAPITOLE":
                            Instantiate(capitol, worldPos + capitol.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "BATEAU_DE_CROISIERE":
                            Instantiate(ship, worldPos + ship.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "GRANDE_BIBLIOTHEQUE":
                            Instantiate(high_library, worldPos + high_library.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "CHATEAU":
                            Instantiate(castle, worldPos + castle.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                        case "REACTEUR_A_FUSION_NUCLEAIRE ":
                            Instantiate(nuclear_fusion_reactor, worldPos + nuclear_fusion_reactor.transform.position, Quaternion.identity).transform.SetParent(building.transform, false);
                            break;
                    }
                }


                if (TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y] != null)
                {
                    biome.transform.SetParent(TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform, false);
                    terrain.transform.SetParent(TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform, false);
                    building.transform.SetParent(TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform, false);
                }
                else
                {
                    Destroy(biome);
                    Destroy(terrain);
                    Destroy(building);
                }
            }
        }
    }

    public void DisplayVillagers()
    {

        if(Villagers != null)
        {
            foreach (Villager villager in Villagers)
            {
                Vector2Int pos = villager.GetPosition() * 7;
                Vector3 worldPos = new Vector3(pos.x, 0, pos.y);

                Instantiate(villagerPrefab, worldPos + villagerPrefab.transform.position, Quaternion.identity);

                StartCoroutine(restAPI.GetMap(new Vector2Int(villager.GetPosition().x - 3, villager.GetPosition().x + 2), 
                    new Vector2Int(villager.GetPosition().y - 3, villager.GetPosition().y + 2)));
                
            }
        }

    }


    public void CreateLoggingHut()
    {
        foreach(Villager villager in Villagers)
        {
            if (Map[villager.GetPosition().x, villager.GetPosition().y].IsAccessible())
            {
                //Construire
                UnityWebRequestAsyncOperation req = restAPI.CastActionAsync(villager.GetId(), "CONSTRUIRE", "CABANE_DE_BUCHERON");
                if(req.isDone)
                    StartCoroutine(restAPI.GetMap(new Vector2Int(villager.GetPosition().x - 3, villager.GetPosition().x + 2),
                    new Vector2Int(villager.GetPosition().y - 3, villager.GetPosition().y + 2)));
                return;
            }
        }
    }

    public void CreateStoneCuttingWorkshop()
    {
        foreach (Villager villager in Villagers)
        {
            if (Map[villager.GetPosition().x, villager.GetPosition().y].IsAccessible())
            {
                //Construire
                restAPI.CastActionAsync(villager.GetId(), "COMMENCER_CONSTRUCTION", "ATELIER_DE_TAILLE_DE_PIERRE");
                StartCoroutine(restAPI.GetMap(new Vector2Int(villager.GetPosition().x - 3, villager.GetPosition().x + 2),
                    new Vector2Int(villager.GetPosition().y - 3, villager.GetPosition().y + 2)));
                return;
            }
        }
    }

    public void CreateCoalMiningWorkshop()
    {
        foreach (Villager villager in Villagers)
        {
            if (Map[villager.GetPosition().x, villager.GetPosition().y].IsAccessible())
            {
                //Construire
                restAPI.CastActionAsync(villager.GetId(), "COMMENCER_CONSTRUCTION", "ATELIER_DE_CHARBONNIER");
                StartCoroutine(restAPI.GetMap(new Vector2Int(villager.GetPosition().x - 3, villager.GetPosition().x + 2),
                    new Vector2Int(villager.GetPosition().y - 3, villager.GetPosition().y + 2)));
                return;
            }
        }
    }

}
