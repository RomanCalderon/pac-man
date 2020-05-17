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
    private Node m_startingNode = null;
    private bool m_isDead = false;
    private bool m_levelCleared = false;

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

    }

    private void OnEnable ()
    {
        GameManager.onPlayerDied += OnPlayerDied;
        GameManager.onLevelCleared += OnLevelCleared;
        GameManager.onLevelClosed += OnLevelClosed;
    }

    private void OnDisable ()
    {
        GameManager.onPlayerDied -= OnPlayerDied;
        GameManager.onLevelCleared -= OnLevelCleared;
        GameManager.onLevelClosed -= OnLevelClosed;
    }

    // Start is called before the first frame update
    void Start ()
    {
        m_startingNode = m_grid.GetNode ( Node.NodeType.PLAYER_SPAWN );
        Node.NodeType [] invalidNodeTypes = new Node.NodeType [] { Node.NodeType.WALL, Node.NodeType.VILLAIN_WALL };
        m_playerEntity = new EntityMover ( m_startingNode, EntityMover.Directions.RIGHT, invalidNodeTypes );
        transform.position = m_startingNode.WorldPosition;

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

    public void ResetPlayer ()
    {
        transform.position = m_startingNode.WorldPosition;
        m_playerEntity.SetCurrentPosition ( m_grid, transform.position );
        m_playerEntity.UpdateDirection ( EntityMover.Directions.RIGHT );
        m_playerEntity.Stop ();
        m_updatePositionCooler = 3.0f;
        m_canMove = true;
        m_isDead = false;
        m_levelCleared = false;
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
        if ( !m_canMove || m_isDead )
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
            AudioManager.PlaySound ( "pickup_coin", 0.3f, false );
            Destroy ( item.gameObject );
        }

        if ( collision.tag == "Villain" )
        {
            GameManager.instance.PlayerKilled ();
        }
    }

    private void AnimationUpdater ()
    {
        m_animator.SetBool ( "IsDead", m_isDead );
        m_animator.SetBool ( "ClearedLevel", m_levelCleared );

        if ( !m_isDead )
        {
            m_animator.SetBool ( "IsMoving", m_playerEntity.IsMoving && m_canMove );
        }
    }

    private void UpdateGraphicsDirection ( EntityMover.Directions direction )
    {
        if ( !m_canMove )
        {
            return;
        }
        m_spriteRenderer.flipX = direction == EntityMover.Directions.LEFT;
    }

    private void OnLevelCleared ()
    {
        m_canMove = false;
        m_levelCleared = true;
    }

    private void OnLevelClosed ()
    {
        Destroy ( gameObject );
    }

    private void OnPlayerDied ()
    {
        m_canMove = false;
        m_isDead = true;
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
