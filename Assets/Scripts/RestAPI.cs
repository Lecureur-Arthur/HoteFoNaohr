using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class RestAPI : MonoBehaviour
{
    public UnityEvent mapReady = new UnityEvent();
    public UnityEvent npcReady = new UnityEvent();
    public UnityEvent uiUpdate = new UnityEvent();
    private string URL = "http://51.210.117.22:8080";

    // Start is called before the first frame update
    void Start()
    {
        if (mapReady == null)
            mapReady = new UnityEvent();

        if (npcReady == null)
            npcReady = new UnityEvent();

        if (uiUpdate == null)
            uiUpdate = new UnityEvent();

        mapReady.AddListener(MapManager.Instance.GenerateMap);
    }

    public IEnumerator CastAction(string idVillager, string action, string reference = null)
    {
        string body = "";
        if (reference == null)
        {
            body = "{\naction:" + action + "\n}";
        }
        else
        {
            body = "{\naction: " + action + "\nreferece: " + "\n}";
        }

        

        string uri = URL + $"/equipes/{Config.GetId_Team()}/villageois/{idVillager}/demander-action";
        UnityWebRequest request = new UnityWebRequest(uri, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());
        if (body != null)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(body);
            UploadHandlerRaw upHandler = new UploadHandlerRaw(data);
            upHandler.contentType = "application/json";
            request.uploadHandler = upHandler;
        }
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        } else
        {

        }
    }

    public UnityWebRequestAsyncOperation CastActionAsync(string idVillager, string action, string reference = null)
    {
        string body = "";
        if (reference == null)
        {
            body = "{\n\"action\": \"" + action + "\"\n}";
        }
        else
        {
            body = "{\n\"action\": \"" + action + "\",\n\"reference\": \"" + reference + "\"\n}";
        }



        string uri = URL + $"/equipes/{Config.GetId_Team()}/villageois/{idVillager}/demander-action";
        UnityWebRequest request = new UnityWebRequest(uri, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());
        if (body != null)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(body);
            UploadHandlerRaw upHandler = new UploadHandlerRaw(data);
            upHandler.contentType = "application/json";
            request.uploadHandler = upHandler;
        }

        return request.SendWebRequest();
    }

        public IEnumerator GetMap(Vector2Int x, Vector2Int y)
    {
        string uri = URL + $"/monde/map?x_range={x.x},{x.y}&y_range={y.x},{y.y}";
        UnityWebRequest request = new UnityWebRequest(uri, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request?.downloadHandler.text;
            JSONNode map = JSON.Parse(json);

            foreach(JSONNode tile in map)
            {
                Vector2Int position = new Vector2Int(tile["coord_x"], tile["coord_y"]);
                
                if (position.x >= 0 && position.x < 33 && position.y >= 0 && position.y < 33)
                {
                    JSONNode jsonBiome = tile["biome"];
                    JSONNode jsonTerrain = tile["terrain"];
                    JSONNode jsonOwner = tile["proprietaire"];
                    JSONNode jsonBiomeAllowedBuildings = jsonBiome["batimentsContructible"];


                    List<Building> buildings = new List<Building>();
                    foreach (JSONNode _building in jsonBiomeAllowedBuildings)
                    {

                        JSONNode _buildableOn = _building["contructibleSur"];
                        List<Biome> _buildableOnList = new List<Biome>();
                        foreach (JSONNode _biome in _buildableOn)
                        {
                            Biome b = new Biome(_biome["identifiant"], _biome["nom"], _biome["description"]);
                            _buildableOnList.Add(b);
                        }

                        buildings.Add(new Building(_building["id"], _building["description"], _building["type"], _building["tempsConstruction"],
                            _building["estUneMerveille"], _buildableOnList, Utility.ConstructResourceDict(_building["coutParTour"]),
                            Utility.ConstructResourceDict(_building["coutConstruction"]), Utility.ConstructResourceDict(_building["bonusConstruction"]),
                            Utility.ConstructResourceDict(_building["bonus"])));
                    }


                    Biome biome = new Biome(jsonBiome["identifiant"], jsonBiome["nom"], jsonBiome["description"]);

                    Terrain terrain = new Terrain(jsonTerrain["identifiant"], jsonTerrain["nom"], jsonTerrain["description"], Utility.ConstructResourceDictNoID(jsonTerrain["ressourcesPresente"]));
                    Owner owner = new Owner(jsonOwner["idEquipe"], jsonOwner["nom"], jsonOwner["type"]);

                    JSONNode jsonBuild = tile["batiment_construit"];
                    JSONNode jsonBuilding = jsonBuild["detailBatiment"];
                    JSONNode jsonBuildingOwner = jsonBuild["proprietaire"];
                    bool accessible = tile["accessible"];
                    JSONNode jsonResources = tile["ressources"];

                    if (jsonBuild != null)
                    {
                        JSONNode _pBuildableOn = jsonBuilding["contructibleSur"];
                        List<Biome> _pBuildableOnList = new List<Biome>();
                        foreach (JSONNode _biome in _pBuildableOn)
                        {
                            Biome b = new Biome(_biome["identifiant"], _biome["nom"], _biome["description"]);
                            _pBuildableOnList.Add(b);
                        }
                        Building _buildProgress = new Building(jsonBuilding["id"], jsonBuilding["description"], jsonBuilding["type"], jsonBuilding["tempsConstruction"],
                                jsonBuilding["estUneMerveille"], _pBuildableOnList, Utility.ConstructResourceDict(jsonBuilding["coutParTour"]),
                                Utility.ConstructResourceDict(jsonBuilding["coutConstruction"]), Utility.ConstructResourceDict(jsonBuilding["bonusConstruction"]),
                                Utility.ConstructResourceDict(jsonBuilding["bonus"]));


                        Owner buildingOwner = new Owner(jsonBuildingOwner["idEquipe"], jsonBuildingOwner["nom"], jsonBuildingOwner["type"]);

                        BuildingProgress building = new BuildingProgress(jsonBuild["progression"], jsonBuild["identifiant"], buildingOwner, _buildProgress);


                        MapManager.Instance.Map[position.x, position.y] = new Tile(position, building, accessible, biome, terrain, owner, Utility.ConstructResourceDictNoID(jsonResources));
                    }

                    MapManager.Instance.Map[position.x, position.y] = new Tile(position, accessible, biome, terrain, owner, Utility.ConstructResourceDictNoID(jsonResources));
                }
            }
            mapReady?.Invoke();
        }
    }

    public IEnumerator GetTeam()
    {
        // http://51.210.117.22:8080/equipes/f77e66d4-be12-4249-bab0-02701e3b0853
        string uri = URL + $"/equipes/{Config.GetId_Team()}";
        UnityWebRequest request = new UnityWebRequest(uri, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request?.downloadHandler.text;
            JSONNode team = JSON.Parse(json);

            // REcuperation des données
            MapManager.Instance.activeResources = Utility.ConstructResourceDictNoID(team["ressources"]);
            uiUpdate.Invoke();

            JSONNode jsonVillagers = team["villageois"];
            List<Villager> villagers = new List<Villager>();

            foreach(JSONNode jsonVillager in jsonVillagers)
            {
                JSONNode jsonVillagerType = jsonVillager["type"];
                VillagerType villagerType = new VillagerType(jsonVillagerType["nom"], jsonVillagerType["description"], 
                    jsonVillagerType["mutliplicateurDeCooldown"]);

                villagers.Add(new Villager(jsonVillager["idVillageois"], jsonVillager["nom"], 
                    jsonVillager["dateDerniereAction"], jsonVillager["disponible"],
                    villagerType, new Vector2Int(jsonVillager["positionX"], jsonVillager["positionY"])));
            }

        }
    }

    public IEnumerator GetBatiments()
    {
        // http://51.210.117.22:8080/batiments
        string uri = URL + $"/batiments";
        UnityWebRequest request = new UnityWebRequest(uri, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request?.downloadHandler.text;
            SimpleJSON.JSONNode building = SimpleJSON.JSON.Parse(json);

            Debug.Log(building?.ToString());
            // REcuperation des données
            List<Building> buildings = new List<Building>();
            foreach (JSONNode b in building)
            {
                JSONNode jsonBiomes = b["contructibleSur"];
                List<Biome> biomes = new List<Biome>();
                foreach (JSONNode jsonBiome in jsonBiomes)
                {
                    Biome biome = new Biome(jsonBiome["id"], jsonBiome["name"], jsonBiome["description"]);
                    biomes.Add(biome);
                }
                Building tempBuilding = new Building(b["id"], b["description"], b["type"], b["tempsConstruction"], b["estUneMerveille"],
                biomes, Utility.ConstructResourceDict(b["coutParTour"]), Utility.ConstructResourceDict(b["coutConstruction"]), Utility.ConstructResourceDict(b["bonusConstruction"]),
                Utility.ConstructResourceDict(b["bonus"]));
                buildings.Add(tempBuilding);
            }
        }
    }

    public IEnumerator GetBatimentsDispo()
    {
        // http://51.210.117.22:8080/batiments/disponible
        string uri = URL + $"/batiments/disponible";
        UnityWebRequest request = new UnityWebRequest(uri, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request?.downloadHandler.text;
            JSONNode building = JSON.Parse(json);

            Debug.Log(building?.ToString());
            // REcuperation des données
            List<Building> buildings = new List<Building>();
            foreach (JSONNode b in building)
            {
                JSONNode jsonBiomes = b["contructibleSur"];
                List<Biome> biomes = new List<Biome>();
                foreach (JSONNode jsonBiome in jsonBiomes)
                {
                    Biome biome = new Biome(jsonBiome["id"], jsonBiome["name"], jsonBiome["description"]);
                    biomes.Add(biome);
                }
                Building tempBuilding = new Building(b["id"], b["description"], b["type"], b["tempsConstruction"], b["estUneMerveille"],
                biomes, Utility.ConstructResourceDict(b["coutParTour"]), Utility.ConstructResourceDict(b["coutConstruction"]), Utility.ConstructResourceDict(b["bonusConstruction"]),
                Utility.ConstructResourceDict(b["bonus"]));
                buildings.Add(tempBuilding);
            }
        }
    }

    public IEnumerator GetBatimentsWithIdBatiment(string id_batiment)
    {
        // http://51.210.117.22:8080/batiments/{idBatiment}
        string uri = URL + $"/batiments/{id_batiment}";
        UnityWebRequest request = new UnityWebRequest(uri, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request?.downloadHandler.text;
            JSONNode building = JSON.Parse(json);

            Debug.Log(building?.ToString());
            // REcuperation des données
            JSONNode jsonBiomes = building["contructibleSur"];
            List<Biome> biomes = new List<Biome>();
            foreach (JSONNode jsonBiome in jsonBiomes)
            {
                Biome biome = new Biome(jsonBiome["id"], jsonBiome["name"], jsonBiome["description"]);
                biomes.Add(biome);
            }
            Building tempBuilding = new Building(building["id"], building["description"], building["type"], building["tempsConstruction"], building["estUneMerveille"],
            biomes, Utility.ConstructResourceDict(building["coutParTour"]), Utility.ConstructResourceDict(building["coutConstruction"]), Utility.ConstructResourceDict(building["bonusConstruction"]),
            Utility.ConstructResourceDict(building["bonus"]));
        }
    }

    public IEnumerator GetTeamNpcWithIdEquipe()
    {
        // http://51.210.117.22:8080/equipes/{idEquipe}/villageois
        string uri = URL + $"/equipes/{Config.GetId_Team()}/villageois";
        UnityWebRequest request = new UnityWebRequest(uri, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request?.downloadHandler.text;
            JSONNode jsonVillagers = JSON.Parse(json);

            // REcuperation des données
            List<Villager> villagers = new List<Villager>();

            foreach (JSONNode jsonVillager in jsonVillagers)
            {
                
                JSONNode jsonVillagerType = jsonVillager["type"];
                if (jsonVillagerType != null)
                {
                    VillagerType villagerType = new VillagerType(jsonVillagerType["nom"], jsonVillagerType["description"],
                        jsonVillagerType["mutliplicateurDeCooldown"]);
                    villagers.Add(new Villager(jsonVillager["idVillageois"], jsonVillager["nom"],
                        jsonVillager["dateDerniereAction"], jsonVillager["disponible"],
                        villagerType, new Vector2Int(jsonVillager["positionX"], jsonVillager["positionY"])));
                }

            }
            MapManager.Instance.Villagers = villagers;
            npcReady.Invoke();
        }
    }

    public IEnumerator GetTeamNpcWithIdEquipeAndIdPnj(string id_villageois)
    {
        // http://51.210.117.22:8080/equipes/{idEquipe}/villageois/{idVillageois}
        string uri = URL + $"/equipes/{Config.GetId_Team()}/villageois/{id_villageois}";
        UnityWebRequest request = new UnityWebRequest(uri, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request?.downloadHandler.text;
            SimpleJSON.JSONNode jsonVillager = SimpleJSON.JSON.Parse(json);

            Debug.Log(jsonVillager?.ToString());
            // REcuperation des données

            JSONNode jsonVillagers = jsonVillager["Villageois"];

            JSONNode jsonVillagerType = jsonVillagers["type"];
            VillagerType villagerType = new VillagerType(jsonVillagerType["nom"],
                jsonVillagers["description"], jsonVillagers["mutliplicateurDeCooldown"]);

            MapManager.Instance.VillagerById = new Villager(jsonVillagers["idVillageois"], jsonVillagers["nom"],
                jsonVillagers["dateDerniereAction"], jsonVillagers["disponible"],
                villagerType, new Vector2Int(jsonVillagers["positionX"], jsonVillagers["positionY"]));

        }
    }

    public UnityWebRequestAsyncOperation GetRessourcesDirect()
    {
        string uri = URL + $"/ressources";
        UnityWebRequest request = new UnityWebRequest(uri, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());
        return request.SendWebRequest();

    }

    public IEnumerator GetRessources()
    {
        // http://51.210.117.22:8080/ressources
        string uri = URL + $"/ressources";
        UnityWebRequest request = new UnityWebRequest(uri, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string json = request?.downloadHandler.text;
            JSONNode rsc = JSON.Parse(json);

            Debug.Log(rsc?.ToString());
            // REcuperation des données

            JSONNode jsonResources = rsc["Ressource"];
            List<Resource> ressource = new List<Resource>();

            foreach (JSONNode jsonResource in jsonResources)
            {
                ressource.Add(new Resource(jsonResource["idRessource"], jsonResource["description"],
                    jsonResource["nom"], jsonResource["type"]));
            }
        }
    }

    public UnityWebRequestAsyncOperation GetTeamAsync()
    {
        // http://51.210.117.22:8080/equipes/f77e66d4-be12-4249-bab0-02701e3b0853
        string uri = URL + $"/equipes/{Config.GetId_Team()}";
        UnityWebRequest request = new UnityWebRequest(uri, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", "Bearer " + Config.GetToken());

        //Debug.Log("Sending to the moon my very allergic nemesis");

        return request.SendWebRequest();
    }

}
