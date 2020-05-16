using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent ( typeof ( Grid ) )]
[RequireComponent ( typeof ( PathRequestManager ) )]
public class Pathfinding : MonoBehaviour
{
    private Grid m_grid = null;
    private PathRequestManager m_requestManager;

    private void Awake ()
    {
        m_grid = GetComponent<Grid> ();
        m_requestManager = GetComponent<PathRequestManager> ();
    }

    public void StartFindPath ( Vector3 startPos, Vector3 targetPos )
    {
        StartCoroutine ( FindPath ( startPos, targetPos ) );
    }

    private IEnumerator FindPath ( Vector3 startPos, Vector3 targetPos )
    {
        Vector3 [] waypoints = new Vector3 [ 0 ];
        bool pathSuccess = false;

        Node startNode = m_grid.NodeFromWorldPoint ( startPos );
        Node targetNode = m_grid.NodeFromWorldPoint ( targetPos );

        if ( startNode.IsWalkable && targetNode.IsWalkable )
        {
            Heap<Node> openSet = new Heap<Node> ( m_grid.MaxSize );
            HashSet<Node> closedSet = new HashSet<Node> ();
            openSet.Add ( startNode );

            while ( openSet.Count > 0 )
            {
                Node currentNode = openSet.RemoveFirst ();
                closedSet.Add ( currentNode );

                if ( currentNode == targetNode )
                {
                    pathSuccess = true;
                    break;
                }

                foreach ( Node neighbor in currentNode.GetNeighbors () )
                {
                    if ( !neighbor.IsWalkable || closedSet.Contains ( neighbor ) )
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = currentNode.gCost + GetDistance ( currentNode, neighbor );
                    if ( newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains ( neighbor ) )
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetDistance ( neighbor, targetNode );
                        neighbor.Parent = currentNode;

                        if ( !openSet.Contains ( neighbor ) )
                        {
                            openSet.Add ( neighbor );
                        }
                    }
                }
            }
        }

        yield return null;
        if ( pathSuccess )
        {
            waypoints = RetracePath ( startNode, targetNode );
            pathSuccess = waypoints.Length > 0;
        }
        m_requestManager.FinishedProcessingPath ( waypoints, pathSuccess );
    }

    private Vector3 [] RetracePath ( Node startNode, Node endNode )
    {
        List<Node> path = new List<Node> ();
        Node currentNode = endNode;

        while ( currentNode != startNode )
        {
            path.Add ( currentNode );
            currentNode = currentNode.Parent;
        }
        Vector3 [] waypoints = ConvertPathToWaypoints ( path );
        Array.Reverse ( waypoints );
        return waypoints;
    }

    Vector3 [] ConvertPathToWaypoints ( List<Node> path )
    {
        List<Vector3> waypoints = new List<Vector3> ();
        
        for ( int i = 0; i < path.Count; i++ )
        {
            waypoints.Add ( path [ i ].WorldPosition );
        }
        return waypoints.ToArray ();
    }

    private int GetDistance ( Node nodeA, Node nodeB )
    {
        int distX = Mathf.Abs ( nodeA.GridXPos - nodeB.GridXPos );
        int distY = Mathf.Abs ( nodeA.GridYPos - nodeB.GridYPos );

        if ( distX > distY )
        {
            return 14 * distY + 10 * ( distX - distY );
        }
        return 14 * distX + 10 * ( distY - distX );
    }
}
