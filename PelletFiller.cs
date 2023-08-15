using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletFiller : MonoBehaviour
{   
    //Fills field with pellet (where walkable)
    public int hCells;
    public int vCells;

    public GameObject bottomLeft, topRight;
    public GameObject prefab;
    public GameObject pelletHolder;

    public bool active;

}