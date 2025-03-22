using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    private string _id = "";
    private string _name = "";
    private string _description = "";

    private Dictionary<Resource, int> _availableResources = new Dictionary<Resource, int>();
}