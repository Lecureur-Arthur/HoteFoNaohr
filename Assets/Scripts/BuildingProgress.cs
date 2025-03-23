using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProgress
{
    private int _progress;
    private string _id = "";
    private Owner _owner;
    private Building _building;

    public BuildingProgress(int progress, string id, Owner owner, Building building)
    {
        _progress = progress;
        _id = id;
        _owner = owner;
        _building = building;
    }
    public Building GetBuild()
    {
        return _building;
    }
}
