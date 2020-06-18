using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent ( typeof ( CanvasGroup ) )]
public class CanvasFader : MonoBehaviour
{
    public delegate void CanvasFadeHandler ();
    public static CanvasFadeHandler onFinishedFadeIn;
    public static CanvasFadeHandler onFinishedFadeOut;

    public static CanvasFader Instance = null;
    private CanvasGroup m_canvasGroup = null;
    [SerializeField]
    private float m_fadeSpeed = 1.0f;
    private float m_fadeCooler = 0.0f;
    private bool m_isFadingIn = false;
    private bool m_isFadingOut = false;

    private void Awake ()
    {
        if ( Instance != null && Instance != this )
        {
            Destroy ( gameObject );
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad ( gameObject );

        m_canvasGroup = GetComponent<CanvasGroup> ();
        m_fadeSpeed = Mathf.Max ( 0.1f, m_fadeSpeed );
        m_fadeCooler = m_fadeSpeed;
    }

    private void Update ()
    {
        if ( m_isFadingIn )
        {
            // Block raycasts
            m_canvasGroup.blocksRaycasts = true;

            // Fade in the canvas group
            if ( m_fadeCooler > 0.0f )
            {
                m_fadeCooler -= Time.deltaTime * m_fadeSpeed * m_fadeSpeed;
                m_canvasGroup.alpha = 1.0f - ( m_fadeCooler / m_fadeSpeed );
            }
            if ( m_fadeCooler <= 0.0f )
            {
                m_isFadingIn = false;
                m_fadeCooler = m_fadeSpeed;
                m_canvasGroup.alpha = 1.0f;
                onFinishedFadeIn?.Invoke ();
                m_canvasGroup.blocksRaycasts = false;
            }
        }
        else if ( m_isFadingOut )
        {
            // Block raycasts
            m_canvasGroup.blocksRaycasts = true;

            // Fade out the canvas group
            if ( m_fadeCooler > 0.0f )
            {
                m_fadeCooler -= Time.deltaTime * m_fadeSpeed * m_fadeSpeed;
                m_canvasGroup.alpha = m_fadeCooler / m_fadeSpeed;
            }
            if ( m_fadeCooler <= 0.0f )
            {
                m_isFadingOut = false;
                m_fadeCooler = m_fadeSpeed;
                m_canvasGroup.alpha = 0.0f;
                onFinishedFadeOut?.Invoke ();
                m_canvasGroup.blocksRaycasts = false;
            }
        }
    }

    public void FadeIn ()
    {
        m_isFadingIn = true;
    }

    public void FadeOut ()
    {
        m_isFadingOut = true;
    }
}
