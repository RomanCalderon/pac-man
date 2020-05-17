using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent ( typeof ( TimeKeeper ) )]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private TimeKeeper m_timeKeeper = null;

    private const int PLAYER_STARTING_LEVEL = 0;
    private const int PLAYER_STARTING_SCORE = 0;
    private const int PLAYER_STARTING_LIVES = 3;
    private const int COIN_SCORE_VALUE = 100;
    private const int PLAYER_DEATH_STROBE_ITERATIONS = 3;

    public delegate void PlayerStatsHandler ( PlayerStats playerStats );
    public static PlayerStatsHandler onPlayerStatsChanged;
    public delegate void PlayerEventHandler ();
    public static PlayerEventHandler onStartNewGame;
    public static PlayerEventHandler onPlayerDied;
    public static PlayerEventHandler onLevelCleared;
    public static PlayerEventHandler onLevelClosed;
    public static PlayerEventHandler onStartLevel;
    public static PlayerEventHandler onStartCountdown;
    public delegate void ResultsHandler ( PlayerResults playerResults );
    public static ResultsHandler onShareResults;

    private PlayerStats m_playerStats;
    private PlayerResults m_playerResults;
    private bool m_isContinuedGame = false;

    private int m_totalCoins = 0;
    private Coroutine m_startVillainsCoroutine = null;

    [SerializeField]
    private Transform m_gameObjectsHolder = null;
    [SerializeField]
    private Transform m_coinsHolder = null;

    [SerializeField]
    private Grid m_grid = null;

    [Header ( "Prefabs" )]
    [SerializeField]
    private Player m_playerPrefab = null;
    [SerializeField]
    private Villain [] m_villainPrefabs = null;
    [SerializeField]
    private Transform m_coinPrefab = null;

    private Player m_player = null;
    private List<Villain> m_villains = new List<Villain> ();

    [Header ( "Wall" )]
    [SerializeField]
    private Material m_wallMaterial = null;
    [SerializeField]
    private Color m_wallNormalColor = Color.blue;
    [SerializeField]
    private Color m_wallStrobeColor = Color.white;

    [Header ( "Scenes" )]
    [SerializeField]
    private string m_mainMenuSceneName = null;

    private void Awake ()
    {
        Debug.Log ( "GameManager::Awake()" );

        instance = this;

        m_timeKeeper = GetComponent<TimeKeeper> ();

        // Event subscriptions
        TimeKeeper.onTimerChanged += UpdatePlayerLevelTime;
        ResultsUIManager.onContinueGame += ContinueGame;

        Debug.Assert ( m_grid != null );
        Debug.Assert ( m_gameObjectsHolder != null );
        Debug.Assert ( m_playerPrefab != null );

    }

    private void Start ()
    {
        m_wallMaterial.color = m_wallNormalColor;

        StartNewGame ();
    }

    private void OnDisable ()
    {
        Debug.Log ( "GameManager::OnDisable() | playerTotalScore = " + m_playerStats.ScoreTotal + " | playerLevel = " + m_playerStats.Level );

        // Event unsubscriptions
        TimeKeeper.onTimerChanged -= UpdatePlayerLevelTime;
        ResultsUIManager.onContinueGame -= ContinueGame;
    }

    private void StartNewGame ()
    {
        onStartNewGame?.Invoke ();
        m_playerStats = new PlayerStats ( PLAYER_STARTING_LEVEL, PLAYER_STARTING_LIVES );
        onPlayerStatsChanged?.Invoke ( m_playerStats );

        StartNewLevel ( m_playerStats.Level + 1 );
    }

    private void StartNewLevel ( int level )
    {
        onStartLevel?.Invoke ();
        onStartCountdown?.Invoke ();

        UpdatePlayerLevel ( level );
        m_timeKeeper.ResetTime ();

        SpawnEntities ();
        PlaceCoins ();
        m_startVillainsCoroutine = StartCoroutine ( StartVillains () );

        // Start level timer
        m_timeKeeper.StartTime ( 3.0f );
    }

    private void SpawnEntities ()
    {
        m_player = Instantiate ( m_playerPrefab );
        m_player.SetGrid ( m_grid );

        m_villains = new List<Villain> ();
        foreach ( Villain villainPrefab in m_villainPrefabs )
        {
            Villain villainInstance = Instantiate ( villainPrefab );
            villainInstance.SetGrid ( m_grid );
            m_villains.Add ( villainInstance );
        }
    }

    private void PlaceCoins ()
    {
        m_totalCoins = 0;
        foreach ( Node node in m_grid.NodeArray )
        {
            if ( node.Type == Node.NodeType.DOT )
            {
                Instantiate ( m_coinPrefab, node.WorldPosition, Quaternion.identity, m_coinsHolder );
                m_totalCoins++;
            }
        }
    }

    public void ConsumedCoin ()
    {
        UpdatePlayerScore ( COIN_SCORE_VALUE );
        m_totalCoins--;
        if ( m_totalCoins <= 0 )
        {
            FinishedLevel ();
        }
    }

    public PlayerStats GetPlayerStats ()
    {
        return m_playerStats;
    }

    private void UpdatePlayerLevel ( int newLevel )
    {
        m_playerStats.Level = Mathf.Max ( 0, newLevel );
        onPlayerStatsChanged?.Invoke ( m_playerStats );
    }

    private void UpdatePlayerLevelTime ( int newLevelTime )
    {
        m_playerStats.LevelTime = Mathf.Max ( 0, newLevelTime );
        onPlayerStatsChanged?.Invoke ( m_playerStats );
    }

    private void UpdatePlayerLives ( int increment )
    {
        m_playerStats.Lives += increment;
        m_playerStats.Lives = Mathf.Max ( 0, m_playerStats.Lives );
        onPlayerStatsChanged?.Invoke ( m_playerStats );
    }

    private void UpdatePlayerScore ( int increment )
    {
        m_playerStats.ScoreLast += increment;
        onPlayerStatsChanged?.Invoke ( m_playerStats );
    }

    public void PlayerKilled ()
    {
        // Stop level timer
        m_timeKeeper.StopTime ();

        if ( m_startVillainsCoroutine != null )
        {
            StopCoroutine ( m_startVillainsCoroutine );
        }

        AudioManager.PlaySound ( "player_death", 0.5f, false );
        UpdatePlayerLives ( -1 );
        onPlayerDied?.Invoke ();

        if ( m_playerStats.Lives <= 0 )
        {
            // Game over
            StartCoroutine ( StartEndingSequence ( GoToResults ) );
        }
        else
        {
            // Reset entities
            StartCoroutine ( StartEndingSequence ( ResetEntities ) );
        }
    }

    private void ResetEntities ()
    {
        m_player.ResetPlayer ();

        for ( int i = 0; i < m_villains.Count; i++ )
        {
            m_villains [ i ].ResetVillain ();
        }

        if ( m_startVillainsCoroutine != null )
        {
            StopCoroutine ( m_startVillainsCoroutine );
        }
        m_startVillainsCoroutine = StartCoroutine ( StartVillains () );

        // Resume level timer
        m_timeKeeper.StartTime ( 3.0f );

        onStartCountdown?.Invoke ();
    }

    private void FinishedLevel ()
    {
        // Level cleared
        // Stop level timer
        m_timeKeeper.StopTime ();
        if ( m_startVillainsCoroutine != null )
        {
            StopCoroutine ( m_startVillainsCoroutine );
        }

        onLevelCleared?.Invoke ();
        StartCoroutine ( StartEndingSequence ( GoToResults ) );
    }

    private void GoToResults ()
    {
        onLevelClosed?.Invoke ();

        // Share results
        //m_gameObjectsHolder.gameObject.SetActive ( false );
        m_playerStats.ScoreTotal += m_playerStats.ScoreLast;
        m_playerResults = new PlayerResults ( m_playerStats );
        onShareResults?.Invoke ( m_playerResults );
        m_playerStats.ScoreLast = 0;
    }

    #region Event Listeners

    private void UpdatePlayerLevelTime ( float time )
    {
        int timeRoundedToInt = Mathf.RoundToInt ( time );
        m_playerStats.LevelTime = timeRoundedToInt;
        onPlayerStatsChanged?.Invoke ( m_playerStats );
    }

    private void ContinueGame ()
    {
        // Continue game
        if ( m_playerResults.GameOver )
        {
            Debug.Log ( "isNewGame" );
            Debug.Log ( "player total score = " + m_playerStats.ScoreTotal );
            StartNewGame ();
        }
        else
        {
            Debug.Log ( "isContinuedGame" );
            Debug.Log ( "player total score = " + m_playerStats.ScoreTotal );
            StartNewLevel ( m_playerStats.Level + 1 );
        }
    }

    #endregion

    #region IEnumerators

    private IEnumerator StartVillains ()
    {
        if ( m_player == null )
        {
            yield break;
        }
        yield return new WaitForSeconds ( 5.0f );
        foreach ( Villain villain in m_villains )
        {
            villain.SetTarget ( m_player.transform );
            yield return new WaitForSeconds ( 3.0f );
        }
    }

    private IEnumerator StartEndingSequence ( Action resultsCallback )
    {
        yield return new WaitForSeconds ( 1.0f );

        for ( int i = 0; i < PLAYER_DEATH_STROBE_ITERATIONS; i++ )
        {
            m_wallMaterial.color = m_wallStrobeColor;
            yield return new WaitForSeconds ( 0.5f );
            m_wallMaterial.color = m_wallNormalColor;
            yield return new WaitForSeconds ( 0.5f );
        }
        resultsCallback?.Invoke ();
    }

    #endregion
}

public struct PlayerStats
{
    public int Level;
    public int ScoreLast;
    public int ScoreTotal;
    public int Lives;
    public int LevelTime;

    public PlayerStats ( int level, int lives )
    {
        Level = level;
        Lives = lives;
        ScoreLast = ScoreTotal = 0;
        LevelTime = 0;
    }
}

public struct PlayerResults
{
    public PlayerStats PlayerStats;
    public bool GameOver;

    public PlayerResults ( PlayerStats playerStats )
    {
        PlayerStats = playerStats;
        GameOver = playerStats.Lives <= 0;
    }
}
