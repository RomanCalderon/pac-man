using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Grid m_grid = null;
    private EntityMover m_playerEntity = null;
    private float m_movementSpeed = 5.0f;
    private float m_updatePositionCooler;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer;
    [SerializeField]
    private Animator m_animator;

    private void Awake ()
    {
        Debug.Assert ( m_grid != null );
        Debug.Assert ( m_spriteRenderer != null );
        Debug.Assert ( m_animator != null );
    }

    // Start is called before the first frame update
    void Start ()
    {
        Node startingNode = m_grid.GetNode ( Node.NodeType.PLAYER_SPAWN );
        transform.position = startingNode.WorldPosition;
        m_playerEntity = new EntityMover ( startingNode, EntityMover.Directions.RIGHT );
        m_playerEntity.Move ();
    }

    // Update is called once per frame
    void Update ()
    {
        Vector3 targetPosition = m_playerEntity.GetCurrentPosition ().WorldPosition;
        transform.position = Vector3.MoveTowards ( transform.position, targetPosition, Time.deltaTime * m_movementSpeed );

        MovementInput ();
        MovementUpdater ();
        AnimationUpdater ();
    }

    private void MovementInput ()
    {
        if ( Input.GetKeyDown ( KeyCode.UpArrow ) || Input.GetKeyDown ( KeyCode.W ) )
        {
            m_playerEntity.UpdateDirection ( EntityMover.Directions.UP );
        }
        if ( Input.GetKeyDown ( KeyCode.RightArrow ) || Input.GetKeyDown ( KeyCode.D ) )
        {
            m_playerEntity.UpdateDirection ( EntityMover.Directions.RIGHT );
            UpdateGraphicsDirection ( EntityMover.Directions.RIGHT );
        }
        if ( Input.GetKeyDown ( KeyCode.DownArrow ) || Input.GetKeyDown ( KeyCode.S ) )
        {
            m_playerEntity.UpdateDirection ( EntityMover.Directions.DOWN );
        }
        if ( Input.GetKeyDown ( KeyCode.LeftArrow ) || Input.GetKeyDown ( KeyCode.A ) )
        {
            m_playerEntity.UpdateDirection ( EntityMover.Directions.LEFT );
            UpdateGraphicsDirection ( EntityMover.Directions.LEFT );
        }
    }

    private void MovementUpdater ()
    {
        if ( m_updatePositionCooler > 0 )
        {
            m_updatePositionCooler -= Time.deltaTime;
        }
        else
        {
            m_playerEntity.Move ();
            m_updatePositionCooler = ( 1f / m_movementSpeed );
        }
    }

    private void AnimationUpdater ()
    {
        m_animator.SetBool ( "IsMoving", m_playerEntity.m_isMoving );
    }

    private void UpdateGraphicsDirection ( EntityMover.Directions direction )
    {
        m_spriteRenderer.flipX = direction == EntityMover.Directions.LEFT;
    }

    private void OnDrawGizmos ()
    {
        if ( m_playerEntity != null )
        {
            Gizmos.color = Color.white;
            Vector3 playerNodePosition = m_playerEntity.GetCurrentPosition ().WorldPosition;
            Gizmos.DrawCube ( playerNodePosition, Vector3.one * 0.6f );
        }
    }
}
