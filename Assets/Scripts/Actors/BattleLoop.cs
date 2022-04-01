using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleLoop : MonoBehaviour
{
    Dictionary<int, Vector3Int> _characters = new Dictionary<int, Vector3Int>();
    MoveCam _camera;

    private GameObject _currentChar, _targetTile;

    List<Actor> _charCollection, _enemyCollection;
    List<GameObject> _moved;

    [SerializeField] UnityEvent _showBars, _removeBars;

    bool _playerTurn, _startedEnemyCoroutine;

    [SerializeField] Text _text;
    //battle range
    //moving range
    //compare pos with enemy

    public void AddChar(GameObject tmp, Vector3Int pos)
    {
        _characters.Add(tmp.GetInstanceID(), pos);
    }

    private void Update()
    {
        if(_enemyCollection.Count <= 0)
        {
            _text.text = "You win!";
            return;
        }
        if (_charCollection.Count <= 0)
        {
            _text.text = "You lose!";
            return;
        }

        if (_playerTurn)
        {
            PlayerTurn();
        }
        else
        {
            if (!_startedEnemyCoroutine)
            {
                StartCoroutine(EnemyTurn());
                _startedEnemyCoroutine = true;
            }
        }
    }

    void Start()
    {
        _moved = new List<GameObject>();
        _camera = FindObjectOfType<MoveCam>();
        _charCollection = new List<Actor>();
        _enemyCollection = new List<Actor>();
        UpdateEnemies();
        UpdatePlayers();
    }

    GameObject GetClosest(Actor[] players, Transform currentPos)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = currentPos.position;
        foreach (Character potentialTarget in players)
        {
            Vector3 directionToTarget = potentialTarget.gameObject.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.gameObject.transform;
            }
        }
        return bestTarget.transform.gameObject;
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(2);
        _showBars.Invoke();
        UpdateEnemies();

        foreach (var enemy in _enemyCollection)
        {
            UpdatePlayers();

            if (_charCollection.Count <= 0) break; //Exit case if there's no more enemies

            _camera.NextPos(enemy.transform.position);
            yield return new WaitForSeconds(.5f);
            GameObject target = GetClosest(_charCollection.ToArray(), enemy.transform);
            Actor actorSelf = enemy.GetComponent<Actor>();
            Vector3Int pos = Vector3Int.FloorToInt((target.transform.position));
            int stepsToTarget = enemy.GetComponent<FollowPath>().GetReady(pos);
            int allowedSteps = enemy.GetComponent<Actor>().GetSteps();
            if (stepsToTarget <= actorSelf.GetRange() + 1)
                //Attack
                target.GetComponent<Character>().SetDamage(actorSelf.GetDamage());
            else
                //Move to target, making sure they don't move into you
                enemy.GetComponent<FollowPath>().Go(allowedSteps - stepsToTarget >= -1 ? stepsToTarget - 2 : allowedSteps);
            yield return new WaitForSeconds(2);
        }
        //return to player turn
        _camera.EndTurn();
        _removeBars.Invoke();
        _startedEnemyCoroutine = false;
        _playerTurn = true;
        _moved.Clear();
        yield return null;

    }

    public void ClickedChar(GameObject obj)
    {
        if (_moved.Contains(obj)) return;
        _currentChar = obj;
        _targetTile = null;
    }
    public void ClickedTile(GameObject obj)
    {
        _targetTile = obj;
    }

    void UpdateEnemies()
    {
        _enemyCollection.Clear();
        _enemyCollection.AddRange(FindObjectsOfType<EnemyBase>());
    }
    void UpdatePlayers()
    {
        _charCollection.Clear();
        _charCollection.AddRange(FindObjectsOfType<Character>());
    }

    public void EndTurn() => _playerTurn = false;

    void PlayerTurn()
    {
        if (_currentChar && _targetTile)
        {
            int allowedSteps = _currentChar.GetComponent<Actor>().GetSteps();
            Vector3Int targetTile = Vector3Int.FloorToInt(_targetTile.transform.position);
            int steps = _currentChar.gameObject.GetComponent<FollowPath>().GetReady(targetTile);
            UpdateEnemies();
            foreach (var enemy in _enemyCollection)
            {
                if (targetTile == enemy.transform.position || targetTile == enemy.transform.position - Vector3.up)
                {
                    if (_currentChar.GetComponent<Actor>().GetRange() + 1 >= steps)
                    {
                        enemy.GetComponent<EnemyBase>().SetDamage(_currentChar.GetComponent<Actor>().GetDamage());
                        allowedSteps = 0;
                    }
                    else if (allowedSteps - steps >= -1)
                    {
                        allowedSteps = steps - 2;
                    }
                }
            }

            _currentChar.gameObject.GetComponent<FollowPath>().Go(allowedSteps);
            _moved.Add(_currentChar);
            _currentChar = null;
            _targetTile = null;
        }
        if (_moved.Count >= _charCollection.Count)
        {
            EndTurn();
        }
    }
}
