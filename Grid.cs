using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public GameObject bottomLeft, topRight;
    //DEBUG
    
    Node[,] myGrid;

    public List<Node> clydePath;
    public List<Node> blinkyPath;
    public List<Node> pinkyPath;
    public List<Node> inkyPath;

    [SerializeField] private LayerMask unwalkable;

    //GRID INFO
    private int _xStart, _zStart;

    private int _xEnd, _zEnd;

    private int _vCells, _hCells; //amount of cells in grid

    private const int CellWidth = 1;
    private const int CellHeight = 1;

    void Awake(){
        MPGridCreate();
    }

    //Get coordinates from toprigh and bottomleft
    // Calc distance / how many cubes fit
    void MPGridCreate(){
        _xStart = (int) bottomLeft.transform.position.x;
        _zStart = (int) bottomLeft.transform.position.z;

        _xEnd = (int) topRight.transform.position.x;
        _zEnd = (int) topRight.transform.position.z;

        _hCells = (int) ((_xEnd-_xStart)/CellWidth);
        _vCells = (int) ((_zEnd-_zStart)/CellHeight);


        myGrid = new Node[_hCells,_vCells+1];

        UpdateGrid();

    }
    public void UpdateGrid(){
        for(var i = 0; i < _hCells; i++)
        {
            for(var j = 0; j <= _vCells; j++)
            {
                var walkable = !(Physics.CheckSphere(new Vector3(_xStart + i,0, _zStart + j), 0.4f, unwalkable));
                myGrid[i, j] = new Node(i, j, 0, walkable);
            }
        }
    }

    //Visualise grid and paths with colors
    void OnDrawGizmos()
    {
        if (myGrid == null) return;
        foreach(var node in myGrid)
        {
            Gizmos.color = (node.walkable)?Color.white:Color.red;
            if(clydePath != null){
                if(clydePath.Contains(node)){
                    Gizmos.color = Color.yellow;
                }
            }
            if(blinkyPath != null){
                if(blinkyPath.Contains(node)){
                    Gizmos.color = Color.green;
                }
            }
            if(inkyPath != null){
                if(inkyPath.Contains(node)){
                    Gizmos.color = Color.cyan;
                }
            }
            if(pinkyPath != null){
                if(pinkyPath.Contains(node)){
                    Gizmos.color = Color.magenta;
                }
            }
            Gizmos.DrawWireCube(new Vector3(_xStart + node.posX, 0.5f, _zStart + node.posZ), new Vector3(0.9f, 0.9f, 0.9f));
        }
    }

    public Node NodeRequest(Vector3 pos){
        int gridX = (int)Vector3.Distance(new Vector3(pos.x,0,0), new Vector3(_xStart,0,0));
        int gridZ = (int)Vector3.Distance(new Vector3(0,0,pos.z), new Vector3(0,0,_zStart));

        return myGrid[gridX, gridZ];
    }

    public Vector3 NextPathpoint(Node node){
        int gridX = (int)(_xStart+node.posX);
        int gridZ = (int)(_zStart+node.posZ);

        return new Vector3(gridX, 0, gridZ);
    }

    public List<Node> GetNeighbourNodes(Node node){
        List<Node> neighbours = new List<Node>();
        //find all neighbours in a 3x3 square around current node
        for (var x = -1; x <= 1; x++){
            for (var z = -1; z <= 1; z++){
                switch (x)
                {
                    //ignore following fields
                    case 0 when z == 0:
                    //ignore top Left
                    case -1 when z == 1:
                    //ignore top Right
                    case 1 when z == 1:
                    //ignore bottom Left
                    case 1 when z == -1:
                    //ignore bottom Right
                    case -1 when z == -1:
                        continue;
                }

                int checkPosX = node.posX + x;
                int checkPosZ = node.posZ + z;

                if(checkPosX >= 0 && checkPosX <= (_hCells) && checkPosZ >= 0 && checkPosZ < (_vCells)){
                    neighbours.Add(myGrid[checkPosX, checkPosZ]);
                }

            }

        }
        
        return neighbours;
    }

    public bool CheckInsideGrid(Vector3 requestedPosition){
        int gridX = (int)(requestedPosition.x -_xStart);
        int gridZ = (int)(requestedPosition.z -_zStart);

        if(gridX > _hCells -1){
            return false;
        }
        if(gridX < 0){
            return false;    
        }
        if(gridZ > _vCells -1){
            return false;    
        }
        if(gridZ < 0){
            return false;    
        }
        if(!NodeRequest(requestedPosition).walkable){
            return false;
        }
        return true;
    }
    public Vector3 GetNearestNonWallNode(Vector3 target){
        float min = 1000;
        int minIndexI = 0;
        int minIndexJ = 0;

        for(var i = 0; i < _hCells; i++){
            for(var j = 0; j < _vCells; j++)
            {
                if (!myGrid[i, j].walkable) continue;
                
                Vector3 nextPoint = NextPathpoint(myGrid[i,j]);
                float distance = Vector3.Distance(nextPoint, target);
                
                if (!(distance < min)) continue;
                
                min = distance;
                minIndexI = i;
                minIndexJ = j;
            }
        }
        return NextPathpoint(myGrid[minIndexI,minIndexJ]);
    }
}
