using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCountdown : MonoBehaviour
{
    [SerializeField]
    private int m_startingValue = 3;
    [SerializeField]
    private string m_endMessage = "GO!";
    [SerializeField]
    private float m_endMessageDuration = 0.5f;
    [SerializeField]
    private GameObject m_countdownGameObject = null;
    [SerializeField]
    private Text m_countdownText = null;
    private bool m_isCounting = false;
    private float m_countdown = 0.0f;
    private bool m_showEndMessage = false;
    private float m_endMessageCooldown = 0.0f;

    private void Awake ()
    {
        Debug.Assert ( m_countdownText != null );

        m_countdownGameObject.SetActive ( false );
    }

    private void OnEnable ()
    {
        GameManager.onStartCountdown += StartCountdown;
    }

    private void OnDisable ()
    {
        GameManager.onStartCountdown -= StartCountdown;
    }

    // Update is called once per frame
    void Update ()
    {
        if ( m_isCounting )
        {
            if ( m_countdown > 0 )
            {
                m_countdown -= Time.deltaTime;
                m_countdownText.text = Mathf.CeilToInt ( m_countdown ).ToString ( "n0" );
            }
            else
            {
                m_countdown = 0;
                FinishedCountdown ();
            }
        }

        if ( m_showEndMessage )
        {
            if ( m_endMessageCooldown > 0 )
            {
                m_endMessageCooldown -= Time.deltaTime;
            }
            else
            {
                m_endMessageCooldown = 0.0f;
                CancelCountdown ();
            }
        }
    }

    public void StartCountdown ()
    {
        Debug.Log ( "StartCountdown()" );
        m_countdownGameObject.SetActive ( true );
        m_countdown = m_startingValue;
        m_isCounting = true;
    }

    private void FinishedCountdown ()
    {
        m_isCounting = false;
        m_endMessageCooldown = m_endMessageDuration;
        m_countdownText.text = m_endMessage;
        m_showEndMessage = true;
    }

    private void CancelCountdown ()
    {
        m_countdownGameObject.SetActive ( false );
        m_isCounting = false;
        m_showEndMessage = false;
    }
}
