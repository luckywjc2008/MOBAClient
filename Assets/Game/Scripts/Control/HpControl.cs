﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpControl : MonoBehaviour
{
    /// <summary>
    /// 血条
    /// </summary>
    [SerializeField]
    private Slider barHp;
    /// <summary>
    /// 控制颜色
    /// </summary>
    [SerializeField]
    private Image imgFill;
    /// <summary>
    /// 设置颜色
    /// </summary>
    public void setColor(bool friend)
    {
        imgFill.color = friend ? Color.green : Color.red;
    }
    /// <summary>
    /// 设置血量
    /// </summary>
    /// <param name="value"></param>
    public void SetHp(float value)
    {
        barHp.value = value;
    }

    void LateUpdate()
    {
        //血条时刻面向相机
        //transform.LookAt(Camera.main.transform);
        transform.forward = Camera.main.transform.forward;
    }
}
