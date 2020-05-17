using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private float m_shakeAmount = 0.0f;
    [SerializeField]
    private float m_shakeDuration = 0.0f;
    private float m_shakeCountdown = 0.0f;
    private bool m_isShaking = false;
    private Vector3 m_originalPosition;

    private void OnEnable ()
    {
        GameManager.onPlayerDied += BeginShake;
    }

    private void OnDisable ()
    {
        GameManager.onPlayerDied -= BeginShake;
    }

    // Start is called before the first frame update
    void Awake ()
    {
        m_originalPosition = transform.localPosition;
    }

    private void Update ()
    {
        Shake ();
    }

    private void BeginShake ()
    {
        m_shakeCountdown = m_shakeDuration;
        m_isShaking = true;
    }

    private void Shake ()
    {
        if ( m_shakeAmount > 0.0f && m_shakeCountdown > 0.0f )
        {
            Vector3 camPosition = transform.position;

            float offsetX = Random.value * m_shakeAmount * 2.0f - m_shakeAmount;
            float offsetY = Random.value * m_shakeAmount * 2.0f - m_shakeAmount;
            camPosition.x += offsetX;
            camPosition.y += offsetY;

            transform.position = camPosition;
            m_shakeCountdown -= Time.deltaTime;
        }
        else if ( m_isShaking )
        {
            StopShake ();
        }
    }

    private void StopShake ()
    {
        transform.localPosition = m_originalPosition;
        m_isShaking = false;
    }
}
