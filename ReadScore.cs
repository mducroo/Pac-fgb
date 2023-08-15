using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ReadScore : MonoBehaviour
{
    //Gameover overlay + scoreholder
    public Text highscoreText, levelText;

    void Start()
    {
        highscoreText.text = "Highscore: " + ScoreHolder.Score;
        levelText.text = "Best Level: " + ScoreHolder.Level;
    }

}
