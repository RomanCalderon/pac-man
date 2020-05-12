using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum NodeType
    {
        DOT = 0,
        WALL = 1,
        EMPTY = 2,
        PLAYER_SPAWN = 3,
        GHOST_SPAWN = 4,
        LOOP_POINT = 5,
        BIG_DOT = 6
    }

    public int GridXPos { get; private set; }
    public int GridYPos { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public NodeType Type { get; private set; }
    public Node LeftNode { get; private set; }
    public Node RightNode { get; private set; }
    public Node UpNode { get; private set; }
    public Node DownNode { get; private set; }
    public List<Node> WallConnections { get; private set; }

    public Node ( int x, int y, Vector3 worldPos, int type )
    {
        GridXPos = x;
        GridYPos = y;
        WorldPosition = worldPos;
        Type = ( NodeType ) type;
        LeftNode = null;
        RightNode = null;
        UpNode = null;
        DownNode = null;
        WallConnections = new List<Node> ();
    }

    public void AssignNeighbors ( Node [,] grid )
    {
        // Left node
        if ( GridXPos - 1 >= 0 ) // check bounds
        {
            LeftNode = grid [ GridXPos - 1, GridYPos ];
        }
        // Right node
        if ( GridXPos + 1 < grid.GetLength ( 0 ) ) // check bounds
        {
            RightNode = grid [ GridXPos + 1, GridYPos ];
        }
        // Up node
        if ( GridYPos + 1 < grid.GetLength ( 1 ) ) // check bounds
        {
            UpNode = grid [ GridXPos, GridYPos + 1 ];
        }
        // Down node
        if ( GridYPos - 1 >= 0 ) // check bounds
        {
            DownNode = grid [ GridXPos, GridYPos - 1 ];
        }
    }

    public int GetNeighborCount ()
    {
        int total = 0;

        if ( LeftNode != null )
        {
            total++;
        }
        if ( RightNode != null )
        {
            total++;
        }
        if ( UpNode != null )
        {
            total++;
        }
        if ( DownNode != null )
        {
            total++;
        }

        return total;
    }

    public int GetNeighborCount ( NodeType nodeType )
    {
        int total = 0;

        if ( LeftNode != null && LeftNode.Type == nodeType )
        {
            total++;
        }
        if ( RightNode != null && RightNode.Type == nodeType )
        {
            total++;
        }
        if ( UpNode != null && UpNode.Type == nodeType )
        {
            total++;
        }
        if ( DownNode != null && DownNode.Type == nodeType )
        {
            total++;
        }

        return total;
    }

    public override string ToString ()
    {
        return string.Format ( "Node[{0},{1}]", GridXPos, GridYPos );
    }
}
