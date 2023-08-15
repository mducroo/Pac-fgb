using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PacMan : MonoBehaviour
{
    public float speed = 5f;
    public AudioSource Pelletsource;
    public AudioSource PowerPelletsource;
    public AudioSource Pickupsource;

    private readonly Vector3 _up = Vector3.zero;

    private readonly Vector3 _right = new Vector3(0,90,0);

    private readonly Vector3 _down = new Vector3(0,180,0);

    private readonly Vector3 _left = new Vector3(0,270,0);

    private Vector3 _currentDirection = Vector3.zero;

    private Vector3 _nextPos, _destination; //direction;

    private bool _canMove;

    public LayerMask unwalkable;

    //reset
    private Vector3 _initPosition;

    void Start()
    {
        //Same position always
        _initPosition = transform.position;
        Reset();
    }
    public void Reset(){
        transform.position = _initPosition;
        _currentDirection = _up;
        _nextPos = Vector3.forward;
        _destination = transform.position;           
    }
    // Update is called once per frame
    public void Update()
    {
        //Keys pressed, move
        Move(); 
    }

    private void Move(){
        transform.position = Vector3.MoveTowards(transform.position, _destination, speed * Time.deltaTime);
        //key inputs
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)){
            _nextPos = Vector3.forward;
            _currentDirection = _up;
        }
        else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)){
            _nextPos = Vector3.back;
            _currentDirection = _down;

        }
        else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
            _nextPos = Vector3.left;
            _currentDirection = _left;

        }
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
            _nextPos = Vector3.right;
            _currentDirection = _right;

        }

        if (!(Vector3.Distance(_destination, transform.position) < 0.00001f)) return;
        transform.localEulerAngles = _currentDirection;
        {
            if(Valid()){
                _destination = transform.position + _nextPos;
            }
        }
    }

    private bool Valid(){
        var myRay = new Ray(transform.position + new Vector3(0, 0.25f, 0), transform.forward);
        //Cant walk through Walls
        if (!Physics.Raycast(myRay, out var myHit, 1f, unwalkable)) return true;
        return myHit.collider.tag != "Wall";
    }
    public void OnTriggerEnter(Collider col){
        if(col.tag == "Ghost"){
            //	Ghosts when they arent frightened or eaten, -> lose life If frightened -> add to score
            var pGhost = col.GetComponent<Pathfinding>();
            if(pGhost.state == Pathfinding.GhostStates.Frightened){
                pGhost.state = Pathfinding.GhostStates.GotEaten;
                //score ++
                GameManager.AddScore(400);
                
            }    
            else if (pGhost.state != Pathfinding.GhostStates.Frightened && pGhost.state != Pathfinding.GhostStates.GotEaten){
                //lose a life
                GameManager.instance.LoseLife();
            }    
        }
        //Audio when collides
        switch (col.tag)
        {
            case "Pellet":
                Pelletsource.Play();
                break;
            case "PowerPellet":
                PowerPelletsource.Play();
                break;
            case "Pickups":
                Pickupsource.Play();
                break;
        }
    }
}