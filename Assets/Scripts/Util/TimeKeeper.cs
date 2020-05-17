using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour
{
    public delegate void TimerHandler ( float ellapsedTime );
    public static TimerHandler onTimerChanged;

    private float m_ellapsedTime = 0.0f;
    public float EllapsedTime
    {
        get
        {
            return m_ellapsedTime;
        }
        private set
        {
            float oldValue = m_ellapsedTime;
            m_ellapsedTime = value;
            float flooredValue = Mathf.FloorToInt ( value );
            float ceiledValue = Mathf.CeilToInt ( value );

            if ( flooredValue > oldValue || ceiledValue < oldValue )
            {
                onTimerChanged?.Invoke ( flooredValue );
            }
        }
    }
    private bool m_isCounting = false;

    // Update is called once per frame
    void Update ()
    {
        if ( m_isCounting )
        {
            EllapsedTime += Time.deltaTime;
        }
    }

    public void StartTime ( float delay = 0.0f )
    {
        if ( m_isCounting )
        {
            return;
        }

        if ( delay > 0.0f )
        {
            StartCoroutine ( StartDelay ( delay ) );
        }
        else
        {
            m_isCounting = true;
        }
    }

    public void StopTime ()
    {
        m_isCounting = false;
    }

    public void ResetTime ()
    {
        m_isCounting = false;
        m_ellapsedTime = 0.0f;
    }

    private IEnumerator StartDelay ( float delay )
    {
        yield return new WaitForSeconds ( delay );
        m_isCounting = true;
    }
}
