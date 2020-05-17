using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public enum NodeType
    {
        DOT = 0,
        WALL = 1,
        EMPTY = 2,
        PLAYER_SPAWN = 3,
        VILLAIN_SPAWN = 4,
        LOOP_POINT = 5,
        BIG_DOT = 6,
        VILLAIN_WALL = 7
    }

    public int GridXPos { get; private set; }
    public int GridYPos { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public NodeType Type { get; private set; }
    public Node LeftNode { get; private set; }
    public Node RightNode { get; private set; }
    public Node UpNode { get; private set; }
    public Node DownNode { get; private set; }
    public Node LoopNode { get; private set; }
    public List<Node> WallConnections { get; private set; }
    public bool IsWalkable
    {
        get
        {
            return Type != NodeType.WALL;
        }
    }
    public int gCost;
    public int hCost;
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    public Node Parent;
    private int heapIndex;

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

    /// <summary>
    /// Assigns references to all neighboring Nodes.
    /// </summary>
    /// <param name="grid">The grid of Nodes.</param>
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

    /// <summary>
    /// Sets this Nodes' loop node.
    /// </summary>
    /// <param name="loopNode"></param>
    public void SetLoopNode ( Node loopNode )
    {
        LoopNode = loopNode;
    }

    /// <summary>
    /// Returns the distance between this Node and Node <paramref name="node"/>.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public int GetDistance ( Node node )
    {
        int distX = Mathf.Abs ( GridXPos - node.GridXPos );
        int distY = Mathf.Abs ( GridYPos - node.GridYPos );

        if ( distX > distY )
        {
            return 14 * distY + 10 * ( distX - distY );
        }
        return 14 * distX + 10 * ( distY - distX );
    }

    /// <summary>
    /// Returns the total number of neighboring Nodes.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Returns the total number of neighboring Nodes of type <paramref name="nodeType"/>.
    /// </summary>
    /// <param name="nodeType">The type of Node to compare to.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Returns a list of all neighboring Nodes.
    /// </summary>
    /// <returns></returns>
    public List<Node> GetNeighbors ()
    {
        List<Node> neighbors = new List<Node> ();

        if ( UpNode != null )
        {
            neighbors.Add ( UpNode );
        }
        if ( RightNode != null )
        {
            neighbors.Add ( RightNode );
        }
        if ( DownNode != null )
        {
            neighbors.Add ( DownNode );
        }
        if ( LeftNode != null )
        {
            neighbors.Add ( LeftNode );
        }

        return neighbors;
    }

    public override string ToString ()
    {
        return string.Format ( "Node[{0},{1}] | Type:{2}", GridXPos, GridYPos, Type.ToString() );
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo ( Node nodeToCompare )
    {
        int compare = fCost.CompareTo ( nodeToCompare.fCost );
        if ( compare == 0 )
        {
            compare = hCost.CompareTo ( nodeToCompare.hCost );
        }
        return -compare;
    }
}
