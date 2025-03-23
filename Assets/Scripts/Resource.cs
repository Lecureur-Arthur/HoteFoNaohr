using System.Collections;
using UnityEngine;

public class Resource
{
    private string _id = "";
    private string _name = "";
    private string _description = "";
    private string type = "";

    public Resource(string id, string name, string description = "", string type = "")
    {
        _id = id;
        _name = name;
        _description = description;
        this.type = type;
    }

    public string GetName() { return _name; }
}