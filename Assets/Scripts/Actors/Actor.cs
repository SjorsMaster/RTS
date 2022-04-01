using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    protected int _health = 10;
    protected int _damage = 5;
    protected int _armor = 0;
    [SerializeField] protected int _travelDistance = 1;
    [SerializeField] protected int _attackingRange = 1;

    [SerializeField] TextMesh _stats;

    public int GetSteps() { return _travelDistance; }
    public int GetRange() { return _attackingRange; }
    public int GetDamage() { return _damage; }

    private void Awake() => updateStats();

    public void SetDamage(int atk)
    {
        _armor = _armor - _damage;
        int tmp = _armor;
        if (tmp < 0)
        {
            _health = _health + tmp;
            _armor = 0;
        }
        updateStats();
        if (_health <= 0)
            Destroy(gameObject);
    }

    protected void updateStats()
    {
        if (_stats)
            _stats.text = $"HP:{_health}\nATK:{_damage}\nDEF:{_armor}\nSTEPS:{_travelDistance}\nRANGE:{_attackingRange}\n|";
    }
}
