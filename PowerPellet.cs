using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPellet : MonoBehaviour
{
    private const int Score = 5;

    void Start()
    {
        GameManager.instance.AddPellet();
    }

    void OnTriggerEnter(Collider col)
    {
        //Changes ghost state to frightened
        if (col.tag != "Player") return;
        
        GameManager.instance.ReducePellet(Score);
        GameManager.instance.isFrightened = true;
        Destroy(gameObject);
    }
}
