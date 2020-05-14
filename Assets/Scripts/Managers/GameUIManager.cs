using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private Image m_playerLivesMask = null;
    [SerializeField]
    private Text m_playerScoreText = null;
    [SerializeField]
    private Text m_playerLevelTimeText = null;

    private void OnEnable ()
    {
        GameManager.onPlayerLivesChanged += UpdatePlayerLives;
        GameManager.onPlayerScoreChanged += UpdatePlayerScore;
        GameManager.onPlayerLevelTimeChanged += UpdatePlayerLevelTime;
    }

    private void OnDisable ()
    {
        GameManager.onPlayerLivesChanged -= UpdatePlayerLives;
        GameManager.onPlayerScoreChanged -= UpdatePlayerScore;
        GameManager.onPlayerLevelTimeChanged -= UpdatePlayerLevelTime;
    }

    // Start is called before the first frame update
    void Awake ()
    {
        Debug.Assert ( m_playerLivesMask != null );
        Debug.Assert ( m_playerScoreText != null );
        Debug.Assert ( m_playerLevelTimeText != null );
    }

    private void UpdatePlayerLives ( int lives )
    {
        lives = Mathf.Clamp ( lives, 0, 3 );
        m_playerLivesMask.fillAmount = lives / 3f;
    }

    private void UpdatePlayerScore ( int score )
    {
        score = Mathf.Max ( 0, score );
        m_playerScoreText.text = score.ToString ();
    }

    private void UpdatePlayerLevelTime ( int time )
    {
        time = Mathf.Max ( 0, time );
        m_playerLevelTimeText.text = time.ToString () + "s";
    }
}
