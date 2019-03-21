﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏初始化脚本
/// </summary>
public class GameInit : MonoBehaviour
{
    void Start()
    {
        //加载登录UI
        UIManager.Instance.ShowUIPanel(UIDefind.UIAccount);
    }

}
