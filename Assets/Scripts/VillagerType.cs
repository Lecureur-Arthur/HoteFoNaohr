using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerType
{
    private string _name = "";
    private string _desc = "";
    private float _cooldownCoefficient = 0f;

    public VillagerType(string name, string desc, float cooldownCoefficient)
    {
        _name = name;
        _desc = desc;
        _cooldownCoefficient = cooldownCoefficient;
    }
}
