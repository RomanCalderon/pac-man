using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villain : MonoBehaviour
{
    [SerializeField]
    private Grid m_grid = null;
    private EntityMover m_villainEntity = null;
    private float m_movementSpeed = 5.0f;
    private float m_updatePositionCooler;
    [SerializeField]
    private EntityMover.Directions m_startDirection = EntityMover.Directions.RIGHT;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer;

    [SerializeField]
    private Transform m_target = null;
    private Coroutine pathCoroutine = null;
    private Vector3 [] path = null;
    private int targetIndex = 0;

    private void Awake ()
    {
        Debug.Assert ( m_grid != null );
        Debug.Assert ( m_spriteRenderer != null );

        Node startingNode = m_grid.GetNode ( Node.NodeType.VILLAIN_SPAWN );
        transform.position = startingNode.WorldPosition;
        m_villainEntity = new EntityMover ( startingNode, EntityMover.Directions.RIGHT );
    }

    // Start is called before the first frame update
    void Start ()
    {
        PathRequestManager.RequestPath ( transform.position, m_target.position, OnPathFound );
    }

    void Update ()
    {
        MovementUpdater ();
    }

    private void MovementUpdater ()
    {
        if ( m_updatePositionCooler > 0 )
        {
            m_updatePositionCooler -= Time.deltaTime;
        }
        else
        {
            PathRequestManager.RequestPath ( transform.position, m_target.position, OnPathFound );
            m_updatePositionCooler = ( 11f / m_movementSpeed );
        }
    }

    public void OnPathFound ( Vector3 [] newPath, bool pathSuccessful )
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
        if ( m_villainEntity != null )
        {
            Gizmos.color = Color.red;
            Vector3 playerNodePosition = m_villainEntity.GetCurrentPosition ().WorldPosition;
            Gizmos.DrawCube ( playerNodePosition, Vector3.one * 0.6f );
        }

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
