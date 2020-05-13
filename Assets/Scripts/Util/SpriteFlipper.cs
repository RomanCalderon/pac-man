using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent ( typeof ( SpriteRenderer ) )]
public class SpriteFlipper : MonoBehaviour
{
    enum FlipAxis
    {
        X_AXIS,
        Y_AXIS,
        BOTH
    }

    private SpriteRenderer m_spriteRenderer = null;
    [SerializeField, Range ( 0.1f, 5.0f )]
    private float m_flipRate = 1.0f;
    private float m_flipCooler = 0.0f;
    [SerializeField]
    private FlipAxis m_flipAxis = FlipAxis.X_AXIS;

    private void Awake ()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer> ();
    }

    // Update is called once per frame
    void Update ()
    {
        if ( m_flipCooler > 0 )
        {
            m_flipCooler -= Time.deltaTime;
        }
        else
        {
            FlipSprite ();
            m_flipCooler = 1.0f / m_flipRate;
        }
    }

    private void FlipSprite ()
    {
        switch ( m_flipAxis )
        {
            case FlipAxis.X_AXIS:
                m_spriteRenderer.flipX = !m_spriteRenderer.flipX;
                break;
            case FlipAxis.Y_AXIS:
                m_spriteRenderer.flipY = !m_spriteRenderer.flipY;
                break;
            case FlipAxis.BOTH:
                m_spriteRenderer.flipX = !m_spriteRenderer.flipX;
                m_spriteRenderer.flipY = !m_spriteRenderer.flipY;
                break;
            default:
                break;
        }
    }
}
