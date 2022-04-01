using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Actor
{
    [SerializeField] ParticleSystem _particleSystem;
    public void Boost(Item item)
    {
        _health += item.hp;
        _damage += item.dmg;
        _armor += item.def;
        _travelDistance += item.steps;
        _attackingRange += item.range;
        if(_particleSystem) _particleSystem.Play();

        updateStats();
    }

}
