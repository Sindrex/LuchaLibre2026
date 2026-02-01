using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSoundController : MonoBehaviour
{
    public PlayerSounds Sounds;

    public void GrabThrown()
    {
        AudioManager.Instance.PlaySFXClip(Sounds.Grab);
    }

    public void GrabThrowMiss()
    {
        var randomNumber = new System.Random().Next(0, Sounds.Voices.Count - 1);
        var sfxClip = Sounds.Voices[randomNumber];
        AudioManager.Instance.PlaySFXClip(sfxClip);
    }

    public void Punch()
    {
        var randomNumber = new System.Random().Next(0, Sounds.Voices.Count - 1);
        var sfxClip = Sounds.Voices[randomNumber];
        AudioManager.Instance.PlaySFXClip(sfxClip);
    }

    public void Thrown()
    {
        AudioManager.Instance.PlaySFXClip(Sounds.Throw);
    }
}

[CreateAssetMenu(menuName = "LuchaLibre/PlayerSounds")]
[Serializable]
public class PlayerSounds : ScriptableObject
{
    public AudioLabel Throw; //thrown
    public AudioLabel Grab; //grabthrow, grabthrow miss
    public List<AudioLabel> Ouches; //hit
    public List<AudioLabel> Voices; //punch, grabthrow miss
    public AudioLabel VoiceWin;
    public AudioLabel VoiceLoss;
}