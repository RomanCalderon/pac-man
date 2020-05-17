using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCountdown : MonoBehaviour
{
    public delegate void CountdownHandler ( float countdown );
    public static CountdownHandler onCountdownChanged;
    public static CountdownHandler onFinishedCountdown;

    [SerializeField]
    private GameObject m_countdownGameObject = null;
    [SerializeField]
    private int m_startingValue = 3;
    [SerializeField]
    private float m_countdownSpeed = 1.0f;
    [SerializeField]
    private AudioClip m_countdownAudioClip = null;
    [SerializeField]
    private string m_endMessage = "GO!";
    [SerializeField]
    private float m_endMessageDuration = 0.5f;
    [SerializeField]
    private AudioClip m_endMessageAudioClip = null;
    [SerializeField]
    private Text m_countdownText = null;
    private bool m_isCounting = false;
    private float m_countdown = 0.0f;
    public float Countdown
    {
        get
        {
            return m_countdown;
        }
        private set
        {
            float oldValue = m_countdown;
            m_countdown = value;
            float ceiledValue = Mathf.CeilToInt ( value );

            if ( ceiledValue < oldValue )
            {
                onCountdownChanged?.Invoke ( ceiledValue );
            }
        }
    }

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
        onCountdownChanged += PlayCountdownSound;
    }

    private void OnDisable ()
    {
        GameManager.onStartCountdown -= StartCountdown;
        onCountdownChanged -= PlayCountdownSound;
    }

    // Update is called once per frame
    void Update ()
    {
        if ( m_isCounting )
        {
            if ( m_countdown > 0 )
            {
                Countdown -= Time.deltaTime * m_countdownSpeed;
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
        m_countdownGameObject.SetActive ( true );
        m_countdown = m_startingValue;
        m_isCounting = true;
        PlayCountdownSound ( m_startingValue );
    }

    private void FinishedCountdown ()
    {
        m_isCounting = false;
        m_endMessageCooldown = m_endMessageDuration;
        m_countdownText.text = m_endMessage;
        m_showEndMessage = true;
        PlayEndMessageSound ();
    }

    private void CancelCountdown ()
    {
        m_countdownGameObject.SetActive ( false );
        m_isCounting = false;
        m_showEndMessage = false;
        onFinishedCountdown?.Invoke ( m_countdown );
    }

    private void PlayCountdownSound ( float countdown )
    {
        if ( m_countdownAudioClip == null )
        {
            return;
        }
        AudioManager.PlaySound ( m_countdownAudioClip, 0.5f, false );
    }

    private void PlayEndMessageSound ()
    {
        if ( m_endMessageAudioClip == null )
        {
            return;
        }
        AudioManager.PlaySound ( m_endMessageAudioClip, 0.5f, false );
    }
}
