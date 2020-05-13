using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent ( typeof ( Image ) )]
public class ImageFillSlider : MonoBehaviour
{
    private enum FillTypes
    {
        FILL,
        DRAIN
    }

    private Image m_image = null;

    [SerializeField]
    private FillTypes m_fillType = FillTypes.FILL;
    [SerializeField]
    private float m_fillDelay = 1.0f;
    [SerializeField, Range ( 0.01f, 10.0f )]
    private float m_fillSpeed = 1.0f;

    private void Awake ()
    {
        m_image = GetComponent<Image> ();
    }

    private void Start ()
    {
        StartCoroutine ( StartFill () );
    }

    private IEnumerator StartFill ()
    {
        yield return new WaitForSeconds ( m_fillDelay );

        switch ( m_fillType )
        {
            case FillTypes.FILL:
                while ( m_image.fillAmount < 1.0f )
                {
                    m_image.fillAmount += Time.deltaTime * m_fillSpeed;
                    yield return null;
                }
                break;
            case FillTypes.DRAIN:
                while ( m_image.fillAmount > 0.0f )
                {
                    m_image.fillAmount -= Time.deltaTime * m_fillSpeed;
                    yield return null;
                }
                break;
            default:
                break;
        }
    }
}
