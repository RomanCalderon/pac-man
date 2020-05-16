using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUIManager : MonoBehaviour
{
    private const float UPDATE_STAT_INTERVAL_WAIT_TIME = 0.04f;
    private const float NEXT_STAT_DELAY_TIME = 1.0f;
    private const string GAME_STATUS_TEXT_GAMEOVER = "GAME OVER";
    private const string GAME_STATUS_TEXT_ALIVE = "STILL ALIVE";

    public delegate void ContinueGameHandler ();
    public static ContinueGameHandler onContinueGame;

    [SerializeField]
    private Canvas m_resultsCanvas = null;
    [SerializeField]
    private GameObject m_lastRoundStats = null;
    [SerializeField]
    private GameObject m_gameSummary = null;

    [Header ( "Last Round Stats" )]
    [SerializeField]
    private Text m_levelText = null;
    [SerializeField]
    private Text m_lastScoreText = null;
    [SerializeField]
    private Text m_levelTimeText = null;
    [SerializeField]
    private Button m_nextButton = null;

    [Header ( "Game Summary" )]
    [SerializeField]
    private Text m_totalScoreText = null;
    [SerializeField]
    private Text m_livesRemainingText = null;
    [SerializeField]
    private Text m_gameStatusText = null;
    [SerializeField]
    private Button m_continueButton = null;


    private PlayerResults m_playerResults;

    private void OnEnable ()
    {
        GameManager.onLevelClosed += DisplayResultsCanvas;
        GameManager.onShareResults += UpdateResults;
        GameManager.onStartLevel += HideResultsCanvas;
    }

    private void OnDisable ()
    {
        GameManager.onLevelClosed -= DisplayResultsCanvas;
        GameManager.onShareResults -= UpdateResults;
        GameManager.onStartLevel -= HideResultsCanvas;
    }

    // Start is called before the first frame update
    void Start ()
    {
        m_resultsCanvas.gameObject.SetActive ( false );

        m_lastRoundStats.SetActive ( false );
        m_levelText.gameObject.SetActive ( false );
        m_lastScoreText.gameObject.SetActive ( false );
        m_levelTimeText.gameObject.SetActive ( false );
        m_nextButton.gameObject.SetActive ( false );

        m_gameSummary.SetActive ( false );
        m_totalScoreText.gameObject.SetActive ( false );
        m_livesRemainingText.gameObject.SetActive ( false );
        m_gameStatusText.gameObject.SetActive ( false );
        m_continueButton.gameObject.SetActive ( false );
    }

    private void DisplayResultsCanvas ()
    {
        m_resultsCanvas.gameObject.SetActive ( true );

        m_lastRoundStats.SetActive ( false );
        m_levelText.gameObject.SetActive ( false );
        m_lastScoreText.gameObject.SetActive ( false );
        m_levelTimeText.gameObject.SetActive ( false );
        m_nextButton.gameObject.SetActive ( false );

        m_gameSummary.SetActive ( false );
        m_totalScoreText.gameObject.SetActive ( false );
        m_livesRemainingText.gameObject.SetActive ( false );
        m_gameStatusText.gameObject.SetActive ( false );
        m_continueButton.gameObject.SetActive ( false );

        // Init text values
        m_levelText.text = "0";
        m_lastScoreText.text = "0";
        m_levelTimeText.text = "0s";

        m_totalScoreText.text = "0";
        m_livesRemainingText.text = "0";
        m_gameStatusText.text = GAME_STATUS_TEXT_ALIVE;
    }

    private void HideResultsCanvas ()
    {
        m_resultsCanvas.gameObject.SetActive ( false );
    }

    private void UpdateResults ( PlayerResults playerResults )
    {
        m_playerResults = playerResults;
        StartCoroutine ( DisplayLastRoundStats () );
    }

    public void GoToGameSummary ()
    {
        m_lastRoundStats.SetActive ( false );
        StartCoroutine ( DisplayGameSummary () );
    }

    public void ContinueGame ()
    {
        onContinueGame?.Invoke ();
    }


    #region IEnumerators

    private IEnumerator DisplayLastRoundStats ()
    {
        yield return new WaitForSeconds ( 1.0f );

        m_lastRoundStats.SetActive ( true );
        yield return new WaitForSeconds ( NEXT_STAT_DELAY_TIME );

        // Level
        m_levelText.gameObject.SetActive ( true );
        m_levelText.text = m_playerResults.PlayerStats.Level.ToString ();
        yield return new WaitForSeconds ( NEXT_STAT_DELAY_TIME );

        // Last Score
        m_lastScoreText.gameObject.SetActive ( true );
        yield return StartCoroutine ( UpdateStat ( m_lastScoreText, 0, m_playerResults.PlayerStats.ScoreLast ) );
        yield return new WaitForSeconds ( NEXT_STAT_DELAY_TIME );

        // Time
        m_levelTimeText.gameObject.SetActive ( true );
        yield return StartCoroutine ( UpdateStat ( m_levelTimeText, 0, m_playerResults.PlayerStats.LevelTime, "s" ) );
        yield return new WaitForSeconds ( NEXT_STAT_DELAY_TIME );

        // Display next button
        m_nextButton.gameObject.SetActive ( true );
    }

    private IEnumerator DisplayGameSummary ()
    {
        yield return new WaitForSeconds ( 0.5f );
        m_gameSummary.SetActive ( true );

        yield return new WaitForSeconds ( NEXT_STAT_DELAY_TIME );

        // Total Score
        int oldScore = m_playerResults.PlayerStats.ScoreTotal - m_playerResults.PlayerStats.ScoreLast;
        m_totalScoreText.gameObject.SetActive ( true );
        yield return StartCoroutine ( UpdateStat ( m_totalScoreText, oldScore, m_playerResults.PlayerStats.ScoreTotal ) );
        yield return new WaitForSeconds ( NEXT_STAT_DELAY_TIME );

        // Lives Remaining
        m_livesRemainingText.gameObject.SetActive ( true );
        m_livesRemainingText.text = m_playerResults.PlayerStats.Lives.ToString ();
        yield return new WaitForSeconds ( NEXT_STAT_DELAY_TIME );

        // Game Status
        m_gameStatusText.gameObject.SetActive ( true );
        m_gameStatusText.text = m_playerResults.GameOver ? GAME_STATUS_TEXT_GAMEOVER : GAME_STATUS_TEXT_ALIVE;
        yield return new WaitForSeconds ( NEXT_STAT_DELAY_TIME );

        // Display continue button
        m_continueButton.gameObject.SetActive ( true );
    }


    private IEnumerator UpdateStat ( Text textElement, int oldStat, int newStat, string suffix = "" )
    {
        int statValue = oldStat;
        textElement.text = oldStat.ToString ();
        float updateInterval = Mathf.Abs ( newStat - oldStat ) / 10f;

        if ( newStat > oldStat )
        {
            while ( statValue < newStat )
            {
                textElement.text = statValue.ToString () + suffix;
                statValue += ( int ) updateInterval;
                yield return new WaitForSeconds ( UPDATE_STAT_INTERVAL_WAIT_TIME );
            }
        }
        else if ( newStat < oldStat )
        {
            while ( statValue > newStat )
            {
                textElement.text = statValue.ToString () + suffix;
                statValue -= ( int ) updateInterval;
                yield return new WaitForSeconds ( UPDATE_STAT_INTERVAL_WAIT_TIME );
            }
        }
        statValue = newStat;
        textElement.text = statValue.ToString () + suffix;
    }

    #endregion
}
