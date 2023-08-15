using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    //Set the amount of score each pellet gives when eaten
    //When pacman collides with it it get destroyed and amount score gets added
    int score = 1;

    void Start()
    {
        GameManager.instance.AddPellet();   
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Player") return;
        
        GameManager.instance.ReducePellet(score);
        Destroy(gameObject);
    }
}