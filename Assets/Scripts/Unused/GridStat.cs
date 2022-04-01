using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridStat : MonoBehaviour
{
    int _visited = -1;
    Vector2Int _pos;

    public Vector2Int getPos()
    {
        return _pos;
    }

    public void setValue(int value)
    {
        _visited = value;
    }

    public int getValue()
    {
        return _visited;
    }

    public void AssignValues(int x, int y)
    {
        _pos = new Vector2Int(x,y);
    }
}
