using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public enum Menus
    {
        MAIN,
        SOLO,
        COOP,
        SETTINGS,
        QUIT_CONFIRMATION
    }
    private Menus m_currentMenu = Menus.MAIN;

    [SerializeField]
    private GameObject m_mainMenuCanvas = null;
    [SerializeField]
    private GameObject m_soloPlayCanvas = null;
    [SerializeField]
    private GameObject m_coopPlayCanvas = null;
    [SerializeField]
    private GameObject m_settingsCanvas = null;
    [SerializeField]
    private GameObject m_quitConfirmationCanvas = null;

    private void OnEnable ()
    {
        CanvasFader.onFinishedFadeIn += SwitchCanvas;
    }

    private void OnDisable ()
    {
        CanvasFader.onFinishedFadeIn -= SwitchCanvas;
    }

    private void Awake ()
    {
        Debug.Assert ( m_mainMenuCanvas != null );
        Debug.Assert ( m_soloPlayCanvas != null );
        Debug.Assert ( m_coopPlayCanvas != null );
        Debug.Assert ( m_settingsCanvas != null );
        Debug.Assert ( m_quitConfirmationCanvas != null );

    }

    // Start is called before the first frame update
    void Start ()
    {
        m_mainMenuCanvas.SetActive ( true );
        m_soloPlayCanvas.SetActive ( false );
    }

    public void ChangeCanvas ( int newCanvasIndex )
    {
        m_currentMenu = ( Menus ) newCanvasIndex;
        CanvasFader.Instance.FadeIn ();
    }

    private void SwitchCanvas ()
    {
        HideAllCanvases ();

        switch ( m_currentMenu )
        {
            case Menus.MAIN:
                m_mainMenuCanvas.SetActive ( true );
                break;
            case Menus.SOLO:
                m_soloPlayCanvas.SetActive ( true );
                break;
            case Menus.COOP:
                m_coopPlayCanvas.SetActive ( true );
                break;
            case Menus.SETTINGS:
                m_settingsCanvas.SetActive ( true );
                break;
            case Menus.QUIT_CONFIRMATION:
                m_quitConfirmationCanvas.SetActive ( true );
                break;
            default:
                break;
        }

        CanvasFader.Instance.FadeOut ();
    }

    private void HideAllCanvases ()
    {
        m_mainMenuCanvas.SetActive ( false );
        m_soloPlayCanvas.SetActive ( false );
        m_coopPlayCanvas.SetActive ( false );
        m_settingsCanvas.SetActive ( false );
        m_quitConfirmationCanvas.SetActive ( false );
    }
}
