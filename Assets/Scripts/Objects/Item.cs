using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items", order = 1)]
public class Item : ScriptableObject
{
    [Header("Stats")]
    [Tooltip("The amount of additional health the character will get when this item applied.")]
    public int hp;
    [Tooltip("The amount of additional damage the character will get when this item applied.")]
    public int dmg;
    [Tooltip("The amount of additional defence the character will get when this item applied.")]
    public int def;
    [Tooltip("The amount of additional distance the character can travel when this item applied.")]
    public int steps;
    [Tooltip("The additional attack range the character obtains when this item applied.")]
    public int range;
    [Tooltip("Object description, usually contains it's additional stats.")]
    public string description;
    [Header("Required")]
    [Tooltip("Object icon.")]
    public Sprite icon;
}
