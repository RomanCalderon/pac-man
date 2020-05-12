using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent ( typeof ( Grid ) )]
public class Pathfinding : MonoBehaviour
{
    private Grid m_grid = null;

    [SerializeField]
    private Transform seeker, target;

    private void Awake ()
    {
        m_grid = GetComponent<Grid> ();
    }

    private void Update ()
    {
        if ( Input.GetKeyDown ( KeyCode.Space ) )
        {
            FindPath ( seeker.position, target.position );
        }
    }

    private void FindPath ( Vector3 startPos, Vector3 targetPos )
    {
        Node startNode = m_grid.NodeFromWorldPoint ( startPos );
        Node targetNode = m_grid.NodeFromWorldPoint ( targetPos );

        Heap<Node> openSet = new Heap<Node> ( m_grid.MaxSize );
        HashSet<Node> closedSet = new HashSet<Node> ();
        openSet.Add ( startNode );

        while ( openSet.Count > 0 )
        {
            Node currentNode = openSet.RemoveFirst ();
            closedSet.Add ( currentNode );

            if ( currentNode == targetNode )
            {
                RetracePath ( startNode, targetNode );
                return;
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

    private void RetracePath ( Node startNode, Node endNode )
    {
        List<Node> path = new List<Node> ();
        Node currentNode = endNode;

        while ( currentNode != startNode )
        {
            path.Add ( currentNode );
            currentNode = currentNode.Parent;
        }

        path.Reverse ();

        m_grid.Path = path;
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
