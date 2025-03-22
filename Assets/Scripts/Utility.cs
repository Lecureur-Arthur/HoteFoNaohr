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
            Resource resouce = new Resource(root["idRessource"], root["ressource"]);
            int quantity = root["quantite"];
            dict.Add(resouce, quantity);
        }

        return dict;
    }

    static public Dictionary<Resource, int> ConstructResourceDictNoID(JSONNode root)
    {
        Dictionary<Resource, int> dict = new Dictionary<Resource, int>();
        foreach (JSONNode res in root)
        {
            JSONNode jsonRes = root["ressource"];
            Resource resource = new Resource(jsonRes["idRessource"], jsonRes["nom"], jsonRes["description"]);
            int quantity = root["quantite"];
            dict.Add(resource, quantity);
        }

        return dict;
    }
}
