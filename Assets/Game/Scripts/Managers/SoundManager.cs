using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 声音管理类
/// </summary>
public class SoundManager : Singleton<SoundManager>
{
    /// <summary>
    /// 用来播放背景音乐（循环）
    /// </summary>
    [SerializeField]
    private AudioSource bgmAudioSource;
    /// <summary>
    /// 特效音乐
    /// </summary>
    [SerializeField]
    private AudioSource effectAudioSource;
    /// <summary>
    /// 等待播放队列
    /// </summary>
    private Queue<AudioClip> accEffectQue = new Queue<AudioClip>();

    void Start()
    {
        bgmAudioSource.loop = true;
        bgmAudioSource.playOnAwake = true;

        effectAudioSource.loop = false;
        effectAudioSource.playOnAwake = false;
    }


    #region 背景音乐

    /// <summary>
    /// 背景音乐音量
    /// </summary>
    /// <returns></returns>
    public float BGVolume
    {
        get { return bgmAudioSource.volume; }
        set { bgmAudioSource.volume = value; }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="clip"></param>
    public void PlayBgMusic(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }

    /// <summary>
    /// 停止背景音乐的播放
    /// </summary>
    public void StopBgMusic()
    {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = null;
    }

    /// <summary>
    /// 停止音效音乐的播放
    /// </summary>
    public void StopEffectMusic()
    {
        effectAudioSource.Stop();
        effectAudioSource.clip = null;
    }


    #endregion

    #region 特效音乐
    /// <summary>
    /// 播放特效音乐
    /// </summary>
    /// <param name="clip"></param>
    public void PlayEffectMusic(AudioClip clip)
    {
        if (clip == null)
        {
           return;
        }
        //accEffectQue.Enqueue(clip);
        effectAudioSource.clip = clip;
        effectAudioSource.Play();
    }

    //void Update()
    //{
    //    //不再播放状态，并且等待播放的队列数量大于0
    //    if (!effectAudioSource.isPlaying && accEffectQue.Count > 0)
    //    {
    //        //先出队头文件
    //        AudioClip ac = accEffectQue.Dequeue();
    //        //开始播放
    //        effectAudioSource.clip = ac;
    //        effectAudioSource.Play();
    //    }
    //}

    #endregion
}
