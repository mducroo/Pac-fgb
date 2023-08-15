using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    //Spawnrate
    public float spawnRate;
    public List<GameObject> pickupList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SpawnTimer");
    }

    IEnumerator SpawnTimer(){
        while(true){
            yield return new WaitForSeconds(spawnRate);

            int num = Random.Range(0, pickupList.Count);
            Instantiate(pickupList[num], transform.position, Quaternion.identity);

        }
    }

}