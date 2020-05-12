using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public TextAsset gridConfigFile = null;
    public TextAsset nodeConfigFile = null;

    private GridConfigData gridData = null;
    private int [,] nodeData = null;

    public Node [,] NodeArray { get; private set; } = null;
    public int GridSizeX { get; private set; }
    public int GridSizeY { get; private set; }

    public List<Node> Nodes { get; private set; }

    private void Awake ()
    {
        gridData = GridDataReader.ReadData ( gridConfigFile );
        nodeData = NodeDataReader.ReadData ( nodeConfigFile );
        Nodes = new List<Node> ();

        CreateGrid ( gridData, nodeData );
    }

    public void CreateGrid ( GridConfigData gridData, int [,] nodeData )
    {
        Debug.Log ( "Building grid..." );

        if ( gridData == null )
        {
            throw new Exception ( "Grid data is null." );
        }

        NodeArray = new Node [ gridData.Size.X, gridData.Size.Y ];
        GridSizeX = gridData.Size.X;
        GridSizeY = gridData.Size.Y;

        Debug.Log ( "gridData.Size.X = " + gridData.Size.X );
        Debug.Log ( "gridData.Size.Y = " + gridData.Size.Y );

        for ( int x = 0; x < gridData.Size.X; x++ )
        {
            for ( int y = 0; y < gridData.Size.Y; y++ )
            {
                if ( nodeData == null )
                {
                    throw new Exception ( "nodeData is null." );
                }

                Vector3 worldPos = new Vector3 ( x * gridData.Spacing.X + gridData.Offset.X, 0, y * gridData.Spacing.Y + gridData.Offset.Y );
                NodeArray [ x, y ] = new Node ( x, y, worldPos, nodeData [ x, y ] );
                Nodes.Add ( NodeArray [ x, y ] );
            }
        }

        foreach ( Node n in NodeArray )
        {
            n.AssignNeighbors ( NodeArray );
        }

        // Done building grid
        Debug.Log ( "Done." );
    }

    public Node GetNode ( int posX, int posY )
    {
        if ( posX >= 0 && posX < GridSizeX && posY >= 0 && posY < GridSizeY )
        {
            return NodeArray [ posX, posY ];
        }
        return null;
    }

    public Node GetNode ( Node.NodeType nodeType )
    {
        return Nodes.First ( n => n.Type == nodeType );
    }

    public Node GetClosestNode ( Vector3 worldPosition )
    {
        int xIndex = Mathf.RoundToInt ( ( worldPosition.x - gridData.Offset.X ) / gridData.Spacing.X );
        int yIndex = Mathf.RoundToInt ( ( worldPosition.z - gridData.Offset.Y ) / gridData.Spacing.Y );
        return NodeArray [ xIndex, yIndex ];
    }
    
}
