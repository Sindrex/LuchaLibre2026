using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSoundController : MonoBehaviour
{
    public PlayerSounds Sounds;

    public void Grab()
    {
        AudioManager.Instance.PlaySFXClip(Sounds.Grab);
    }

    public void Throw()
    {
        AudioManager.Instance.PlaySFXClip(Sounds.Throw);
    }
}

[CreateAssetMenu(menuName = "LuchaLibre/PlayerSounds")]
[Serializable]
public class PlayerSounds : ScriptableObject
{
    public AudioLabel Throw;
    public AudioLabel Grab;
    public List<AudioLabel> Ouches;
    public List<AudioLabel> Voices;
    public AudioLabel VoiceWin;
    public AudioLabel VoiceLoss;
}