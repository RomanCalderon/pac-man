using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;

    public static GameAssets instance
    {
        get
        {
            if ( _instance == null )
            {
                _instance = Instantiate ( Resources.Load<GameAssets> ( "GameAssets" ) );
                return _instance;
            }
            return _instance;
        }
    }

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClipReference [] AudioClips = null;

    public AudioClip GetAudioClip ( string name )
    {
        return AudioClips.First ( c => c.Name == name ).AudioClip;
    }
}

[System.Serializable]
public struct AudioClipReference
{
    public string Name;
    public AudioClip AudioClip;
}
