using System.Collections.Generic;
using UnityEngine;

public static class AStar 
{
   /// <summary>
   /// Builds a path for the room, from the startGridPosition to the endGridPosition and adds
   /// movement steps to the returned stack. returns null if no path is found
   /// </summary>
   
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        //Adjust position by lower bounds 
        startGridPosition -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;
         
        //create open list and closed hashset
        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();

        //create gridnode for pathfinding
        GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y -
            room.templateLowerBounds.y + 1);

        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, room.instantiatedRoom);

        if (endPathNode != null)
        {
            return CreatePathStack(endPathNode, room);
        }
        return null;
    }

    /// <summary>
    /// find the shortest path - returns end node if a path has been found else return null
    /// </summary>
    private static Node FindShortestPath (Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet,
        InstantiatedRoom instantiatedRoom)
    {
        //Add start node to open list
        openNodeList.Add(startNode);

        //Loop through open node list 
        while (openNodeList.Count > 0)
        {
            //sort list
            openNodeList.Sort();

            //current node = the node in the open list with the lowest fcost
            Node currenNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            //if the current node = target node then finish
            if (currenNode ==  targetNode)
            {
                return currenNode;
            }

            //add current node to the closed list
            closedNodeHashSet.Add(currenNode);

            //evaluate fcost for each neighbour of the current node
            EvaluateCurrentNodeNeighbour(currenNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, instantiatedRoom);
        }
        return null;
    }

    /// <summary>
    /// create a stack Vector3 containing movement path
    /// </summary>
    private static Stack<Vector3> CreatePathStack(Node targeNode, Room room)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Node nextNode = targeNode;

        //get mid point of cell
        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while (nextNode != null)
        {
            //Convert grid position to world position
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.templateLowerBounds.x,
                nextNode.gridPosition.y + room.templateLowerBounds.y, 0));

            //set world psoition to the middle of the grid cell
            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);
            nextNode = nextNode.parentNode;
        }
        return movementPathStack;
    }
    private static void EvaluateCurrentNodeNeighbour(Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet,
        InstantiatedRoom instantiatedRoom)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode; 

        //Loop through all directions
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes, closedNodeHashSet,
                    instantiatedRoom);

                if (validNeighbourNode != null)
                {
                    //calculate new gcost for the neighbour
                    int newCostToNeighbour;

                    // get movement penalty
                    // unwalkable paths have a value of 0. Default penalty is set in
                    // settings and applies to other grid squares
                    int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x,
                        validNeighbourNode.gridPosition.y];

                    newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                    bool isValidNeighbourNodeinOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeinOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if(!isValidNeighbourNodeinOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }

                }
            }
        }
    }

    /// <summary>
    /// return the distance between nodeA and nodeB
    /// </summary>
    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeB.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX); 
    }

    /// <summary>
    /// Evaluate a node neighbour at neighbournodeXPosition, neighbournodeYPosition using the
    /// specified gridNodes, closedNodeHashSet, and instantiated room. Returns null if node isnt valid
    /// </summary>
    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNodes gridNodes, HashSet<Node> closedNodeHashSet,
        InstantiatedRoom instantiatedRoom)
    {
        // if neighbour node is beyond grid then return null
        if (neighbourNodeXPosition >= instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x || neighbourNodeXPosition < 0
            || neighbourNodeYPosition >= instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y || neighbourNodeYPosition < 0)
        {
            return null;
        }

        //get neighbour node
        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        //check for obstacle at location 
        int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];    

        //if neighbour node is in an obstacle or is in closed list then skip
        if (movementPenaltyForGridSpace == 0 || closedNodeHashSet.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }
}
