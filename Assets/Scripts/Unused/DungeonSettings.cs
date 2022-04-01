using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSettings : ScriptableObject
{
    public int roomAmount = 3;
    public int width = 50;
    public int height = 50;
    public int minRoomSize = 4;
    public int maxRoomSize = 10;
    public int seed = 420; 
}
