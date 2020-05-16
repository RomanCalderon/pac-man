using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent ( typeof ( Button ) )]
public class ButtonHelper : MonoBehaviour
{
    private Button m_button = null;

    [SerializeField]
    private KeyCode m_shortcutKey = KeyCode.Space;
    [SerializeField]
    private string m_hoverClipName = null;
    [SerializeField]
    private float m_hoverSoundVolume = 1.0f;
    [SerializeField]
    private string m_clickClipName = null;
    [SerializeField]
    private float m_clickSoundVolume = 1.0f;

    private void Awake ()
    {
        m_button = GetComponent<Button> ();
    }

    // Update is called once per frame
    void Update ()
    {
        if ( Input.GetKeyDown ( m_shortcutKey ) && m_button.IsInteractable () )
        {
            m_button.Select ();
        }
    }

    public void OnButtonHover ()
    {
        AudioManager.PlaySound ( m_hoverClipName, m_hoverSoundVolume );
    }

    public void OnButtonClicked ()
    {
        AudioManager.PlaySound ( m_clickClipName, m_clickSoundVolume );
    }
}
