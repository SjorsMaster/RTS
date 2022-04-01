using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    bool _followPath;
    List<Vector3Int> _path = new List<Vector3Int>();

    Vector3Int _start, _end;

    GridBehaviour _gridBehaviour;

    int _iteration, _maxSteps;

    private void Start()
    {
        _gridBehaviour = GameObject.FindObjectOfType<GridBehaviour>().GetComponent<GridBehaviour>();
    }
    //ForAI
    //if list is < attack dist, attack

    void Update()
    {
        if (!_followPath) return;
        Vector3Int offset = Vector3Int.up * Mathf.FloorToInt(transform.position.y);

        if (Vector3.Distance(transform.position, _path[_iteration] + offset) > .1f)
        {
            StartCoroutine(MoveToPoint(offset));
        }
        else
        {
            transform.position = _path[_iteration] + offset;
            _iteration++;
        }
        if (_path.Count <= _iteration || _iteration >= _maxSteps) _followPath = false;
    }

    void GetPath()
    {
        _iteration = 0;
        _path.Clear();
        _path = _gridBehaviour.GetPath(_start, _end);
    }

    public int GetReady(Vector3Int endpos)
    {
        _start = new Vector3Int(Mathf.FloorToInt(transform.position.x), 0, Mathf.FloorToInt(transform.position.z));
        _end = new Vector3Int(Mathf.FloorToInt(endpos.x), 0, Mathf.FloorToInt(endpos.z));

        GetPath();
        return _path.Count;
    }

    public void Go(int maxSteps)
    {
        _maxSteps = maxSteps + 1;
        if (_path.Count > 0)
            _followPath = true;
    }

    IEnumerator MoveToPoint(Vector3Int offset)
    {
        transform.position = Vector3.Lerp(transform.position, _path[_iteration] + offset, 10f * Time.deltaTime);
        yield return new WaitForSeconds(.5f);
    }
}
