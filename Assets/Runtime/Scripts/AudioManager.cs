using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ExtensionMethods;

[Serializable]
public class Sound {
    public string name;
    // public AudioClip clip;
    public AudioClip[] clips;

    public AudioClip GetClip(bool random = false) {
        if (random) {
            return RandomClip();
        }

        return clips[0];
    }

    public AudioClip RandomClip() {
        int i = UnityEngine.Random.Range(0, clips.Length - 1);
        return clips[i];
    }
}

public class AudioManager : MonoBehaviour {
    [Header("General References")]
    [Space(5)]
    public static AudioManager instance;

    public Sound[] musicSounds;
    public Sound[] ambienceSounds;
    public Sound[] sfxSounds;

    public AudioSource musicSource;
    public AudioSource ambienceSource;
    public AudioSource sfxSource;

    public IntSO MusicVolume;
    public IntSO AmbienceVolume;
    public IntSO SFXVolume;

    // private void OnEnable() {
    //     MusicVolume.OnValueChange += ChangeMusicVolume;
    //     AmbienceVolume.OnValueChange += ChangeAmbienceVolume;
    //     SFXVolume.OnValueChange += ChangeSFXVolume;
    // }

    // private void OnDisable() {
    //     MusicVolume.OnValueChange -= ChangeMusicVolume;
    //     AmbienceVolume.OnValueChange -= ChangeAmbienceVolume;
    //     SFXVolume.OnValueChange -= ChangeSFXVolume;
    // }

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
    }

    void Update() {
        ChangeMusicVolume(MusicVolume);
        ChangeAmbienceVolume(AmbienceVolume);
        ChangeSFXVolume(SFXVolume);
    }

    public void PlayMusic(string name , bool random) {
        Sound sound = Array.Find(musicSounds, x => x.name == name);

        if (sound.IsNull()) Debug.Log("Sound not found");
        else {
            musicSource.clip = sound.GetClip(random);
            musicSource.Play();
        }
    }

    // public void PlayMusic(AudioClip clip) {
    //     musicSource.clip = clip;
    //     musicSource.Play();
    // }

    public void PlayAmbience(string name, bool random) {
        Sound sound = Array.Find(ambienceSounds, x => x.name == name);

        if (sound.IsNull()) Debug.Log("Sound not found");
        else {
            ambienceSource.clip = sound.GetClip(random);
            ambienceSource.Play();
        }
    }

    public void PlayAmbience(AudioClip clip) {
        ambienceSource.clip = clip;
        ambienceSource.Play();
    }

    public void PlaySFX(string name, bool random) {
        Sound sound = Array.Find(sfxSounds, x => x.name == name);

        if (sound.IsNull()) Debug.Log("Sound not found");
        else sfxSource.PlayOneShot(sound.GetClip(random));
    }

    public AudioClip GetMusicClip(string name, bool random) {
        return Array.Find(musicSounds, x => x.name == name).GetClip(random);
    }

    public AudioClip GetAmbienceClip(string name, bool random) {
        return Array.Find(ambienceSounds, x => x.name == name).GetClip(random);
    }

    public AudioClip GetSFXClip(string name, bool random) {
        return Array.Find(sfxSounds, x => x.name == name).GetClip(random);
    }

    // public void PlaySFX(AudioClip clip) {
    //     sfxSource.PlayOneShot(clip);
    // }

    // public void PlayClip(AudioSource source, AudioClip clip, bool oneShot) {
    //     if (oneShot) {
    //         source.PlayOneShot(clip);
    //     }
    //     else {
    //         source.clip = clip;
    //         source.Play();
    //     }
    // }

    public void StopMusic() {
        musicSource.Stop();
    }

    public void StopAmbience() {
        ambienceSource.Stop();
    }

    public void TogglePauseMusic(bool on) {
        if (on) musicSource.UnPause();
        else musicSource.Pause();
    }

    public void TogglePauseAmbience(bool on) {
        if (on) ambienceSource.UnPause();
        else ambienceSource.Pause();
    }

    public void ToggleMusic() {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleAmbience() {
        ambienceSource.mute = !ambienceSource.mute;
    }

    public void ToggleSFX() {
        sfxSource.mute = !sfxSource.mute;
    }

    public void ChangeMusicVolume(ValueSO<int> volume) {
        musicSource.volume = volume.Value / 100f;
    }

    public void ChangeAmbienceVolume(ValueSO<int> volume) {
        musicSource.volume = volume.Value / 100f;
    }

    public void ChangeSFXVolume(ValueSO<int> volume) {
        sfxSource.volume = volume.Value / 100f;
    }
}
