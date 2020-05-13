using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private const int PLAYER_STARTING_SCORE = 0;
    private const int PLAYER_STARTING_LIVES = 3;

    private int m_currentLevel = 1;
    private PlayerStats m_playerStats;
    private int m_totalCoins = 0;

    [SerializeField]
    private Grid m_grid = null;

    [SerializeField]
    private Transform m_coinPrefab = null;

    private void Awake ()
    {
        if ( instance == null )
        {
            instance = this;
        }

        Debug.Assert ( m_grid != null );

        m_playerStats = new PlayerStats ( PLAYER_STARTING_SCORE, PLAYER_STARTING_LIVES );
    }

    // Start is called before the first frame update
    void Start ()
    {
        PlaceCoins ();
    }

    // Update is called once per frame
    void Update ()
    {

    }

    private void PlaceCoins ()
    {
        foreach ( Node node in m_grid.NodeArray )
        {
            if ( node.Type == Node.NodeType.DOT )
            {
                Instantiate ( m_coinPrefab, node.WorldPosition, Quaternion.identity, transform );
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

    private void FinishedLevel ()
    {
        Debug.Log ( "Level cleared!" );
    }

    private void UpdatePlayerScore ( int increment )
    {
        m_playerStats.Score += increment;
    }

    public void UpdatePlayerLives ( int increment )
    {
        m_playerStats.Lives += increment;
    }

    public int GetPlayerScore ()
    {
        return m_playerStats.Score;
    }

    public int GetPlayerLives ()
    {
        return m_playerStats.Lives;
    }
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
