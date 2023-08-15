using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float destroyTime = 30;
    public int scoreAmount;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    //When pacman collides with pickups, add score
    void OnTriggerEnter(Collider col){
        if(col.tag == "Player"){
            GameManager.AddScore(scoreAmount);
            Destroy(gameObject);
        }
    }
}
