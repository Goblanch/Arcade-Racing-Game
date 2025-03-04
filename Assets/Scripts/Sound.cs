using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public string tag;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    public bool loop;
    [HideInInspector]
    public AudioSource source;
}
