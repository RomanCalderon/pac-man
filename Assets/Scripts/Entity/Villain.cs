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
    
    private void Awake ()
    {
        Debug.Assert ( m_grid != null );
        Debug.Assert ( m_spriteRenderer != null );
    }

    // Start is called before the first frame update
    void Start()
    {
        Node startingNode = m_grid.GetNode ( Node.NodeType.VILLAIN_SPAWN );
        transform.position = startingNode.WorldPosition;
        m_villainEntity = new EntityMover ( startingNode, EntityMover.Directions.RIGHT );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnDrawGizmos ()
    {
        if ( m_villainEntity != null )
        {
            Gizmos.color = Color.red;
            Vector3 playerNodePosition = m_villainEntity.GetCurrentPosition ().WorldPosition;
            Gizmos.DrawCube ( playerNodePosition, Vector3.one * 0.6f );
        }
    }
}
