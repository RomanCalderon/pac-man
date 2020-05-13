using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    private static List<GameObject> m_sounds = new List<GameObject> ();

    public static void PlaySound ( string clipName, float volume )
    {
        GameObject soundGameObject = new GameObject ( string.Format ( "Sound [{0}]", clipName ) );
        m_sounds.Add ( soundGameObject );
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource> ();
        audioSource.volume = volume;
        AudioClip clip = GameAssets.instance.GetAudioClip ( clipName );
        audioSource.PlayOneShot ( clip );
        Object.Destroy ( soundGameObject, clip.length );
    }

    public static void StopAll()
    {
        foreach ( GameObject go in m_sounds )
        {
            Object.Destroy ( go );
        }
    }
}
