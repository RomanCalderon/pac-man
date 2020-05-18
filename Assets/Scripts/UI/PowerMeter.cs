using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerMeter : MonoBehaviour
{
    [SerializeField]
    private Image m_meterFillImage = null;
    private float m_fillCooldown = 0.0f;
    private float m_fillDuration = 1.0f;

    private void Awake ()
    {
        Debug.Assert ( m_meterFillImage != null );
    }

    private void OnEnable ()
    {
        GameManager.onStartPlayerPowerup += StartPowerMeter;
    }

    private void OnDisable ()
    {
        GameManager.onStartPlayerPowerup -= StartPowerMeter;
    }

    // Update is called once per frame
    void Update ()
    {
        if ( m_fillCooldown > 0 )
        {
            m_fillCooldown -= Time.deltaTime;
        }
        else
        {
            m_fillDuration = 0.0f;
        }
        m_meterFillImage.fillAmount = m_fillCooldown / m_fillDuration;
    }

    private void StartPowerMeter ( float duration )
    {
        m_fillCooldown = m_fillDuration = duration;
        m_fillDuration = Mathf.Max ( 0.01f, m_fillDuration );
    }
}
