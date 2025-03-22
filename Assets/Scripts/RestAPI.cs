using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RestAPI : MonoBehaviour
{
    private string URL = "http://51.210.117.22:8080";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetMap(new Vector2Int(0, 10), new Vector2Int(0, 10)));
    }

    IEnumerator GetMap(Vector2Int x, Vector2Int y)
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
            Debug.Log(request?.downloadHandler.ToString());
            string json = request?.downloadHandler.text;
            JSONNode map = JSON.Parse(json);

            foreach(JSONNode tile in map)
            {
                Vector2Int position = new Vector2Int(tile["coord_x"], tile["coord_y"]);

                JSONNode jsonBiome = tile["biome"];
                JSONNode jsonTerrain = tile["terrain"];
                JSONNode jsonOwner = tile["proprietaire"];
                JSONNode jsonBiomeAllowedBuildings = jsonBiome["batimentsContructible"];
                List<Building> buildings = new List<Building>();
                foreach(JSONNode _building in jsonBiomeAllowedBuildings)
                {

                    JSONNode _buildableOn = _building["contructibleSur"];
                    List<Biome> _buildableOnList = new List<Biome>();
                    foreach(JSONNode _biome in _buildableOn)
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
                JSONNode jsonResources = tile["resources"];

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


            Debug.Log(map?.ToString());
            for (int i = x.x; i < x.y; i++)
            {
                for (int j = y.x; j < y.y; j++)
                {
                    //MapManager.Instance.map[i, j] 
                }
            }
        }
    }


    IEnumerator GetTeam()
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
            SimpleJSON.JSONNode map = SimpleJSON.JSON.Parse(json);

            Debug.Log(map?.ToString());
            // REcuperation des données

        }
    }

    IEnumerator GetBatiments()
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
            SimpleJSON.JSONNode map = SimpleJSON.JSON.Parse(json);

            Debug.Log(map?.ToString());
            // REcuperation des données

        }
    }

    IEnumerator GetBatimentsDispo()
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
            SimpleJSON.JSONNode map = SimpleJSON.JSON.Parse(json);

            Debug.Log(map?.ToString());
            // REcuperation des données

        }
    }

    IEnumerator GetBatimentsWithIdBatiment(string id_batiment)
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
            SimpleJSON.JSONNode map = SimpleJSON.JSON.Parse(json);

            Debug.Log(map?.ToString());
            // REcuperation des données

        }
    }

    IEnumerator getTeamNpcWithIdEquipe()
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
            SimpleJSON.JSONNode map = SimpleJSON.JSON.Parse(json);

            Debug.Log(map?.ToString());
            // REcuperation des données

        }
    }

    IEnumerator getTeamNpcWithIdEquipeAndIdPnj(string id_villageois)
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
            SimpleJSON.JSONNode map = SimpleJSON.JSON.Parse(json);

            Debug.Log(map?.ToString());
            // REcuperation des données

        }
    }

    IEnumerator getRessources()
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
            SimpleJSON.JSONNode map = SimpleJSON.JSON.Parse(json);

            Debug.Log(map?.ToString());
            // REcuperation des données

        }
    }

}
