using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class FriendView : MonoBehaviour
{
    public int Id;
    [SerializeField]
    private Text txtName;
    [SerializeField]
    private Text txtState;
    [SerializeField]
    private Image imgBg;
    /// <summary>
    /// 更新显示
    /// </summary>
    public void InitView(int id, string name,bool isOnLine)
    {
        this.Id = id;
        txtName.text = name;
        string state = isOnLine ? "在线" : "离线";
        txtState.text = "状态" + state;
        imgBg.color = isOnLine ? Color.green : Color.red;
    }

    public void UpdateView( bool isOnLine)
    {
        string state = isOnLine ? "在线" : "离线";
        txtState.text = "状态" + state;
        imgBg.color = isOnLine ? Color.green : Color.red;
    }
}