using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    private static List<GameObject> m_sounds = new List<GameObject> ();

    public static void PlaySound ( string clipName, float volume, bool loop )
    {
        GameObject soundGameObject = new GameObject ( string.Format ( "Sound [{0}]", clipName ) );
        m_sounds.Add ( soundGameObject );
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource> ();
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = GameAssets.Instance.GetAudioMixerGroup ( clipName );
        audioSource.loop = loop;
        AudioClip clip = GameAssets.Instance.GetAudioClip ( clipName );
        audioSource.clip = clip;
        audioSource.Play ();
        if ( !loop )
        {
            Object.Destroy ( soundGameObject, clip.length );
        }
    }

    public static void PlaySound ( AudioClip clip, float volume, bool loop )
    {
        GameObject soundGameObject = new GameObject ( string.Format ( "Sound [{0}]", clip.name ) );
        m_sounds.Add ( soundGameObject );
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource> ();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play ();
        if ( !loop )
        {
            Object.Destroy ( soundGameObject, clip.length );
        }
    }

    public static void StopAll ()
    {
        foreach ( GameObject go in m_sounds )
        {
            Object.Destroy ( go );
        }
        m_sounds.Clear ();
    }
}
