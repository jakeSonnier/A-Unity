using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MapGen;

public class Node
{
    //data from the node class
    public Node cameFrom = null; //parent node
    public double priority = 0; // F value
    public double costSoFar = 0; // G Value
    public Tile tile;

    public Node(Tile _tile, double _priority, Node _cameFrom, double _costSoFar)
    {
        cameFrom = _cameFrom;
        priority = _priority; 
        costSoFar = _costSoFar;
        tile = _tile;
    }
}

public class PathFinder
{
    //TODO and done get added as we find a star path
    List<Node> TODOList = new List<Node>();
    List<Node> DoneList = new List<Node>();
    Tile goalTile = null;

    public PathFinder()
    {
    }

    public Queue<Tile> FindPathAStar(Tile start, Tile goal)
    {
        //setup starting cost
        double startNodeHCost = HeuristicsDistance(start, goal);
        Node startNode = new Node(start, startNodeHCost, null, 0);

        //add starting square to todo list
        TODOList.Add(startNode);

        //first element
        Node currentNode = TODOList[0];

        //repeat while we havent found goal
        while(currentNode.tile != goal && TODOList.Count != 0)
        {
            currentNode = TODOList[0];
            TODOList.Remove(currentNode);
            DoneList.Add(currentNode);

            //look at adjacent squares
            foreach(Tile adjTile in currentNode.tile.Adjacents)
            {
                //Why is passable always true?
                if(!DoneList.Exists(n => n.tile == adjTile) && adjTile.isPassable)
                {
                    if(!TODOList.Exists(n => n.tile == adjTile) && adjTile.isPassable)
                    {
                        double adjTileNodeGCost;
                        double adjTileNodeHCost = HeuristicsDistance(adjTile, goal);

                        if (currentNode.tile.indexX == adjTile.indexX + 1 && currentNode.tile.indexY == adjTile.indexY + 1 || currentNode.tile.indexX == adjTile.indexX + 1 && currentNode.tile.indexY == adjTile.indexY - 1 || currentNode.tile.indexX == adjTile.indexX - 1 && currentNode.tile.indexY == adjTile.indexY + 1 || currentNode.tile.indexX == adjTile.indexX - 1 && currentNode.tile.indexY == adjTile.indexY - 1)
                        {
                            //14 for diagonal
                            adjTileNodeGCost = currentNode.costSoFar + 14;
                        }
                        else
                        {
                            //10 for straight
                            adjTileNodeGCost = currentNode.costSoFar + 10;
                        }

                        double adjTileNodeFCost = adjTileNodeGCost + adjTileNodeHCost;
                        Node adjTileNode = new Node(adjTile, adjTileNodeFCost, currentNode, adjTileNodeGCost);

                        TODOList.Add(adjTileNode);
                    }
                    else
                    {
                        //adj tile is in todo list already
                        Node adjTileNode = TODOList.Find(n => n.tile == adjTile);
                        if(adjTileNode.priority < currentNode.priority)
                        {
                            adjTileNode.cameFrom = currentNode;

                            double adjTileNodeGCost;
                            double adjTileNodeHCost = HeuristicsDistance(adjTile, goal);

                            if (currentNode.tile.indexX == adjTile.indexX + 1 && currentNode.tile.indexY == adjTile.indexY + 1 || currentNode.tile.indexX == adjTile.indexX + 1 && currentNode.tile.indexY == adjTile.indexY - 1 || currentNode.tile.indexX == adjTile.indexX - 1 && currentNode.tile.indexY == adjTile.indexY + 1 || currentNode.tile.indexX == adjTile.indexX - 1 && currentNode.tile.indexY == adjTile.indexY - 1)
                            {
                                adjTileNodeGCost = adjTileNode.cameFrom.costSoFar + 14;
                            }
                            else
                            {
                                adjTileNodeGCost = adjTileNode.cameFrom.costSoFar + 10;
                            }

                            double adjTileNodeFCost = adjTileNodeGCost + adjTileNodeHCost;

                            adjTileNode.priority = adjTileNodeFCost;
                            adjTileNode.costSoFar = adjTileNodeGCost;

                            TODOList.Sort((x, y) => x.priority.CompareTo(y.priority));
                        }
                    }
                }
            }
        }

        Node targetNode = currentNode;
        Queue<Tile> finalPath = new Queue<Tile>();
        while (targetNode != null)
        {
            Debug.Log(targetNode.ToString());
            finalPath.Enqueue(targetNode.tile);
            targetNode = targetNode.cameFrom;
        }

        return finalPath; // returns path
    }

