using System.Collections.Generic;
using UnityEngine;

public class Biome
{
    private string _id = "";
    private string _name = "";
    private string _description = "";


    public Biome(string id, string name, string description)
    {
        _id = id;
        _name = name;
        _description = description;
    }
}