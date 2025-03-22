using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    // X and Y
    private Vector2Int _coordinates = new Vector2Int(0, 0);
    private BuildingProgress _builtBuilding = null;
    private bool _accessible = false;
    private Biome _biome;
    private Terrain _terrain;
    private Owner _owner;
    private Dictionary<Resource, int> _resources = new Dictionary<Resource, int>();

    public Tile (Vector2Int coordinates, BuildingProgress builtBuilding, bool accessible, Biome biome, Terrain terrain, Owner owner, Dictionary<Resource, int> resources)
    {
        _coordinates = coordinates;
        _builtBuilding = builtBuilding;
        _accessible = accessible;
        _biome = biome;
        _terrain = terrain;
        _owner = owner;
        _resources = resources;
    }
    public Tile(Vector2Int coordinates, bool accessible, Biome biome, Terrain terrain, Owner owner, Dictionary<Resource, int> resources)
    {
        _coordinates = coordinates;
        _accessible = accessible;
        _biome = biome;
        _terrain = terrain;
        _owner = owner;
        _resources = resources;
    }

    public Vector2Int GetCoordinates() { return _coordinates; }
    public Biome GetBiome() { return _biome; }
    public Terrain GetTerrain() { return _terrain; }
    public Owner GetOwner() { return _owner; }
    public Dictionary<Resource, int> GetResources() { return _resources; }
    public BuildingProgress GetBuiltBuilding() { return _builtBuilding; }
    public bool IsAccessible() { return _accessible; }


}
