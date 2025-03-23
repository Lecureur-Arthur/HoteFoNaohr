using System.Collections;
using UnityEngine;

public class Villager
{
    private string _id = "";
    private string _name = "";
    private string _lastActionDate = "";
    private bool _available = false;
    private VillagerType _type;
    private Vector2Int _position;

    public Villager(string id, string name, string lastActionDate, bool available, VillagerType type, Vector2Int position)
    {
        _id = id;
        _name = name;
        _lastActionDate = lastActionDate;
        _available = available;
        _type = type;
        _position = position;
    }

    public Vector2Int GetPosition() { return _position; }
    public string GetId() { return _id; }
}