using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private int m_gameSceneIndex = 1;

    // Loads the game scene
    public void LoadGameScene ()
    {
        SceneManager.LoadScene ( m_gameSceneIndex );
    }
}
