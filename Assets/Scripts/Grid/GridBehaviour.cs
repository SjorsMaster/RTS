using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A* based on this video:
/// https://www.youtube.com/watch?v=fUiNDDcU_I4
/// However it didn't really work that well for my usecase so had to reverse engeneer it and fix it.. 
/// ..may have been smarter to just make it from the start, but math is hard.. At least I learned that and how A* works! 
/// Lots of changes compared to the video, thanks Quint for helping me figure it out!
/// </summary>

public class GridBehaviour : MonoBehaviour
{
    GridGeneration _grid;
    [SerializeField] protected int _gridWidth;
    [SerializeField] protected int _gridHeight;

    //[SerializeField] int _dist;
    [SerializeField] List<Vector3Int> _path = new List<Vector3Int>();

    Vector3Int _startPos;
    Dictionary<Vector3Int, int> _visited = new Dictionary<Vector3Int, int>();

    protected Dictionary<Vector3Int, GroundType> _dungeon = new Dictionary<Vector3Int, GroundType>();

    void Start()
    {
        _grid = GetComponent<GridGeneration>();
        _dungeon = _grid.GiveDictionary();

        _gridWidth = _grid.GetWidthHeight().x;
        _gridHeight = _grid.GetWidthHeight().y;
    }

    void setDistance(Vector3Int startPoint)
    {
        _startPos = new Vector3Int(startPoint.x,0, startPoint.z);
        initialSetup();
        Vector3Int start = _startPos;
        for (int step = 1; step < _gridWidth * _gridHeight; step++)
        {
            foreach (var dic in _dungeon)
            {
                if (dic.Key.x < _gridWidth && dic.Key.z < _gridHeight && dic.Value == GroundType.Floor && _visited[dic.Key] == step - 1)
                    checkAllDirections(dic.Key, step);
            }
        }
    }

    public List<Vector3Int> GetPath(Vector3Int startPoint, Vector3Int endPoint)
    {
        setDistance(endPoint);
        setPath(startPoint);
        return _path;
    }

    void setPath(Vector3Int endTarget)
    {
        int step;
        Vector3Int target = endTarget;
        _path.Clear();
        // if the target was reached in the getDistance phase, then we found a path!
        if (_visited.ContainsKey(endTarget) && _visited[endTarget] > 0)
        {
            _path.Add(target);
            step = _visited[target] - 1;

            // the distance to the target is the number of steps it took us to get to the target using the shortest path
            //_dist = _visited[target];
        }
        else
        {
            print($"Can't reach..");
            return;
        }

        // using the step info we calculated from the getDistance phase, start at the target point and walk to the starting point
        for (; step > -1; step--)
        {
            if (checkDirection(target, step, 1))
                target = new Vector3Int(target.x - 1, 0, target.z);
            else if (checkDirection(target, step, 2))
                target = new Vector3Int(target.x + 1, 0, target.z);
            else if (checkDirection(target, step, 3))
                target = new Vector3Int(target.x, 0, target.z - 1);
            else if (checkDirection(target, step, 4))
                target = new Vector3Int(target.x, 0, target.z + 1);
            else
                Debug.Log($"error in setPath: can't find a continuous path using data generated in setDistance");
            _path.Add(target);
        }
    }

    void initialSetup()
    {
        _visited.Clear();
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int z = 0; z < _gridHeight; z++)
            {
                _visited.Add(new Vector3Int(x, 0, z), -1);
            }
        }
        _visited[new Vector3Int(_startPos.x, 0, _startPos.z)] = 0;
    }

    bool checkDirection(Vector3Int pos, int step, int direction)
    {
        switch (direction)
        {
            case 1:
                Vector3Int left = new Vector3Int(pos.x - 1, 0, pos.z);
                return pos.x > 0 && _visited.ContainsKey(left) && _visited[left] == step && _dungeon[left] == GroundType.Floor;
            case 2:
                Vector3Int right = new Vector3Int(pos.x + 1, 0, pos.z);
                return pos.x + 1 < _gridWidth && _visited.ContainsKey(right) && _visited[right] == step && _dungeon[right] == GroundType.Floor;
            case 3:
                Vector3Int down = new Vector3Int(pos.x, 0, pos.z - 1);
                return pos.z > 0 && _visited.ContainsKey(down) && _visited[down] == step && _dungeon[down] == GroundType.Floor;
            case 4:
                Vector3Int up = new Vector3Int(pos.x, 0, pos.z + 1);
                return pos.z + 1 < _gridHeight && _visited.ContainsKey(up) && _visited[up] == step && _dungeon[up] == GroundType.Floor;
        }
        return false;
    }

    void checkAllDirections(Vector3Int pos, int step) // Why you sus >:(
    {
        if (checkDirection(pos, -1, 1))
            setVisited(new Vector3Int(pos.x - 1, 0, pos.z), step);
        if (checkDirection(pos, -1, 2))
            setVisited(new Vector3Int(pos.x + 1, 0, pos.z), step);
        if (checkDirection(pos, -1, 3))
            setVisited(new Vector3Int(pos.x, 0, pos.z - 1), step);
        if (checkDirection(pos, -1, 4))
            setVisited(new Vector3Int(pos.x, 0, pos.z + 1), step);
    }

    void setVisited(Vector3Int pos, int step)
    {
        if (_visited[pos] == -1)
            _visited[pos] = step;
    }
}
