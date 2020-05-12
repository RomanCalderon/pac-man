using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villain : MonoBehaviour
{
    [SerializeField]
    private Grid m_grid = null;
    [SerializeField, Range ( 1.0f, 10.0f )]
    private float m_movementSpeed = 5.0f;
    [SerializeField]
    private float m_pathUpdateInterval = 10.0f;
    private float m_updatePathRequestCooler;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer = null;
    [SerializeField]
    private Color m_color = Color.white;
    
    private Node m_targetNode = null;
    private Coroutine pathCoroutine = null;
    private Vector3 [] path = null;
    private int targetIndex = 0;

    private void Awake ()
    {
        Debug.Assert ( m_grid != null );
        Debug.Assert ( m_spriteRenderer != null );

        Node startingNode = m_grid.GetNode ( Node.NodeType.VILLAIN_SPAWN );
        transform.position = startingNode.WorldPosition;
        m_spriteRenderer.color = m_color;
    }

    // Start is called before the first frame update
    void Start ()
    {
        m_updatePathRequestCooler = ( m_pathUpdateInterval / m_movementSpeed );
    }

    void Update ()
    {
        MovementUpdater ();
    }

    public void SetTarget ( Node targetNode )
    {
        m_targetNode = targetNode;

        if ( targetNode != null )
        {
            PathRequestManager.RequestPath ( transform.position, m_targetNode.WorldPosition, OnPathFound );
        }
    }

    private void MovementUpdater ()
    {
        if ( m_targetNode != null )
        {
            if ( m_updatePathRequestCooler > 0 )
            {
                m_updatePathRequestCooler -= Time.deltaTime;
            }
            else
            {
                PathRequestManager.RequestPath ( transform.position, m_targetNode.WorldPosition, OnPathFound );
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

    private IEnumerator FollowPath ()
    {
        targetIndex = 0;
        Vector3 currentWaypoint = path [ 0 ];

        while ( true )
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
