using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent ( typeof ( BoxCollider2D ) )]
public class Player : MonoBehaviour
{
    private const float POWER_UP_SCALE_MULTIPLIER = 1.4f;
    private const float POWER_UP_SPEED_MULTIPLIER = 1.5f;
    private const int DASH_DISTANCE = 10;
    private const float DASH_SPEED = 60;

    private Grid m_grid = null;
    private EntityMover m_playerEntity = null;
    [SerializeField]
    private float m_movementSpeed = 5.0f;
    private float m_originalSpeed = 5.0f;
    private bool m_canMove = true;
    private float m_updatePositionCooler;
    private Node m_startingNode = null;
    private bool m_isDead = false;
    private bool m_levelCleared = false;
    private bool m_isPoweredUp = false;
    [SerializeField]
    private KeyCode m_actionKey = KeyCode.Space;
    private bool m_hasEnergy = false;
    private bool m_isDashing = false;

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
        m_originalSpeed = m_movementSpeed;
    }

    private void OnEnable ()
    {
        GameManager.onStartPlayerPowerup += StartPowerup;
        GameManager.onEndPlayerPowerup += EndPowerup;
        GameManager.onPlayerDied += OnPlayerDied;
        GameManager.onLevelCleared += OnLevelCleared;
        GameManager.onLevelClosed += OnLevelClosed;
    }

    private void OnDisable ()
    {
        GameManager.onStartPlayerPowerup -= StartPowerup;
        GameManager.onEndPlayerPowerup -= EndPowerup;
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

        // Reset transform scale
        transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update ()
    {
        MovementInput ();
        MovementUpdater ();
        AnimationUpdater ();
        ActionInput ();

        if ( m_isDashing )
        {
            Vector3 targetPosition = m_playerEntity.GetCurrentPosition ().WorldPosition;
            float dashEndDistance = Vector3.Distance ( transform.position, targetPosition );
            if ( dashEndDistance <= 1 )
            {
                m_movementSpeed = m_originalSpeed;
                m_isDashing = false;
            }
        }
    }

    public void SetGrid ( Grid grid )
    {
        m_grid = grid;
    }

    // Resets the player to the starting position, along with the relevant traits
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
        m_isPoweredUp = false;

        // Reset transform scale
        transform.localScale = Vector3.one;
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
        if ( Vector3.Distance ( transform.position, targetPosition ) > 2 && !m_isDashing )
        {
            // Teleport to loop node position
            transform.position = targetPosition;

            // Play teleport sfx
            AudioManager.PlaySound ( "teleport", 0.5f, false );
        }
        else
        {
            // Normal movement to target node
            transform.position = Vector3.MoveTowards ( transform.position, targetPosition, Time.deltaTime * m_movementSpeed );
        }

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
            PickupItem ( item );
        }

        if ( collision.tag == "Villain" )
        {
            if ( m_isPoweredUp )
            {
                Villain villain = collision.transform.GetComponent<Villain> ();
                villain.Killed ();
            }
            else
            {
                GameManager.instance.PlayerKilled ();
            }
        }
    }

    private void PickupItem ( Pickup item )
    {
        if ( item == null )
        {
            return;
        }
        switch ( item.GetPickupType () )
        {
            case Pickup.PickupTypes.COIN:
                GameManager.instance.ConsumedCoin ();
                break;
            case Pickup.PickupTypes.POWERUP:
                GameManager.instance.ConsumedDrop ( item.GetPickupType () );
                break;
            case Pickup.PickupTypes.ENERGY_DROP:
                m_hasEnergy = true;
                GameManager.instance.ConsumedDrop ( item.GetPickupType () );
                break;
            case Pickup.PickupTypes.EVO_DROP:
            case Pickup.PickupTypes.LIFE_DROP:
                GameManager.instance.ConsumedDrop ( item.GetPickupType () );
                break;
            default:
                break;
        }
        Destroy ( item.gameObject );
    }

    #region Updaters

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

    #endregion

    #region Powerup

    private void StartPowerup ( float duration )
    {
        m_isPoweredUp = true;

        // Scale up transform
        transform.localScale = Vector3.one * POWER_UP_SCALE_MULTIPLIER;
        // Increase movement speed
        m_movementSpeed = m_originalSpeed * POWER_UP_SPEED_MULTIPLIER;
    }

    private void EndPowerup ( float duration )
    {
        m_isPoweredUp = false;

        // Reset transform scale
        transform.localScale = Vector3.one;
        // Reset movement speed
        m_movementSpeed = m_originalSpeed;
    }

    #endregion

    #region Actions

    private void ActionInput ()
    {
        if ( Input.GetKeyDown ( m_actionKey ) )
        {
            Dash ();
        }
    }

    private void Dash ()
    {
        if ( !m_hasEnergy )
        {
            return;
        }
        m_hasEnergy = false;

        m_movementSpeed = DASH_SPEED;
        for ( int i = 0; i < DASH_DISTANCE; i++ )
        {
            m_playerEntity.Move ();
        }
        m_isDashing = true;
    }

    #endregion

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
