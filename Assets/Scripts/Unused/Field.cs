using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    enum _groundType { Water, Grass, Stone, Unset = -1 }
    enum _occupation { Actors, GroundType }
    enum _status { Free, Occupied }

    [SerializeField]
    Vector2 _widthHeight;

    [SerializeField] 
    private static Vector2 _instance;
    [SerializeField] 
    public static Vector2 Instance { get { return _instance; } }

    List<int>[,,] _overworld = new List<int>[0,0,0];

    [SerializeField]GameObject _tile;

    // Start is called before the first frame update
    void Start()
    {
        _overworld = new List<int>[Mathf.FloorToInt(_widthHeight.x), Mathf.FloorToInt(_widthHeight.y), 0];
        for (int x = 0; x < _widthHeight.x; x++)
        {
            for (int y = 0; y < _widthHeight.y; y++)
            {
                //_overworld[x, y, (int)_occupation.GroundType] =  ((int)_groundType.Unset);
                //print(_overworld[x, y, (int)_occupation.GroundType]);
                //print(_overworld[x, y, (int)_occupation.Actors].Equals(0)? _status.Free : _status.Occupied);
                Instantiate(_tile, new Vector3(x,0, y), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
