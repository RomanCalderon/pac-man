using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private const int PLAYER_STARTING_SCORE = 0;
    private const int PLAYER_STARTING_LIVES = 3;
    private const int PLAYER_DEATH_STROBE_ITERATIONS = 4;

    public delegate void PlayerHandler ();
    public static PlayerHandler onPlayerDied;
    public static PlayerHandler onLevelCleared;

    private int m_currentLevel = 1;
    private PlayerStats m_playerStats;
    private int m_totalCoins = 0;

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
    private Color m_wallOriginalColor;
    private Color m_wallStrobeColor = Color.white;

    private bool m_gameOver = false;
    [Header ( "Scenes" )]
    [SerializeField]
    private string m_gameSceneName;
    [SerializeField]
    private string m_resultsSceneName;

    private void Awake ()
    {
        if ( instance == null )
        {
            instance = this;
        }
        else if ( instance != this )
        {
            Destroy ( gameObject );
        }
        DontDestroyOnLoad ( gameObject );

        Debug.Assert ( m_gameObjectsHolder != null );
        Debug.Assert ( m_grid != null );
        Debug.Assert ( m_playerPrefab != null );
        Debug.Assert ( m_wallMaterial != null );

        m_playerStats = new PlayerStats ( PLAYER_STARTING_SCORE, PLAYER_STARTING_LIVES );
        m_wallOriginalColor = m_wallMaterial.color;
    }

    // Start is called before the first frame update
    void Start ()
    {
        SpawnEntities ();
        PlaceCoins ();
        StartCoroutine ( StartVillains () );
    }

    // Update is called once per frame
    void Update ()
    {

    }

    private void SpawnEntities ()
    {
        m_player = Instantiate ( m_playerPrefab );
        m_player.SetGrid ( m_grid );

        foreach ( Villain villainPrefab in m_villainPrefabs )
        {
            Villain villainInstance = Instantiate ( villainPrefab );
            villainInstance.SetGrid ( m_grid );
            m_villains.Add ( villainInstance );
        }
    }

    private void PlaceCoins ()
    {
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
        UpdatePlayerScore ( 1 );
        m_totalCoins--;
        if ( m_totalCoins <= 0 )
        {
            FinishedLevel ();
        }
    }

    public int GetPlayerScore ()
    {
        return m_playerStats.Score;
    }

    public int GetPlayerLives ()
    {
        return m_playerStats.Lives;
    }

    private void UpdatePlayerScore ( int increment )
    {
        m_playerStats.Score += increment;
    }

    private void UpdatePlayerLives ( int increment )
    {
        m_playerStats.Lives += increment;
    }

    public void PlayerKilled ()
    {
        Debug.Log ( "Player got killed!" );
        AudioManager.PlaySound ( "player_death", 0.5f );
        UpdatePlayerLives ( -1 );
        onPlayerDied?.Invoke ();

        if ( m_playerStats.Lives <= 0 )
        {
            // Game over
            StartCoroutine ( StartEndingSequence ( GoToResults ) );
        }
        else
        {
            // Reset player
            StartCoroutine ( StartEndingSequence ( ResetEntities ) );
        }
    }

    private void ResetEntities ()
    {
        m_player.ResetPlayer ();

        foreach ( Villain villain in m_villains )
        {
            villain.ResetVillain ();
        }
    }

    private void FinishedLevel ()
    {
        // Level cleared
        onLevelCleared?.Invoke ();
        StartCoroutine ( StartEndingSequence ( GoToResults ) );
    }

    private void GoToResults ()
    {
        m_gameOver = m_playerStats.Lives <= 0;
        m_gameObjectsHolder.gameObject.SetActive ( false );

        SceneManager.LoadScene ( m_resultsSceneName );
    }

    #region IEnumerators

    private IEnumerator StartVillains ()
    {
        yield return new WaitForSeconds ( 3.0f );
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
            m_wallMaterial.color = m_wallOriginalColor;
            yield return new WaitForSeconds ( 0.5f );
        }

        yield return new WaitForSeconds ( 1.0f );
        resultsCallback?.Invoke ();
    }

    #endregion
}

public struct PlayerStats
{
    public int Score;
    public int Lives;

    public PlayerStats ( int score, int lives )
    {
        Score = score;
        Lives = lives;
    }
}
