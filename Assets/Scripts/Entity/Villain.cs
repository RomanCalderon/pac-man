using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent ( typeof ( BoxCollider2D ) )]
public class Villain : MonoBehaviour
{
    private const float DEATH_MOVE_DELAY = 1.0f;
    private const float DEATH_SFX_VOLUME = 0.6f;

    private Grid m_grid = null;
    [SerializeField, Range ( 1.0f, 10.0f )]
    private float m_movementSpeed = 5.0f;
    private bool m_canMove = true;
    [SerializeField, Range ( 5, 40 )]
    private float m_pathUpdateInterval = 10.0f;
    private float m_updatePathRequestCooler;
    private Node m_startingNode = null;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer = null;
    [SerializeField]
    private Sprite m_normalSprite = null;
    [SerializeField]
    private Sprite m_deadSprite = null;
    [SerializeField]
    private Color m_normalColor = Color.white;
    [SerializeField]
    private Color m_evasionColor = Color.white;

    private Transform m_target = null;
    private Coroutine m_pathCoroutine = null;
    private Vector3 [] m_path = null;
    private int m_targetIndex = 0;
    private bool m_isEvadingPlayer = false;
    private bool m_isDead = false;

    private void Awake ()
    {
        Debug.Assert ( m_spriteRenderer != null );
    }

    private void OnEnable ()
    {
        GameManager.onStartPlayerPowerup += StartPlayerEvasion;
        GameManager.onEndPlayerPowerup += EndPlayerEvasion;
        GameManager.onPlayerDied += StopPath;
        GameManager.onLevelCleared += StopPath;
        GameManager.onLevelClosed += OnLevelClosed;
    }

    private void OnDisable ()
    {
        GameManager.onStartPlayerPowerup -= StartPlayerEvasion;
        GameManager.onEndPlayerPowerup -= EndPlayerEvasion;
        GameManager.onPlayerDied -= StopPath;
        GameManager.onLevelCleared -= StopPath;
        GameManager.onLevelClosed -= OnLevelClosed;
    }

    // Start is called before the first frame update
    void Start ()
    {
        m_startingNode = m_grid.GetNode ( Node.NodeType.VILLAIN_SPAWN );
        transform.position = m_startingNode.WorldPosition;
        m_spriteRenderer.sprite = m_normalSprite;
        m_spriteRenderer.color = m_normalColor;

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

    public void ResetVillain ()
    {
        transform.position = m_startingNode.WorldPosition;
        m_target = null;
        m_canMove = true;
        m_isEvadingPlayer = false;
        m_isDead = false;
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
        if ( m_pathCoroutine != null && GameManager.IsLevelCleared )
        {
            StopCoroutine ( m_pathCoroutine );
            m_pathCoroutine = null;
        }

        if ( m_target != null && m_canMove && !m_isDead && !m_isEvadingPlayer )
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
            m_path = newPath;
            if ( m_pathCoroutine != null )
            {
                StopCoroutine ( m_pathCoroutine );
            }
            m_pathCoroutine = StartCoroutine ( FollowPath () );
        }
    }

    private void StopPath ()
    {
        if ( m_pathCoroutine != null )
        {
            StopCoroutine ( m_pathCoroutine );
        }
        m_canMove = false;
    }

    private IEnumerator FollowPath ()
    {
        m_targetIndex = 0;
        Vector3 currentWaypoint = m_path [ 0 ];

        while ( m_canMove )
        {
            float waypointDistance = Vector3.Distance ( transform.position, currentWaypoint );
            if ( waypointDistance <= 0.005f )
            {
                m_targetIndex++;
                if ( m_targetIndex >= m_path.Length )
                {
                    m_targetIndex = 0;
                    m_path = new Vector3 [ 0 ];
                    FinishedPath ();
                    yield break;
                }
                currentWaypoint = m_path [ m_targetIndex ];
            }

            transform.position = Vector3.MoveTowards ( transform.position, currentWaypoint, Time.deltaTime * m_movementSpeed );
            yield return null;
        }
    }

    private void FinishedPath ()
    {
        // Respawing
        if ( m_isDead )
        {
            m_spriteRenderer.sprite = m_normalSprite;
            m_canMove = true;
            m_isDead = false;
            if ( !m_isEvadingPlayer )
            {
                SetTarget ( m_target );
            }
        }
    }

    #region Player Evasion

    private void StartPlayerEvasion ( float duration )
    {
        m_spriteRenderer.color = m_evasionColor;
        StopPath ();
        m_canMove = true;
        m_isEvadingPlayer = true;
        Vector3 destination = m_grid.GetNode ( Node.NodeType.VILLAIN_SPAWN ).WorldPosition;
        PathRequestManager.RequestPath ( transform.position, destination, OnPathFound );
    }

    private void EndPlayerEvasion ( float duration )
    {
        m_isEvadingPlayer = false;
        if ( !m_isDead )
        {
            m_spriteRenderer.color = m_normalColor;
            StopPath ();
            m_canMove = true;
            m_isEvadingPlayer = false;
            SetTarget ( m_target );
        }
    }

    #endregion

    #region Death

    public void Killed ()
    {
        if ( m_isDead )
        {
            return;
        }
        m_spriteRenderer.color = m_normalColor;
        m_spriteRenderer.sprite = m_deadSprite;
        m_isDead = true;
        StopPath ();
        m_canMove = true;
        StartCoroutine ( KillDelay () );

        // Play SFX
        AudioManager.PlaySound ( GameAssets.Instance.GetAudioClip ( "powerup" ), DEATH_SFX_VOLUME, false );
    }

    private IEnumerator KillDelay ()
    {
        yield return new WaitForSeconds ( DEATH_MOVE_DELAY );

        Vector3 destination = m_grid.GetNode ( Node.NodeType.VILLAIN_SPAWN ).WorldPosition;
        PathRequestManager.RequestPath ( transform.position, destination, OnPathFound );
    }

    #endregion

    private void OnLevelClosed ()
    {
        Destroy ( gameObject );
    }

    private void OnDrawGizmos ()
    {
        if ( m_path != null )
        {
            for ( int i = m_targetIndex; i < m_path.Length; i++ )
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube ( m_path [ i ], Vector3.one * 0.6f );

                if ( i == m_targetIndex )
                {
                    Gizmos.DrawLine ( transform.position, m_path [ i ] );
                }
                else
                {
                    Gizmos.DrawLine ( m_path [ i - 1 ], m_path [ i ] );
                }
            }
        }
    }
}
