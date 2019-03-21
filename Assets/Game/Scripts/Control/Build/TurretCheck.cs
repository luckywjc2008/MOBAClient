using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 炮塔的敌人检查
/// </summary>
public class TurretCheck : MonoBehaviour
{
    /// <summary>
    /// 当前炮塔的队伍
    /// </summary>
    private int team;

    public void SetTeam(int team)
    {
        this.team = team;
    }

    //检测到的敌人列表
    public List<BaseControl> conList = new List<BaseControl>();

    void OnTriggerEnter(Collider other)
    {
        BaseControl con = other.GetComponent<BaseControl>();
        if (con != null && con.Model.Team != this.team)
        {
            conList.Add(con);
        }
    }

    void OnTriggerExit(Collider other)
    {
        BaseControl con = other.GetComponent<BaseControl>();
        if (con != null && conList.Contains(con))
        {
            conList.Remove(con);
        }
    }
}
