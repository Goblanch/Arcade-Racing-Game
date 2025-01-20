using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundController : MonoBehaviour
{
    public Sound[] sounds;
    public static SoundController instance;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Inicializamos los sonidos
        foreach(Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    private void Start() {
        PlaySound("MainTheme");
    }

    public void PlaySound(string soundName) {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        s.source.Play();
    }

    public void StopSound(string soundName) {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        s.source.Stop();
    }
}
