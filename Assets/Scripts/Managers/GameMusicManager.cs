using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicManager : MonoBehaviour
{
    [SerializeField]
    private string m_gameMusicName = "lines_of_code";
    [SerializeField, Range ( 0.0f, 1.0f )]
    private float m_gameMusicVolume = 0.1f;
    [SerializeField]
    private string m_resultsMusicName = "bgm_action_4";
    [SerializeField, Range ( 0.0f, 1.0f )]
    private float m_resultsMusicVolume = 0.4f;


    private void OnEnable ()
    {
        GameManager.onStartLevel += PlayGameMusic;
        GameManager.onLevelClosed += PlayResultsMusic;
    }

    private void OnDisable ()
    {
        GameManager.onStartLevel -= PlayGameMusic;
        GameManager.onLevelClosed -= PlayResultsMusic;
    }


    private void PlayGameMusic ()
    {
        StopGameMusic ();
        AudioManager.PlaySound ( m_gameMusicName, m_gameMusicVolume, true );
    }

    private void PlayResultsMusic ()
    {
        StopGameMusic ();
        AudioManager.PlaySound ( m_resultsMusicName, m_resultsMusicVolume, true );
    }

    private void StopGameMusic ()
    {
        AudioManager.StopAll ();
    }
}