    // TODO: Find the path based on A-Star Algorithm
    // In this case avoid a path passing near an enemy tile
    //only can pathfind, no enemy evasion yet
    public Queue<Tile> FindPathAStarEvadeEnemy(Tile start, Tile goal)
    {
        double startNodeHCost = HeuristicsDistance(start, goal);
        Node startNode = new Node(start, startNodeHCost, null, 0);

        //add starting square to todo list
        TODOList.Add(startNode);

        Node currentNode = TODOList[0];

        while (currentNode.tile != goal && TODOList.Count != 0)
        {
            currentNode = TODOList[0];
            TODOList.Remove(currentNode);
            DoneList.Add(currentNode);

            foreach (Tile adjTile in currentNode.tile.Adjacents)
            {
                if (!DoneList.Exists(n => n.tile == adjTile) && !adjTile.isPassable)
                {
                    if (!TODOList.Exists(n => n.tile == adjTile) && !adjTile.isPassable)
                    {
                        double adjTileNodeGCost;
                        double adjTileNodeHCost = HeuristicsDistance(adjTile, goal);

                        if (currentNode.tile.indexX == adjTile.indexX + 1 && currentNode.tile.indexY == adjTile.indexY + 1 || currentNode.tile.indexX == adjTile.indexX + 1 && currentNode.tile.indexY == adjTile.indexY - 1 || currentNode.tile.indexX == adjTile.indexX - 1 && currentNode.tile.indexY == adjTile.indexY + 1 || currentNode.tile.indexX == adjTile.indexX - 1 && currentNode.tile.indexY == adjTile.indexY - 1)
                        {
                            adjTileNodeGCost = currentNode.costSoFar + 14;
                        }
                        else
                        {
                            adjTileNodeGCost = currentNode.costSoFar + 10;
                        }

                        double adjTileNodeFCost = adjTileNodeGCost + adjTileNodeHCost;
                        Node adjTileNode = new Node(adjTile, adjTileNodeFCost, currentNode, adjTileNodeGCost);

                        TODOList.Add(adjTileNode);
                    }
                    else
                    {
                        //adj tile is in todo list
                        Node adjTileNode = TODOList.Find(n => n.tile == adjTile);
                        if (adjTileNode.priority < currentNode.priority)
                        {
                            //path to adj tile is better than current node
                            adjTileNode.cameFrom = currentNode;

                            double adjTileNodeGCost;
                            double adjTileNodeHCost = HeuristicsDistance(adjTile, goal);

                            if (currentNode.tile.indexX == adjTile.indexX + 1 && currentNode.tile.indexY == adjTile.indexY + 1 || currentNode.tile.indexX == adjTile.indexX + 1 && currentNode.tile.indexY == adjTile.indexY - 1 || currentNode.tile.indexX == adjTile.indexX - 1 && currentNode.tile.indexY == adjTile.indexY + 1 || currentNode.tile.indexX == adjTile.indexX - 1 && currentNode.tile.indexY == adjTile.indexY - 1)
                            {
                                adjTileNodeGCost = adjTileNode.cameFrom.costSoFar + 14;
                            }
                            else
                            {
                                adjTileNodeGCost = adjTileNode.cameFrom.costSoFar + 10;
                            }

                            double adjTileNodeFCost = adjTileNodeGCost + adjTileNodeHCost;

                            adjTileNode.priority = adjTileNodeFCost;
                            adjTileNode.costSoFar = adjTileNodeGCost;

                            TODOList.Sort((x, y) => x.priority.CompareTo(y.priority));
                        }
                    }
                }
            }
        }

        Node targetNode = currentNode;
        Queue<Tile> path = new Queue<Tile>();
        while (targetNode != null)
        {
            finalPath.Enqueue(targetNode.tile);
            targetNode = targetNode.cameFrom;
        }

        //the a star path, doesnt seem to follow tiles
        return path;
    }

    // Manhattan Distance with horizontal/vertical cost of 10
    double HeuristicsDistance(Tile currentTile, Tile goalTile)
    {
        int xdist = Math.Abs(goalTile.indexX - currentTile.indexX);
        int ydist = Math.Abs(goalTile.indexY - currentTile.indexY);
        // Assuming cost to move horizontally and vertically is 10
        //return manhattan distance
        return (xdist * 10 + ydist * 10);
    }

    // Retrace path from a given Node back to the start Node
    Queue<Tile> RetracePath(Node node)
    {
        List<Tile> tileList = new List<Tile>();
        Node nodeIterator = node;
        while (nodeIterator.cameFrom != null)
        {
            tileList.Insert(0, nodeIterator.tile);
            nodeIterator = nodeIterator.cameFrom;
        }
        return new Queue<Tile>(tileList);
    }

    // Generate a Random Path. Used for enemies
    public Queue<Tile> RandomPath(Tile start, int stepNumber)
    {
        List<Tile> tileList = new List<Tile>();
        Tile currentTile = start;
        for (int i = 0; i < stepNumber; i++)
        {
            Tile nextTile;
            //find random adjacent tile different from last one if there's more than one choice
            if (currentTile.Adjacents.Count < 0)
            {
                break;
            }
            else if (currentTile.Adjacents.Count == 1)
            {
                nextTile = currentTile.Adjacents[0];
            }
            else
            {
                nextTile = null;
                List<Tile> adjacentList = new List<Tile>(currentTile.Adjacents);
                ShuffleTiles<Tile>(adjacentList);
                if (tileList.Count <= 0) nextTile = adjacentList[0];
                else
                {
                    foreach (Tile tile in adjacentList)
                    {
                        if (tile != tileList[tileList.Count - 1])
                        {
                            nextTile = tile;
                            break;
                        }
                    }
                }
            }
            tileList.Add(currentTile);
            currentTile = nextTile;
        }
        return new Queue<Tile>(tileList);
    }

    private void ShuffleTiles<T>(List<T> list)
    {
        // Knuth shuffle algorithm :: 
        // courtesy of Wikipedia :) -> https://forum.unity.com/threads/randomize-array-in-c.86871/
        for (int t = 0; t < list.Count; t++)
        {
            T tmp = list[t];
            int r = UnityEngine.Random.Range(t, list.Count);
            list[t] = list[r];
            list[r] = tmp;
        }
    }
}
