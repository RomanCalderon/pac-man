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

    private void Awake ()
    {
        GameManager.onPlayerLivesChanged += UpdatePlayerLives;
        GameManager.onPlayerScoreChanged += UpdatePlayerScore;
    }

    // Start is called before the first frame update
    void Start ()
    {
        Debug.Assert ( m_playerLivesMask != null );
        Debug.Assert ( m_playerScoreText != null );
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

    private void OnDisable ()
    {
        GameManager.onPlayerLivesChanged -= UpdatePlayerLives;
        GameManager.onPlayerScoreChanged -= UpdatePlayerScore;
    }
}
