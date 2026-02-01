using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public enum AudioLabel
{
    ThemeMusic, FightSFX, BellSFX, PickCharacterVoice, VictoryVoice,
    SwapMenuSFX, 
    Punch1SFX, Punch2SFX, Punch3SFX, Punch4SFX,
    Character1Throw, Character2Throw,
    Character1Ouch1, Character1Ouch2, Character1Ouch3,
    Character2Ouch1, Character2Ouch2, Character2Ouch3,
    Character1Voice1, Character1Voice2, Character1Voice3,
    Character1VoiceWin, Character1VoiceLoss,
    Character2Voice1, Character2Voice2, Character2Voice3, 
    Character2VoiceWin, Character2VoiceLoss,
    UnmaskHimSFX, MaskalitySFX, ApplauseSFX
}

[Serializable]
public class AudioClipMapping
{
    public AudioLabel Label;
    public AudioClip Clip;
}

public class AudioManager : MonoBehaviour
{
    public AudioMixer MainMixer;
    public AudioSource MusicSource;
    public GameObject SFXSourceParent;
    public GameObject SFXSourcePrefab;
    public List<AudioSource> SFXInPlay = new List<AudioSource>();

    //mapping
    public List<AudioClipMapping> AudioClips;

    //Singleton pattern
    public static AudioManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); //keep this between scenes. NB! GameObject must be placed on root level on hierarchy!
            return;
        }
        Destroy(this.gameObject);
    }

    public void PlaySFXClip(AudioLabel audioLabel)
    {
        var sfxGameObject = Instantiate(SFXSourcePrefab, SFXSourceParent.transform);
        var sfxSource = sfxGameObject.GetComponent<AudioSource>();
        sfxSource.clip = AudioClips.FirstOrDefault(e => e.Label == audioLabel).Clip;
        sfxSource.Play();
        SFXInPlay.Add(sfxSource);
    }

    public void DestroySFXClips(AudioLabel audioLabel)
    {
        var clip = AudioClips.FirstOrDefault(e => e.Label == audioLabel).Clip;
        var sfxToDestroy = new List<AudioSource>();
        foreach (var sfxSource in SFXInPlay)
        {
            if (sfxSource.clip == clip)
            {
                sfxToDestroy.Add(sfxSource);
            }
        }
        for (int i = sfxToDestroy.Count - 1; i >= 0; i--)
        {
            SFXInPlay.Remove(sfxToDestroy[i]);
            var gameObjectToDestroy = sfxToDestroy[i].gameObject;
            sfxToDestroy.RemoveAt(i);
            Destroy(gameObjectToDestroy);
        }
    }

    public void PlayMusicClip(AudioLabel audioLabel)
    {
        var audioClip = AudioClips.FirstOrDefault(e => e.Label == audioLabel).Clip;
        MusicSource.clip = audioClip;
        MusicSource.Play(); //loops
    }

    public void FixedUpdate()
    {
        //check if any SFX are done playing, clean up objects when done
        if (SFXInPlay.Count > 0)
        {
            var sfxToDestroy = new List<AudioSource>();
            foreach (var sfxSource in SFXInPlay)
            {
                if (!sfxSource.isPlaying)
                {
                    sfxToDestroy.Add(sfxSource);
                }
            }
            for (int i = sfxToDestroy.Count - 1; i >= 0; i--)
            {
                SFXInPlay.Remove(sfxToDestroy[i]);
                var gameObjectToDestroy = sfxToDestroy[i].gameObject;
                sfxToDestroy.RemoveAt(i);
                Destroy(gameObjectToDestroy);
            }
        }
    }
}
