using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Followed the guide as after trying this three times I absolutely lost my mind lmao
/// 
/// References:
/// https://docs.unity3d.com/ScriptReference/Texture2D.SetPixel.html
/// </summary>
/// 

public class GridGeneration : MonoBehaviour
{
    Nullable<int> _seed = null;

    [SerializeField] GameObject _floorPrefab, _wallPrefab;

    [SerializeField] GameObject[] _spawnableItems, _spawnableEnemies, _spawnablePlayers;
    [SerializeField] Actor[] _actors; // wip
    [SerializeField] Image _map;
    [SerializeField] float _mapScale = 2;
    [SerializeField] float _mapOffset = .3f; // Apparently textures wrap around :^)

    [SerializeField] float _spawnProbabillity = 50, _enemyProbabillity = 10; // % chance of spawning item

    [SerializeField] protected int _gridWidth = 50;
    [SerializeField] protected int _gridHeight = 50;

    [SerializeField] int _minRoomSize = 4;
    [SerializeField] int _maxRoomSize = 10;

    [SerializeField] int _minRoomDist = 1;

    int _spawnedPlayers = 0, _maxPlayers = 5;

    [SerializeField] int _roomCount = 5;
    protected Dictionary<Vector3Int, GroundType> _dungeon = new Dictionary<Vector3Int, GroundType>();
    List<Room> _roomCollection = new List<Room>();

    BattleLoop _loop;


    //I know, Player prefs suck, I just really ran out of time..
    void CheckForSave()
    {
        if (PlayerPrefs.HasKey("seed"))
            UnityEngine.Random.seed = PlayerPrefs.GetInt("seed");
        else
            PlayerPrefs.SetInt("seed", UnityEngine.Random.seed);
        print(UnityEngine.Random.seed);
    }

    void Awake()
    {
        CheckForSave();
        _loop = GetComponent<BattleLoop>();
        Generate();
    }

    public Vector2Int GetWidthHeight()
    {
        return new Vector2Int(_gridWidth, _gridHeight);
    }

    public Dictionary<Vector3Int, GroundType> GiveDictionary()
    {
        return _dungeon;
    }

    void Generate()
    {
        for (int i = 0; i < _roomCount; i++)
        {
            int offset = _maxRoomSize;
            int minX = UnityEngine.Random.Range(0, _gridWidth - offset);
            int maxX = minX + UnityEngine.Random.Range(_minRoomSize, offset + 1);
            int minZ = UnityEngine.Random.Range(0, _gridHeight - offset);
            int maxZ = minZ + UnityEngine.Random.Range(_minRoomSize, offset + 1);

            Room room = new Room(minX, maxX, minZ, maxZ);
            if (CanRoomFitInDungeon(room))
                AddRoomToDungeon(room);
            else
                i--;
        }
        for (int i = 0; i < _roomCollection.Count; i++)
        {
            Room room = _roomCollection[i];
            Room otherRoom = _roomCollection[(i + UnityEngine.Random.Range(1, _roomCollection.Count)) % _roomCollection.Count];
            ConnectRooms(room, otherRoom);
        }

        AllocateWalls();
        SpawnDungeon();
    }

