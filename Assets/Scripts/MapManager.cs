using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing.MiniJSON;
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
    public GameObject pointValue;

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
                            if (resources[resource] > 0)
                            {
                                    restAPI.CastActionAsync(villager.GetId(), "RECOLTER", resource.GetName());
                                    postTimer = maxPostTimer;
                            }
                        }
                    }
                }
            }
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
            Debug.Log(activeResources[res].ToString());
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
            }
        }

    }

    public void GenerateMap()
    {
        foreach(Tile tile in Map)
        {
            if(tile != null)
            {
                Vector2Int pos = tile.GetCoordinates() * 7;
                Vector3 worldPos = new Vector3(pos.x, 0, pos.y);
                if(TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y] == null)
                    TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y] = new GameObject("Tile");
                TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform.SetParent(MapObject.transform, false);

                GameObject biome = new GameObject("Biome");
                GameObject terrain = new GameObject("Terrain");
                GameObject building = new GameObject("Batiment");

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
                if (TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y] != null)
                    biome.transform.SetParent(TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform, false);
                else
                    Destroy(biome);

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

                if (TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y] != null)
                    terrain.transform.SetParent(TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform, false);
                else
                    Destroy(terrain);

                switch (tile.GetBuiltBuilding()?.GetBuild().GetBuildType())
                {
                    case "CABANE_DE_BUCHERON":
                        building = Instantiate(logging_hut, worldPos + logging_hut.transform.position, Quaternion.identity);
                        break;
                    case "SCIERIE":
                        building = Instantiate(sawMill, worldPos + sawMill.transform.position, Quaternion.identity);
                        break;
                    case "ATELIER_DE_TAILLE_DE_PIERRE":
                        building = Instantiate(stone_cutting_workshop, worldPos + stone_cutting_workshop.transform.position, Quaternion.identity);
                        break;
                    case "CARRIERE":
                        building = Instantiate(quarry, worldPos + quarry.transform.position, Quaternion.identity);
                        break;
                    case "EXCAVATRICE_A_FER":
                        building = Instantiate(iron_excavator, worldPos + iron_excavator.transform.position, Quaternion.identity);
                        break;
                    case "MINE_DE_FER":
                        building = Instantiate(iron_mining, worldPos + iron_mining.transform.position, Quaternion.identity);
                        break;
                    case "ATELIER_DE_CHARBONNIER":
                        building = Instantiate(coal_mining_workshop, worldPos + coal_mining_workshop.transform.position, Quaternion.identity);
                        break;
                    case "MINE_DE_CHARBON":
                        building = Instantiate(coal_mining, worldPos + coal_mining.transform.position, Quaternion.identity);
                        break;
                    case "MOULIN":
                        building = Instantiate(mill, worldPos + mill.transform.position, Quaternion.identity);
                        break;
                    case "FERME":
                        building = Instantiate(farm, worldPos + farm.transform.position, Quaternion.identity);
                        break;
                    case "PORT":
                        building = Instantiate(port, worldPos + port.transform.position, Quaternion.identity);
                        break;
                    case "EOLIENNE":
                        building = Instantiate(windMill, worldPos + windMill.transform.position, Quaternion.identity);
                        break;
                    case "CENTRALE_ELECTRIQUE_AU_CHARBON":
                        building = Instantiate(coal_power_plant, worldPos + coal_power_plant.transform.position, Quaternion.identity);
                        break;
                    case "CENTRALE_AU_METHANE":
                        building = Instantiate(methane_power_plant, worldPos + methane_power_plant.transform.position, Quaternion.identity);
                        break;
                    case "CENTRALE_A_BIOMASSE":
                        building = Instantiate(biomass_plant, worldPos + biomass_plant.transform.position, Quaternion.identity);
                        break;
                    case "TURBINE_HYDRAULIQUE":
                        building = Instantiate(hydro_turbine, worldPos + hydro_turbine.transform.position, Quaternion.identity);
                        break;
                    case "INSTALLATION_FORESTIERE":
                        building = Instantiate(forest_plant, worldPos + forest_plant.transform.position, Quaternion.identity);
                        break;
                    case "USINE_DE_RENOUVELLEMENT":
                        building = Instantiate(renewal_plant, worldPos + renewal_plant.transform.position, Quaternion.identity);
                        break;
                    case "PUITS_DE_CARBONE":
                        building = Instantiate(carbon_well, worldPos + carbon_well.transform.position, Quaternion.identity);
                        break;
                    case "TOUR_DE_GUET":
                        building = Instantiate(guet_tower, worldPos + guet_tower.transform.position, Quaternion.identity);
                        break;
                    case "OBSERVATOIRE":
                        building = Instantiate(observatory, worldPos + observatory.transform.position, Quaternion.identity);
                        break;
                    case "MUSEE":
                        building = Instantiate(museum, worldPos + museum.transform.position, Quaternion.identity);
                        break;
                    case "BIBLIOTHEQUE":
                        building = Instantiate(library, worldPos + library.transform.position, Quaternion.identity);
                        break;
                    case "THEATRE":
                        building = Instantiate(theater, worldPos + theater.transform.position, Quaternion.identity);
                        break;
                    case "GRANDE_STATUE":
                        building = Instantiate(high_statue, worldPos + high_statue.transform.position, Quaternion.identity);
                        break;
                    case "CAPITOLE":
                        building = Instantiate(capitol, worldPos + capitol.transform.position, Quaternion.identity);
                        break;
                    case "BATEAU_DE_CROISIERE":
                        building = Instantiate(ship, worldPos + ship.transform.position, Quaternion.identity);
                        break;
                    case "GRANDE_BIBLIOTHEQUE":
                        building = Instantiate(high_library, worldPos + high_library.transform.position, Quaternion.identity);
                        break;
                    case "CHATEAU":
                        building = Instantiate(castle, worldPos + castle.transform.position, Quaternion.identity);
                        break;
                    case "REACTEUR_A_FUSION_NUCLEAIRE ":
                        building = Instantiate(nuclear_fusion_reactor, worldPos + nuclear_fusion_reactor.transform.position, Quaternion.identity);
                        break;
                }

                if (TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y] != null)
                    building.transform.SetParent(TileMap[tile.GetCoordinates().x, tile.GetCoordinates().y].transform, false);
                else
                    Destroy(building);
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

}
