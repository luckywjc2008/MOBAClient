using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paths : MonoBehaviour
{
    /// <summary>
    /// ui声音资源路径
    /// </summary>
    public const string RES_SOUND_UI = "Sound/UI/";
    /// <summary>
    /// UI的路径
    /// </summary>
    public const string RES_UI = "UI/";
    /// <summary>
    /// 头像的路径
    /// </summary>
    public const string RES_HEAD = "Head/";
    /// <summary>
    /// 选英雄声音的路径
    /// </summary>
    public const string RES_SOUND_SELECT = "Sound/Select/";
    /// <summary>
    /// 战斗场景声音路径
    /// </summary>
    public const string RES_SOUND_FIGHT = "Sound/Fight/";
    /// <summary>
    /// 英雄预设
    /// </summary>
    public const string RES_HERO = "Hero/";

    /// <summary>
    /// 技能预设
    /// </summary>
    public const string RES_SKILL = "Skill/";

    /// <summary>
    /// 获取UI声音的资源全路径
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetSoundFullName(string name)
    {
        return RES_SOUND_UI + name;
    }
}
