using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node{
    public int gCost;
    public int hCost;

    public int fCost{
        get{ return gCost + hCost;}
    }
    
    public Node parentNode;

    public int posX;
    public int posZ;
    public int state; // 0 = free, 1 = obstacle, 2 = start, 3 = goal, 4 = pacman

    public bool walkable;

    public Node(int _posX, int  _posZ, int _state, bool _walkable)
    {
        posX = _posX;
        posZ = _posZ;
        state = _state;
        walkable = _walkable;
    }
}