using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance; 
    public TMPro.TMP_Text scoreText , levelText, lifesText;

    void Awake(){
        instance = this;
    }

    //Sets text for overlay
    public void UpdateUI(){
        scoreText.text = "Score: " + GameManager.GetScore();
        levelText.text = "Level: " + GameManager.GetCurrentLevel();
        lifesText.text = "Lives: " + GameManager.GetLives();

    }

}