    void AllocateWalls()
    {
        var keys = _dungeon.Keys.ToList();
        foreach (var kv in keys)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (Mathf.Abs(x) == Mathf.Abs(z)) continue;
                        Vector3Int newPos = kv + new Vector3Int(x, 0, z);
                    if (_dungeon.ContainsKey(newPos)) continue;
                        _dungeon.Add(newPos, GroundType.Wall);
                }
            }
        }
    }

    void ConnectRooms(Room _roomA, Room _roomB)
    {
        Vector3Int posA = _roomA.GetCenter();
        Vector3Int posB = _roomB.GetCenter();
        int dirX = posB.x > posA.x ? 1 : -1;
        int x = 0;
        for (x = posA.x; x != posB.x; x += dirX)
        {
            Vector3Int position = new Vector3Int(x, 0, posA.z);
            if (_dungeon.ContainsKey(position)) continue;
                _dungeon.Add(new Vector3Int(x, 0, posA.z), GroundType.Floor);
        }
        int dirZ = posB.z > posA.z ? 1 : -1;
        for (int z = posA.z; z != posB.z; z += dirZ)
        {
            Vector3Int position = new Vector3Int(x, 0, z);
            if (_dungeon.ContainsKey(position)) continue;
                _dungeon.Add(new Vector3Int(x, 0, z), GroundType.Floor);
        }
    }

    public void SpawnDungeon()
    {
        Texture2D texture = new Texture2D(_gridWidth + Mathf.CeilToInt(_gridWidth * _mapOffset), _gridHeight + Mathf.FloorToInt(_gridHeight * _mapOffset));
        _map.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_gridWidth * _mapScale, _gridHeight * _mapScale + Mathf.FloorToInt(_gridHeight * _mapOffset));
        _map.material.mainTexture = texture;
        foreach (KeyValuePair<Vector3Int, GroundType> kv in _dungeon)
        {
            switch (kv.Value)
            {
                case GroundType.Floor:
                        GameObject tmp2 = Instantiate(_floorPrefab, kv.Key, Quaternion.identity, transform);
                        tmp2.name = $"{kv.Key}";
                        texture.SetPixel(kv.Key.x, kv.Key.z, Color.white);
                    break;
                case GroundType.Wall:
                        Instantiate(_wallPrefab, kv.Key + Vector3.up / 2, Quaternion.identity, transform);
                        texture.SetPixel(kv.Key.x, kv.Key.z, Color.gray);
                    break;
                case GroundType.Unset:
                        texture.SetPixel(kv.Key.x, kv.Key.z, Color.clear);
                    break;
            }

            if (_spawnedPlayers < _maxPlayers)
            {
                GameObject temp = Instantiate(_spawnablePlayers[UnityEngine.Random.Range(0, _spawnablePlayers.Length)], kv.Key + Vector3.up, Quaternion.identity, transform);
                temp.name = $"{kv.Key}Char";
                _loop.AddChar(temp, kv.Key);
                _spawnedPlayers++;
            }

            //Spawn items
            int tmp = _gridWidth * _gridHeight;
            if (_spawnableItems.Length > 0 && kv.Value == GroundType.Floor && UnityEngine.Random.Range(0, tmp) <= (tmp / 100) * _spawnProbabillity)
                Instantiate(_spawnableItems[UnityEngine.Random.Range(0, _spawnableItems.Length)], kv.Key + Vector3.up * 0.8f, Quaternion.identity, transform);
            if (_spawnableEnemies.Length > 0 && kv.Value == GroundType.Floor && UnityEngine.Random.Range(0, tmp) <= (tmp / 100) * _enemyProbabillity)
                Instantiate(_spawnableEnemies[UnityEngine.Random.Range(0, _spawnableEnemies.Length)], kv.Key + Vector3.up, Quaternion.identity, transform);
        }
        texture.Apply();
    }

    public void AddRoomToDungeon(Room _room)
    {
        for (int x = _room.minX; x <= _room.maxX; x++)
        {
            for (int z = _room.minZ; z <= _room.maxZ; z++)
                _dungeon.Add(new Vector3Int(x, 0, z), GroundType.Floor);
        }
        _roomCollection.Add(_room);
    }

    public bool CanRoomFitInDungeon(Room _room)
    {
        for (int x = _room.minX - _minRoomDist; x <= _room.maxX; x++)
        {
            for (int z = _room.minZ; z <= _room.maxZ + _minRoomDist; z++)
            {
                if (_dungeon.ContainsKey(new Vector3Int(x, 0, z))) return false;
                if ((_room.minX + _room.maxX < _gridWidth || _room.maxZ + _room.maxZ < _gridHeight)) return false; //Temporary fix, still not working entirely!!
            }
        }
        return true;
    }
}

public class Room
{
    public int roomID;
    public int minX, maxX, minZ, maxZ;
    public Room(int _minX, int _maxX, int _minZ, int _maxZ)
    {
        minX = _minX;
        maxX = _maxX;
        minZ = _minZ;
        maxZ = _maxZ;
    }

    public Vector3Int GetCenter()
    {
        return new Vector3Int(Mathf.RoundToInt(Mathf.Lerp(minX, maxX, 0.5f)), 0, Mathf.RoundToInt(Mathf.Lerp(minZ, maxZ, 0.5f)));
    }
}
