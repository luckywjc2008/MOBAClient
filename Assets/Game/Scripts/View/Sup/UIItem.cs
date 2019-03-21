using System.Collections;
using System.Collections.Generic;
using MobaCommon.Code;
using UnityEngine;

public class UIItem : MonoBehaviour
{
    [SerializeField]
    private int Id;

    public void OnClick()
    {
        //发起购买请求
        PhotonManager.Instance.Request(OpCode.FightCode,OpFight.Buy,this.Id);
    }

}
