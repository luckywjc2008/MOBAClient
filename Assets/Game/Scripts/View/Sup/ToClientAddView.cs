using System;
using MobaCommon.Dto;
using UnityEngine;
using UnityEngine.UI;

public class ToClientAddView : MonoBehaviour
{
    [SerializeField]
    private Text txtInfo;

    public int Id;

    public void UpdateView(PlayerDto dto)
    {
        this.Id = dto.id;
        txtInfo.text = String.Format("姓名:{0}\n等级:{1}\n好友个数:{2}\n逃跑场次:{3}", dto.name,dto.lv,dto.friends.Length,dto.runCount);
    }

}
