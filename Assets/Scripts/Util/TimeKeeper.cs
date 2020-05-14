using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeKeeper : MonoBehaviour
{
    private float m_stopwatch = 0.0f;
    public int Stopwatch
    {
        get
        {
            return ( int ) m_stopwatch;
        }
    }
    private bool m_isCounting = false;

    // Update is called once per frame
    void Update ()
    {
        if ( m_isCounting )
        {
            m_stopwatch += Time.deltaTime;
        }
    }

    public void StartTime ()
    {
        m_isCounting = true;
    }

    public void StopTime ()
    {
        m_isCounting = false;
    }

    public void ResetTime ()
    {
        m_isCounting = false;
        m_stopwatch = 0.0f;
    }
}
