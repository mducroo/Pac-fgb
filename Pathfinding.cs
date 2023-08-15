using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    //enum = a list of int
    public enum Ghosts{
        Blinky,
        Clyde,
        Inky,
        Pinky
    }                      

    public Ghosts ghost;

    public Transform blinky;

    //Pathfinding
    List<Node> path = new List<Node>();
    private const int D = 10; //heuristic distance - cost per step
    public Grid grid;
    private Node _lastVisitedNode;

    //Get current position of ghost and target
    private Transform _currentTarget;
    public Transform pacManTarget;
    public List<Transform> homeTarget = new List<Transform>();
    public List<Transform> scatterTarget = new List<Transform>();

    
    //Movement
    private float _speed = 4f;
    private Vector3 _nextPos, _destination;

    //Direction
    private readonly Vector3 _up = Vector3.zero;

    private readonly Vector3 _right = new Vector3(0,90,0);

    private readonly Vector3 _down = new Vector3(0,180,0);

    private readonly Vector3 _left = new Vector3(0,270,0);

    private Vector3 _currentDirection = Vector3.zero;

    //StateMachine
    public enum GhostStates{
        Home,
        Chase,
        Scatter,
        Frightened,
        GotEaten,
    }
    public GhostStates state;

    //Appearance
    public int activeApperance;// 0 = normal, 1 = frightend, 2 = GotEaten
    public GameObject[] apperance;

    //HomeTimer
    public float timer = 5f;       //5 seconds
    public float curTime = 0;      //current time

    //Info to release the ghosts
    public int pointsToCollect;
    public bool released = false;

    //reset state
    private Vector3 _initPosition;
    private GhostStates _initState;

    void Start(){
        var position = transform.position;
        _initPosition = position;
        _initState = state;

        _destination = position;
        _currentDirection = _up;
        foreach (var t in scatterTarget)
        {
            t.GetComponent<MeshRenderer>().enabled = false;
        }
        foreach (var t in homeTarget)
        {
            t.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    void Update(){

        CheckState();
    }

    private void FindThePath(){
        var startNode = grid.NodeRequest(transform.position); //current (ghost) position in grid
        var goalNode = grid.NodeRequest(_currentTarget.position); //pacmans position in grid

        var openList = new List<Node>();
        var closedList = new List<Node>();

        //add start node
        openList.Add(startNode);
        //keep Looping
        while(openList.Count > 0){
            var currentNode = openList[0];
            for (var i = 1; i < openList.Count; i++){

                if(openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            //goal has been found
            if(currentNode == goalNode){
                //get path before we exit
                PathTracer(startNode, goalNode);
                return;
            }
            //check all neighbours, except backwards
            foreach(var neighbour in grid.GetNeighbourNodes(currentNode)){
                if(!neighbour.walkable || closedList.Contains(neighbour) || neighbour == _lastVisitedNode){
                    continue;
                }
                var calcMoveCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (calcMoveCost >= neighbour.gCost && openList.Contains(neighbour)) continue;
                neighbour.gCost = calcMoveCost;
                neighbour.hCost = GetDistance(neighbour, goalNode);

                neighbour.parentNode = currentNode;
                if(!openList.Contains(neighbour)){
                    openList.Add(neighbour);
                }
            }
            _lastVisitedNode = null;
        }
    }
    void PathTracer(Node startNode, Node goalNode){
        _lastVisitedNode = startNode;

        path.Clear();
        //List<Node>path = new List<Node>();
        var currentNode = goalNode;
        while(currentNode != startNode){
            path.Add(currentNode);
            currentNode = currentNode.parentNode;            
        }
        //reverse path to get it sorted right
        path.Reverse();
        switch (ghost)
        {
            //grid.path = path;
            case Ghosts.Blinky:
                grid.blinkyPath = path;
                break;
            case Ghosts.Clyde:
                grid.clydePath = path;
                break;
            case Ghosts.Inky:
                grid.inkyPath = path;
                break;
            case Ghosts.Pinky:
                grid.pinkyPath = path;
                break;
        }
    }
    int GetDistance(Node a, Node b){
        int distX = Mathf.Abs(a.posX - b.posX);
        int distZ = Mathf.Abs(a.posZ - b.posZ);

        return D * (distX + distZ);
    }
    void Move(){
        transform.position = Vector3.MoveTowards(transform.position,_destination, _speed * Time.deltaTime);
        if (!(Vector3.Distance(transform.position, _destination) < 0.0001f)) return;
        //Find the Path
        FindThePath();

        if (path.Count <= 0) return;
        //destination
        _nextPos = grid.NextPathpoint(path[0]);
        _destination = _nextPos;

        //rotation
        SetDirection();
        transform.localEulerAngles = _currentDirection;
    }
    void SetDirection(){
        int dirX = (int)(_nextPos.x - transform.position.x);
        int dirZ = (int)(_nextPos.z - transform.position.z);

        _currentDirection = dirX switch
        {
            //up
            0 when dirZ > 0 => _up,
            //right
            > 0 when dirZ == 0 => _right,
            //left
            < 0 when dirZ == 0 => _left,
            //down
            0 when dirZ < 0 => _down,
            _ => _currentDirection
        };
    }
    void CheckState(){
        switch(state){
            //Behaviour in home
            case GhostStates.Home:
                activeApperance = 0;
                SetAppearance();
                _speed = 2f;
                if(!homeTarget.Contains(_currentTarget)){
                    _currentTarget = homeTarget[0];
                }
                for(int i = 0; i < homeTarget.Count; i++)
                {
                    if (!(Vector3.Distance(transform.position, homeTarget[i].position) < 0.0001f) ||
                        _currentTarget != homeTarget[i]) continue;
                    i++;
                    if(i>= homeTarget.Count){
                        i = 0;
                    }
                    _currentTarget = homeTarget[i];
                    continue;
                }

                if(released){
                    //Chase mode when released
                    curTime += Time.deltaTime;
                    if(curTime >= timer){
                        curTime = 0;
                        state = GhostStates.Chase;
                    }
                }
                break;
            
            case GhostStates.Chase:
                activeApperance = 0;
                SetAppearance();
                _speed = 3f;
                switch (ghost)
                {
                    case Ghosts.Clyde when Vector3.Distance(transform.position, pacManTarget.position) <= 8:
                    {   //Clyde keeps his distance
                        if(!scatterTarget.Contains(_currentTarget)){
                            _currentTarget = scatterTarget[0];
                        }

                        for(var i = 0; i < scatterTarget.Count; i++)
                        {
                            if (!(Vector3.Distance(transform.position, scatterTarget[i].position) < 0.0001f) ||
                                _currentTarget != scatterTarget[i]) continue;
                            i++;
                            if(i>= scatterTarget.Count){
                                i = 0;
                            }
                            _currentTarget = scatterTarget[i];
                        }

                        break;
                    }
                    //Set behaviours for other ghosts
                    case Ghosts.Clyde:
                        _currentTarget = pacManTarget;
                        break;
                    case Ghosts.Blinky:
                        _currentTarget = pacManTarget;
                        break;
                    case Ghosts.Pinky:
                        PinkyBehavior();
                        break;
                    case Ghosts.Inky:
                        InkyBehavoir();
                        break;
                }
                break;

            case GhostStates.Scatter:
                //Behaviour in scatter mode, follow set targets
                activeApperance = 0;
                SetAppearance();
                _speed = 3f;

                if(!scatterTarget.Contains(_currentTarget)){
                    _currentTarget = scatterTarget[0];
                }
                
                for(var i = 0; i < scatterTarget.Count; i++)
                {
                    if (!(Vector3.Distance(transform.position, scatterTarget[i].position) < 0.0001f) ||
                        _currentTarget != scatterTarget[i]) continue;
                    i++;
                    if(i>= scatterTarget.Count){
                        i = 0;
                    }
                    _currentTarget = scatterTarget[i];
                }
                break;

            case GhostStates.Frightened:
                //Behaviour when frightened, follow scatter targets, changed apperance
                activeApperance = 1;
                SetAppearance();
                _speed = 1.5f;
                if(!scatterTarget.Contains(_currentTarget)){
                    _currentTarget = scatterTarget[0];
                }
                
                for(var i = 0; i < scatterTarget.Count; i++)
                {
                    if (!(Vector3.Distance(transform.position, scatterTarget[i].position) < 0.0001f) ||
                        _currentTarget != scatterTarget[i]) continue;
                    i++;
                    if(i>= scatterTarget.Count){
                        i = 0;
                    }
                    _currentTarget = scatterTarget[i];
                }
                break;

            case GhostStates.GotEaten:
                //Go back home when eaten
                activeApperance = 2;
                SetAppearance();
                _speed = 15f;

                _currentTarget = homeTarget[0];
                if(Vector3.Distance(transform.position, homeTarget[0].position) < 0.0001f){
                    state = GhostStates.Home;
                }

                break;

        }
        
        Move();
    }
    public void SetAppearance(){
        for(var i = 0; i < apperance.Length; i++){
            apperance[i].SetActive(i == activeApperance); 
        }
    }

    void PinkyBehavior(){
        //Looks ahead of pacman
        Transform aheadTarget = new GameObject().transform;
        int lookAhead = 4;
        //Set a Target
        aheadTarget.position = pacManTarget.position + pacManTarget.transform.forward * lookAhead;
        for(var i = lookAhead; i > 0; i--){
            if(!grid.CheckInsideGrid(aheadTarget.position)){
                lookAhead--;
                aheadTarget.position = pacManTarget.position + pacManTarget.transform.forward * lookAhead;
            }
            else{
                break;
            }  
        }
        aheadTarget.position = pacManTarget.position + pacManTarget.transform.forward * lookAhead;
        Debug.DrawLine(transform.position, aheadTarget.position);
        _currentTarget = aheadTarget;

        Destroy(aheadTarget.gameObject);
    }
    void InkyBehavoir(){
        //Follows the path blinky takes to pacman
        Transform blinkyToPacMan = new GameObject().transform;
        Transform target = new GameObject().transform;
        Transform goal = new GameObject().transform;

        blinkyToPacMan.position = new Vector3(pacManTarget.position.x -blinky.position.x, 0, pacManTarget.position.z - blinky.position.z);
        target.position = new Vector3(pacManTarget.position.x + blinkyToPacMan.position.x, 0, pacManTarget.position.z + blinkyToPacMan.position.z);

        goal.position = grid.GetNearestNonWallNode(target.position);
        _currentTarget = goal;
        Debug.DrawLine(transform.position, _currentTarget.position);

        Destroy(target.gameObject);
        Destroy(blinkyToPacMan.gameObject);
        Destroy(goal.gameObject);

    }
    public void Reset(){
        //Reset when dead
        transform.position = _initPosition;
        state = _initState;
        _destination = transform.position;
        _currentDirection = _up;
    }
}


    
