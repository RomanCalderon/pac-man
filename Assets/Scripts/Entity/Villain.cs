using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent ( typeof ( BoxCollider2D ) )]
public class Villain : MonoBehaviour
{
    [SerializeField]
    private Grid m_grid = null;
    [SerializeField, Range ( 1.0f, 10.0f )]
    private float m_movementSpeed = 5.0f;
    private bool m_canMove = true;
    [SerializeField, Range ( 5, 40 )]
    private float m_pathUpdateInterval = 10.0f;
    private float m_updatePathRequestCooler;
    private Node startingNode = null;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer = null;
    [SerializeField]
    private Color m_color = Color.white;

    private Transform m_target = null;
    private Coroutine pathCoroutine = null;
    private Vector3 [] path = null;
    private int targetIndex = 0;

    private void Awake ()
    {
        Debug.Assert ( m_spriteRenderer != null );

        GameManager.onPlayerDied += OnPlayerDied;
        GameManager.onLevelCleared += OnLevelCleared;
    }

    // Start is called before the first frame update
    void Start ()
    {
        startingNode = m_grid.GetNode ( Node.NodeType.VILLAIN_SPAWN );
        transform.position = startingNode.WorldPosition;
        m_spriteRenderer.color = m_color;
        
        m_updatePathRequestCooler = ( m_pathUpdateInterval / m_movementSpeed );
    }

    void Update ()
    {
        MovementUpdater ();
    }

    public void SetGrid ( Grid grid )
    {
        m_grid = grid;
    }

    public void ResetVillain ( float delay )
    {
        transform.position = startingNode.WorldPosition;
        m_updatePathRequestCooler = delay;
        m_canMove = true;
    }

    public void SetTarget ( Transform target )
    {
        m_target = target;

        if ( target != null && m_canMove )
        {
            PathRequestManager.RequestPath ( transform.position, m_target.position, OnPathFound );
        }
    }

    private void MovementUpdater ()
    {
        if ( m_target != null && m_canMove )
        {
            if ( m_updatePathRequestCooler > 0 )
            {
                m_updatePathRequestCooler -= Time.deltaTime;
            }
            else
            {
                PathRequestManager.RequestPath ( transform.position, m_target.position, OnPathFound );
                m_updatePathRequestCooler = ( m_pathUpdateInterval / m_movementSpeed );
            }
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
                    yield break;
                }
                currentWaypoint = path [ targetIndex ];
            }

            transform.position = Vector3.MoveTowards ( transform.position, currentWaypoint, Time.deltaTime * m_movementSpeed );
            yield return null;
        }
    }

    private void OnLevelCleared ()
    {
        StopPath ();
    }

    private void OnPlayerDied ()
    {
        StopPath ();
    }

    private void OnDrawGizmos ()
    {
        if ( path != null )
        {
            for ( int i = targetIndex; i < path.Length; i++ )
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube ( path [ i ], Vector3.one * 0.6f );

                if ( i == targetIndex )
                {
                    Gizmos.DrawLine ( transform.position, path [ i ] );
                }
                else
                {
                    Gizmos.DrawLine ( path [ i - 1 ], path [ i ] );
                }
            }
        }
    }
}
