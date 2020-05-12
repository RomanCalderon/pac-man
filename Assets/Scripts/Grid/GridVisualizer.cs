using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent ( typeof ( Grid ) )]
public class GridVisualizer : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRendererPrefab = null;

    private Grid grid = null;
    private List<LineRenderer> lineRenderers = new List<LineRenderer> ();
    [SerializeField]
    private List<Node> openWallNodes = new List<Node> ();

    private void Awake ()
    {
        grid = GetComponent<Grid> ();
        lineRenderers = new List<LineRenderer> ();
    }

    // Start is called before the first frame update
    void Start ()
    {
        foreach ( Node n in grid.NodeArray )
        {
            if ( n.Type == Node.NodeType.WALL )
            {
                openWallNodes.Add ( n );
            }
        }

        while ( openWallNodes.Count > 0 )
        {
            DrawWalls ();
        }
    }

    private void DrawWalls ()
    {
        List<Node> closedSet = new List<Node> ();

        BoundaryFill ( openWallNodes [ 0 ], ref closedSet );

        foreach ( Node n in closedSet )
        {
            // Up Node
            if ( n.UpNode != null && n.UpNode.Type == Node.NodeType.WALL )
            {
                // Check if connection wasn't made
                if ( !n.WallConnections.Contains ( n.UpNode ) )
                {
                    // Add connection references
                    n.WallConnections.Add ( n.UpNode );
                    n.UpNode.WallConnections.Add ( n );

                    // Create the line connection
                    CreateLine ( n, n.UpNode );
                }
            }

            // Right Node
            if ( n.RightNode != null && n.RightNode.Type == Node.NodeType.WALL )
            {
                // Check if connection wasn't made
                if ( !n.WallConnections.Contains ( n.RightNode ) )
                {
                    // Add connection references
                    n.WallConnections.Add ( n.RightNode );
                    n.RightNode.WallConnections.Add ( n );

                    // Create the line connection
                    CreateLine ( n, n.RightNode );
                }
            }

            // Down Node
            if ( n.DownNode != null && n.DownNode.Type == Node.NodeType.WALL )
            {
                // Check if connection wasn't made
                if ( !n.WallConnections.Contains ( n.DownNode ) )
                {
                    // Add connection references
                    n.WallConnections.Add ( n.DownNode );
                    n.DownNode.WallConnections.Add ( n );

                    // Create the line connection
                    CreateLine ( n, n.DownNode );
                }
            }

            // Left Node
            if ( n.LeftNode != null && n.LeftNode.Type == Node.NodeType.WALL )
            {
                // Check if connection wasn't made
                if ( !n.WallConnections.Contains ( n.LeftNode ) )
                {
                    // Add connection references
                    n.WallConnections.Add ( n.LeftNode );
                    n.LeftNode.WallConnections.Add ( n );

                    // Create the line connection
                    CreateLine ( n, n.LeftNode );
                }
            }

            // Remove this node from the openWallNodes collection
            openWallNodes.Remove ( n );
        }

    }

    private void CreateLine ( Node nodeA, Node nodeB )
    {
        LineRenderer lineRenderer = Instantiate ( lineRendererPrefab, transform );
        lineRenderers.Add ( lineRenderer );
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition ( 0, nodeA.WorldPosition );
        lineRenderer.SetPosition ( 1, nodeB.WorldPosition );
    }

    private void BoundaryFill ( Node n, ref List<Node> closedSet )
    {
        if ( n == null )
        {
            return;
        }
        if ( !closedSet.Contains ( n ) )
        {
            if ( n.Type == Node.NodeType.WALL )
            {
                closedSet.Add ( n );
            }
            else
            {
                return;
            }

            BoundaryFill ( n.UpNode, ref closedSet );
            BoundaryFill ( n.RightNode, ref closedSet );
            BoundaryFill ( n.DownNode, ref closedSet );
            BoundaryFill ( n.LeftNode, ref closedSet );
        }
    }

    private void OnDrawGizmos ()
    {
        if ( grid == null || grid.NodeArray == null )
        {
            return;
        }

        foreach ( Node n in grid.NodeArray )
        {
            switch ( n.Type )
            {
                case Node.NodeType.DOT:
                    Gizmos.color = Color.white;     //0 - DOT
                    break;
                case Node.NodeType.WALL:
                    Gizmos.color = Color.blue;      //1 - WALL
                    break;
                case Node.NodeType.EMPTY:
                    Gizmos.color = Color.black;     //2 - EMPTY
                    break;
                case Node.NodeType.PLAYER_SPAWN:
                    Gizmos.color = Color.yellow;    //3 - PLAYER_SPAWN
                    break;
                case Node.NodeType.VILLAIN_SPAWN:
                    Gizmos.color = Color.red;       //4 - VILLAIN_SPAWN
                    break;
                case Node.NodeType.LOOP_POINT:
                    Gizmos.color = Color.cyan;      //5 - LOOP_POINT
                    break;
                case Node.NodeType.BIG_DOT:
                    Gizmos.color = Color.green;     //6 - BIG_DOT
                    break;
                case Node.NodeType.VILLAIN_WALL:    //7 - VILLAIN_WALL
                    Gizmos.color = Color.grey;
                    break;
                default:
                    break;
            }
            Gizmos.DrawCube ( n.WorldPosition, Vector3.one * 0.3f );
        }
    }
}
