using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private static int _score;
    private int _pelletAmount;
    private static int _curLevel = 1;
    private static int _lives = 3;
    private const int AmountOfLevels = 2;

    public List<GameObject> ghostList = new List<GameObject>();
    public GameObject pacmanObj;

    //bools
    public bool isScattering;
    public bool isChasing;
    public bool isFrightened;

    private static bool _hasLost;

    //isFrightenedTimer
    private const float FrightenedDurationS = 5f; // in seconds
    private float _curFrightenedTime = 0f; // Counter for current iteration

    //ChaseTimer
    private const float ChaseDurationS = 7f;
    private float _curChaseTime = 0f;

    //ScatterTimer
    private const float ScatterDurationS = 6f;
    private float _curScatterTime = 0f;


    void Awake(){
        instance = this;
    }

    void Start(){
        if(_hasLost){
            _score = 0;
            _lives = 3;
            _curLevel = 1;
            _hasLost = false;
        }

        //Debug.Log("score: " + score + "current level: " + cur_level);
        isScattering = true;
        // Assign unity game objects to variable
        ghostList.AddRange(GameObject.FindGameObjectsWithTag("Ghost"));
        pacmanObj = GameObject.FindGameObjectWithTag("Player");

        UIManager.instance.UpdateUI();
    }
    void Update(){
        //Set behaviour of ghosts according to time passed in regards to duration of behaviour
        UpdateGhostBehaviour();
    }

    public void AddPellet(){
        _pelletAmount++;
    }

    public void ReducePellet(int amount){
        //	Pellets get reduced and the score goes up
        _pelletAmount--;
        _score += amount;
        UIManager.instance.UpdateUI();

        if(_pelletAmount <= 0){
            //win the level
            WinCondition();

        }
        foreach (var t in ghostList)
        {
            // Release each ghost after player reaches according score threshold
            Pathfinding pGhost = t.GetComponent<Pathfinding>();
            
            if (_score < pGhost.pointsToCollect || pGhost.released) continue;
            
            pGhost.state = Pathfinding.GhostStates.Chase;
            pGhost.released = true;
        }
    }
    void UpdateGhostBehaviour(){
        // Set behaviour of ghosts according to time passed in regards to duration of behaviour.
        UpdateGhostStates();
        if(isChasing){
            _curChaseTime += Time.deltaTime;
            if(_curChaseTime >= ChaseDurationS){
                _curChaseTime = 0;
                isChasing = false;
                isScattering = true;
            }
        }
        if(isScattering){
            _curScatterTime += Time.deltaTime;
            if(_curScatterTime >= ScatterDurationS){
                _curScatterTime = 0;
                isChasing = true;
                isScattering = false;
            }
        }

        if (!isFrightened) return;
        if(_curChaseTime != 0 || _curScatterTime != 0){
            isScattering = false;
            isChasing = false;
            isFrightened = true;
            _curChaseTime = 0;
            _curScatterTime = 0;
        }
        _curFrightenedTime += Time.deltaTime;
        
        if (!(_curFrightenedTime >= FrightenedDurationS)) return;
        _curFrightenedTime = 0;
        isChasing = true;
        isScattering = false;
        isFrightened = false;
    }
    void UpdateGhostStates()
    {
        foreach (var pGhost in ghostList.Select(t => t.GetComponent<Pathfinding>()))
        {
            switch (pGhost.state)
            {
                case Pathfinding.GhostStates.Chase when isScattering:
                    pGhost.state = Pathfinding.GhostStates.Scatter;
                    break;
                case Pathfinding.GhostStates.Scatter when isChasing:
                    pGhost.state = Pathfinding.GhostStates.Chase;
                    break;
                default:
                {
                    if(pGhost.state == Pathfinding.GhostStates.Frightened && isChasing){
                        pGhost.state = Pathfinding.GhostStates.Chase;
                    }
                    else if(isFrightened && pGhost.state != Pathfinding.GhostStates.Home && pGhost.state != Pathfinding.GhostStates.GotEaten){
                        pGhost.state = Pathfinding.GhostStates.Frightened;
                    }
                    else if(pGhost.state == Pathfinding.GhostStates.Home){
                        pGhost.state = Pathfinding.GhostStates.Home;
                    }

                    break;
                }
            }
        }
    }
    void WinCondition(){
        _curLevel ++;
        // Example with two levels
        // buildIndex % 2 produces either 0 or 1, adding one gives the buildIndex for the next level either 1 or 2.
        var nextLevelBuildIndex = (SceneManager.GetActiveScene().buildIndex % AmountOfLevels ) + 1;
        SceneManager.LoadScene(nextLevelBuildIndex);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LoseLife(){
        _lives --;
        UIManager.instance.UpdateUI();
        if(_lives <= 0){
            //game over
            ScoreHolder.Level = _curLevel;
            ScoreHolder.Score = _score;
            //Gameover when lives = 0
            _hasLost = true;
            SceneManager.LoadScene("GameOver");
            return;
        }
        foreach(var ghost in ghostList){
            ghost.GetComponent<Pathfinding>().Reset();
        }
        pacmanObj.GetComponent<PacMan>().Reset();
    }
    //Get info for display
    public static void AddScore(int amount){
        _score += amount;
        UIManager.instance.UpdateUI();
    }
    public static int GetScore(){
        return _score;
    }
    public static int GetCurrentLevel(){
        return _curLevel;
    }
    public static int GetLives(){
        return _lives;
    }
}