using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    static public Dictionary<Resource, int> ConstructResourceDict(JSONNode root)
    {
        Dictionary<Resource, int> dict = new Dictionary<Resource, int>();
        foreach (JSONNode res in root)
        {
            Resource resource = new Resource(root["idRessource"], root["ressource"]);
            int quantity = root["quantite"];
            dict[resource] = quantity;
        }

        return dict;
    }

    static public Dictionary<Resource, int> ConstructResourceDictNoID(JSONNode root)
    {
        Dictionary<Resource, int> dict = new Dictionary<Resource, int>();
        foreach (JSONNode res in root)
        {
            foreach(JSONNode jsonRes in root)
            {
                JSONNode _jsonRes = jsonRes["ressource"];
                Resource resource = new Resource(_jsonRes["idRessource"], _jsonRes["nom"], _jsonRes["description"], _jsonRes["type"]);
                int quantity = jsonRes["quantite"];
                dict[resource] = quantity;
            }
        }

        return dict;
    }
}
