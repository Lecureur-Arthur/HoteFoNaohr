using System.Collections.Generic;
using UnityEngine;


public class Building
{
    private string _id = "";
    private string _description = "";
    private string _type = "";
    private int _constructionTime = 0;
    private bool _isWonder = false;
    private List<Biome> _buildableHere = new List<Biome>();
    private Dictionary<Resource, int> _turnCost = new Dictionary<Resource, int>();
    private Dictionary<Resource, int> _buildingCost = new Dictionary<Resource, int>();
    private Dictionary<Resource, int> _buildingBonus = new Dictionary<Resource, int>();
    private Dictionary<Resource, int> _bonus = new Dictionary<Resource, int>();
    private List<string> _supplements = new List<string>();

    public Building(string id, string description, string type, int constructionTime, bool isWonder, List<Biome> buildableHere, Dictionary<Resource, int> turnCost, Dictionary<Resource, int> buildingCost, Dictionary<Resource, int> buildingBonus = null, Dictionary<Resource, int> bonus = null, List<string> supplements = null)
    {
        _id = id;
        _description = description;
        _type = type;
        _constructionTime = constructionTime;
        _isWonder = isWonder;
        _buildableHere = buildableHere;
        _turnCost = turnCost;
        _buildingCost = buildingCost;
        _buildingBonus = buildingBonus;
        _bonus = bonus;
        _supplements = supplements;
    }
    public string GetBuildType() { return _type; }
}