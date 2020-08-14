using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Audio2DPlayer : AudioPlayerBase {
    
    /// <summary>
    /// 相同音效生成冷却时间
    /// </summary>
    public const float SFX_COOLDOWN_TIME = 0.05f;
    
    public Dictionary<int, AudioAsset> bgMusicDic = new Dictionary<int, AudioAsset>();
    public List<AudioAsset> sfxList = new List<AudioAsset>();

    public Audio2DPlayer(MonoBehaviour mono) : base(mono) { }

    public override void SetMusicVolume(float volume)
    {
        base.SetMusicVolume(volume);
        foreach (var item in bgMusicDic.Values)
        {
            item.TotleVolume = volume;
        }
    }
    public override void SetSFXVolume(float volume)
    {
        base.SetSFXVolume(volume);
        for (int i = 0; i < sfxList.Count; i++)
        {
            sfxList[i].TotleVolume = volume;
        }
    }

    public void PlayMusic(int channel, string audioName, bool isLoop = true, float volumeScale = 1, float delay = 0f, float fadeTime = 0.5f, string flag = "")
    {
        AudioAsset au;

        if (bgMusicDic.ContainsKey(channel))
        {
            au = bgMusicDic[channel];
        }
        else
        {
            au = CreateAudioAssetByPool(mono.gameObject, false,  AudioSourceType.Music);
            bgMusicDic.Add(channel, au);
        }

        PlayMusicControl(au, audioName, isLoop, volumeScale, delay, fadeTime, flag);
    }
    public void PauseMusic(int channel, bool isPause, float fadeTime = 0.5f)
    {
        if (bgMusicDic.ContainsKey(channel))
        {
            AudioAsset au = bgMusicDic[channel];
            PauseMusicControl(au, isPause, fadeTime);

        }
    }
    public void PauseMusicAll(bool isPause, float fadeTime = 0.5f)
    {
        foreach (int i in bgMusicDic.Keys)
        {
            PauseMusic(i, isPause, fadeTime);
        }
    }

    public void StopMusic(int channel, float fadeTime = 0.5f)
    {
        if (bgMusicDic.ContainsKey(channel))
        {
            StopMusicControl(bgMusicDic[channel], fadeTime);
        }

    }

    public void StopMusicAll()
    {
        foreach (int i in bgMusicDic.Keys)
        {
            StopMusic(i);
        }
    }

    public void PlaySFX(string name, float volumeScale = 1f, float delay = 0f, float pitch = 1, string flag = "")
    {
        if ( SfxPlaying.ContainsKey ( name ) ) {
            if ( SfxPlaying[ name ] <= 0f ) {
                SfxPlaying[ name ] = SFX_COOLDOWN_TIME;
            }
            else {
                //            Debug.Log ( $"音效在同一帧多次出现:{name}，忽略" );
                return;
            }
        }
        else {
            SfxPlaying.Add ( name, SFX_COOLDOWN_TIME );
        }
        
        AudioAsset au = GetEmptyAudioAssetFromSFXList();
        au.flag = flag;
        PlayClip(au, name, false, volumeScale, delay, pitch);

    }
    public void PauseSFXAll(bool isPause)
    {
        for (int i = 0; i < sfxList.Count; i++)
        {
            if (isPause)
            {
                    sfxList[i].Pause();
            }
            else
            {
                    sfxList[i].Play();
            }
        }
    }

    private AudioAsset GetEmptyAudioAssetFromSFXList()
    {
        AudioAsset au = null;
        if (au == null)
        {
            au = CreateAudioAssetByPool(mono.gameObject, false,  AudioSourceType.SFX);
            sfxList.Add(au);
        }
        return au;
    }

    private List<AudioAsset> clearList = new List<AudioAsset>();
    public void ClearMoreAudioAsset()
    {
        for (int i = 0; i < sfxList.Count; i++)
        {
            sfxList[i].CheckState();
            if (sfxList[i].PlayState == AudioPlayState.Stop)
            {
                clearList.Add(sfxList[i]);
            }
        }

        for (int i = 0; i < clearList.Count; i++)
        {
            DestroyAudioAssetByPool(clearList[i]);
            sfxList.Remove(clearList[i]);
        }
        clearList.Clear();

    }

    private Dictionary<string, float> SfxPlaying = new Dictionary<string, float>( 30 );
    public void UpdateLimit () {
        foreach ( var key in SfxPlaying.Keys.ToList () ) {
            SfxPlaying[ key ] -= Time.deltaTime;
        }
    }
}
