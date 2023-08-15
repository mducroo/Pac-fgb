using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //When space is pressed it loads the next scene
    public int buildIndex;
    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Space)){
            SceneManager.LoadScene(buildIndex);
       }
    }
}
