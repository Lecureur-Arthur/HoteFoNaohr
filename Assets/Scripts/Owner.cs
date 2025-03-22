using System.Collections;
using UnityEngine;

public class Owner
{
    private string _id = "";
    private string _name = "";
    private string _type = "";

    public Owner(string id, string name, string type)
    {
        _id = id;
        _name = name;
        _type = type;
    }
}