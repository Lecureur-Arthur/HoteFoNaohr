using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain
{
    private string _id = "";
    private string _name = "";
    private string _description = "";

    private Dictionary<Resource, int> _availableResources = new Dictionary<Resource, int>();

    public Terrain(string id, string name, string description, Dictionary<Resource, int> availableResources)
    {
        _id = id;
        _name = name;
        _description = description;
        _availableResources = availableResources;
    }

    public string GetName()
    {
        return _name;
    }
}