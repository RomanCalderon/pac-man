using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Drop : Pickup
{
    private Grid m_grid = null;
    [SerializeField, Range ( 1.0f, 10.0f )]
    private float m_movementSpeed = 5.0f;
    private bool m_canMove = true;
    private float m_updatePathRequestCooler;
    private Node m_targetNode = null;
    private Coroutine pathCoroutine = null;
    private Vector3 [] path = null;
    private int targetIndex = 0;
    
    private void OnEnable ()
    {
        GameManager.onPlayerDied += StopPath;
        GameManager.onLevelCleared += StopPath;
        GameManager.onLevelClosed += OnLevelClosed;
    }

    private void OnDisable ()
    {
        GameManager.onPlayerDied -= StopPath;
        GameManager.onLevelCleared -= StopPath;
        GameManager.onLevelClosed -= OnLevelClosed;
    }

    public void SetGrid ( Grid grid )
    {
        m_grid = grid;
    }

    public void StartPath ()
    {
        m_targetNode = m_grid.GetRandomNode ( Node.NodeType.DOT );

        if ( m_targetNode != null && m_canMove )
        {
            PathRequestManager.RequestPath ( transform.position, m_targetNode.WorldPosition, OnPathFound );
        }
    }

    private void OnPathFound ( Vector3 [] newPath, bool pathSuccessful )
    {
        if ( pathSuccessful )
        {
            path = newPath;
            if ( pathCoroutine != null )
            {
                StopCoroutine ( pathCoroutine );
            }
            pathCoroutine = StartCoroutine ( FollowPath () );
        }
    }

    private void StopPath ()
    {
        if ( pathCoroutine != null )
        {
            StopCoroutine ( pathCoroutine );
        }
        m_canMove = false;
    }

    private IEnumerator FollowPath ()
    {
        targetIndex = 0;
        Vector3 currentWaypoint = path [ 0 ];

        while ( m_canMove )
        {
            float waypointDistance = Vector3.Distance ( transform.position, currentWaypoint );
            if ( waypointDistance <= 0.005f )
            {
                targetIndex++;
                if ( targetIndex >= path.Length )
                {
                    targetIndex = 0;
                    path = new Vector3 [ 0 ];
                    // Get new path
                    StartPath ();
                    yield break;
                }
                currentWaypoint = path [ targetIndex ];
            }

            transform.position = Vector3.MoveTowards ( transform.position, currentWaypoint, Time.deltaTime * m_movementSpeed );
            yield return null;
        }
    }

    private void OnLevelClosed ()
    {
        Destroy ( gameObject );
    }
}
