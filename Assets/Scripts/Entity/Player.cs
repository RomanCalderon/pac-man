using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent ( typeof ( BoxCollider2D ) )]
public class Player : MonoBehaviour
{
    [SerializeField]
    private Grid m_grid = null;
    private EntityMover m_playerEntity = null;
    [SerializeField]
    private float m_movementSpeed = 5.0f;
    private bool m_canMove = true;
    private float m_updatePositionCooler;
    private Node startingNode = null;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer = null;
    [SerializeField]
    private Animator m_animator = null;
    private BoxCollider2D m_boxCollider = null;

    private void Awake ()
    {
        Debug.Assert ( m_spriteRenderer != null );
        Debug.Assert ( m_animator != null );

        m_boxCollider = GetComponent<BoxCollider2D> ();

        GameManager.onPlayerDied += OnPlayerDied;
        GameManager.onLevelCleared += OnLevelCleared;
    }

    // Start is called before the first frame update
    void Start ()
    {
        startingNode = m_grid.GetNode ( Node.NodeType.PLAYER_SPAWN );
        Node.NodeType [] invalidNodeTypes = new Node.NodeType [] { Node.NodeType.WALL, Node.NodeType.VILLAIN_WALL };
        m_playerEntity = new EntityMover ( startingNode, EntityMover.Directions.RIGHT, invalidNodeTypes );
        transform.position = startingNode.WorldPosition;

        m_updatePositionCooler = 3.0f;
    }

    // Update is called once per frame
    void Update ()
    {
        MovementInput ();
        MovementUpdater ();
        AnimationUpdater ();
    }

    public void SetGrid ( Grid grid )
    {
        m_grid = grid;
    }

    public void ResetPlayer()
    {
        transform.position = startingNode.WorldPosition;
        m_playerEntity.SetCurrentPosition ( m_grid, transform.position );
        m_updatePositionCooler = 3.0f;
        m_canMove = true;
    }

    public Node GetPlayerNode ()
    {
        return m_playerEntity.GetCurrentPosition ();
    }

    #region Movement

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
        if ( !m_canMove )
        {
            return;
        }

        Vector3 targetPosition = m_playerEntity.GetCurrentPosition ().WorldPosition;
        transform.position = Vector3.MoveTowards ( transform.position, targetPosition, Time.deltaTime * m_movementSpeed );

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

    #endregion

    private void OnTriggerEnter2D ( Collider2D collision )
    {
        if ( collision.tag == "Pickup" )
        {
            Pickup item = collision.GetComponent<Pickup> ();
            if ( item == null )
            {
                return;
            }
            GameManager.instance.ConsumedCoin ();
            AudioManager.PlaySound ( "pickup_coin", 0.5f );
            Destroy ( item.gameObject );
        }

        if ( collision.tag == "Villain" )
        {
            GameManager.instance.PlayerKilled ();
        }
    }

    private void AnimationUpdater ()
    {
        m_animator.SetBool ( "IsMoving", m_playerEntity.m_isMoving );
    }

    private void UpdateGraphicsDirection ( EntityMover.Directions direction )
    {
        if ( !m_canMove )
        {
            return;
        }
        m_spriteRenderer.flipX = direction == EntityMover.Directions.LEFT;
    }

    private void OnLevelCleared()
    {
        m_canMove = false;
    }

    private void OnPlayerDied ()
    {
        m_canMove = false;
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
