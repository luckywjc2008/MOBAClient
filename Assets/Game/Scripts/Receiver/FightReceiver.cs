using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using JetBrains.Annotations;
using LitJson;
using MobaCommon.Code;
using MobaCommon.Config;
using MobaCommon.Dto;
using UnityEngine;

public class FightReceiver : MonoBehaviour,IReceiver
{
    #region 缓存数据
    private HeroModel[] Heros;
    private BuildModel[] Builds;
    [Header("队伍1")]
    [SerializeField]
    private Transform team1Parent;
    [SerializeField]
    private Transform[] team1HeroPoints;
    [SerializeField]
    private GameObject[] team1Builds;

    [Header("队伍2")]
    [SerializeField]
    private Transform team2Parent;
    [SerializeField]
    private Transform[] team2HeroPoints;
    [SerializeField]
    private GameObject[] team2Builds;

    private Dictionary<int,BaseControl> idControlDict = new Dictionary<int, BaseControl>();

    [Header("试图")]
    [SerializeField]
    private FightView view;

    [Header("掉血数字")]
    [SerializeField]
    private bl_HUDText HUDText;

    #endregion


    public void OnReceive(byte subCode, OperationResponse response)
    {
        switch (subCode)
        {
            case OpFight.Enter:
                break;
            case OpFight.GetInfo:
                OnGetInfo(JsonMapper.ToObject<HeroModel[]>(response[0].ToString()),
                    JsonMapper.ToObject<BuildModel[]>(response[1].ToString()));
                break;
            case OpFight.Walk:
                OnWalk((int)response[0], (float)response[1], (float)response[2], (float)response[3]);
                break;
            case OpFight.Skill:
                OnSkill(response);
                break;
            case OpFight.Damage:
                OnDamage(response);
                break;
            case OpFight.Buy:
                OnBuy(response);
                break;
            case OpFight.SkillUp:
                OnSkillUp(response);
                break;
            case OpFight.UpdateModel:
                OnUpdateModel(response);
                break;
            case OpFight.Resurge:
                OnResurge(response);
                break;
            case OpFight.GameOver:
                OnGameOver(response);
                break;
            case OpFight.Dog:
                OnDog(response);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 创建小兵
    /// </summary>
    /// <param name="response"></param>
    private void OnDog(OperationResponse response)
    {
        DogModel[] dogs = JsonMapper.ToObject<DogModel[]>(response[0].ToString());
        for (int i = 0; i < dogs.Length; i++)
        {
            DogModel dog = dogs[i];
            GameObject go = PoolManager.Instance.GetObject("Dog");
            DogControl con = go.GetComponent<DogControl>();
            int myTeam = this.GetTeam(this.Heros, GameData.Player.id);
            //初始化小兵控制器
            con.Init(dog,dog.Team == myTeam);
            con.SetCamp(team1Builds[1].transform,team2Builds[1].transform);

            if (dog.Team == 1)
            {
                con.transform.position = con.camp1.position;
                con.Move(con.camp2.position);
            }
            else
            {
                con.transform.position = con.camp2.position;
                con.Move(con.camp1.position);
            }
            idControlDict.Add(dog.Id, con);
        }
    }

    /// <summary>
    /// 响应游戏结束
    /// </summary>
    /// <param name="response"></param>
    private void OnGameOver(OperationResponse response)
    {
        int winTeam = (int) response[0];
        view.GameOver(GameData.myControl.Model.Team == winTeam);
    }

    /// <summary>
    /// 更新英雄数据
    /// </summary>
    /// <param name="response"></param>
    private void OnUpdateModel(OperationResponse response)
    {
        HeroModel heroModel = JsonMapper.ToObject<HeroModel>(response[0].ToString());
        //获取控制器
        BaseControl con = idControlDict[heroModel.Id];
        //更新模型
        con.Model = heroModel;
        //是自身 就更新视图
        if (con == GameData.myControl)
        {
            view.UpdateView(heroModel);
        }
    }
    /// <summary>
    /// 响应复活
    /// </summary>
    /// <param name="response"></param>
    private void OnResurge(OperationResponse response)
    {
        if (response.ReturnCode == 0)
        {
            OnResurge(JsonMapper.ToObject<HeroModel>(response[0].ToString()));
        }
        else if (response.ReturnCode == 1)
        {
            OnResurge(JsonMapper.ToObject<BuildModel>(response[0].ToString()));
        }
    }
    /// <summary>
    /// 复活
    /// </summary>
    /// <param name="heroModel"></param>
    private void OnResurge(HeroModel heroModel)
    {
        BaseControl con = idControlDict[heroModel.Id];
        con.Model = heroModel;
        //重置状态
        con.ResurgeResponse();
        //重置位置
        if (heroModel.Team == 1)
        {
            con.transform.position = team1HeroPoints[0].position;
        }
        else if (heroModel.Team == 2)
        {
            con.transform.position = team2HeroPoints[0].position;
        }

        //是自身 就更新视图
        if (con == GameData.myControl)
        {
            view.UpdateView(heroModel);
        }
    }
    /// <summary>
    /// 复活
    /// </summary>
    /// <param name="buildModel"></param>
    private void OnResurge(BuildModel buildModel)
    {
        BaseControl con = idControlDict[buildModel.Id];
        con.Model = buildModel;
         //复活
        con.ResurgeResponse();
    }

    private void OnSkillUp(OperationResponse response)
    {
        int playerId = (int) response[0];
        SkillModel data = JsonMapper.ToObject<SkillModel>(response[1].ToString());
        //先获取控制器
        BaseControl con = idControlDict[playerId];
        HeroModel model = con.Model as HeroModel;
        //遍历英雄技能
        for (int i = 0; i < model.Skills.Length; i++)
        { 
            if (model.Skills[i].Id != data.Id)
            {
                  continue;
            }
            model.Skills[i] = data;
            //刷新技能显示
            if (con == GameData.myControl)
            {
               model.SkillPoints--;
               view.UpdateSkills(model);
            }
            break;
        }
    }

    private void OnBuy(OperationResponse response)
    {
        if (response.ReturnCode == 0)
        {
            HeroModel hero = JsonMapper.ToObject<HeroModel>(response[0].ToString());
            int id = hero.Id;
            //更新数据模型
            idControlDict[id].Model = hero;
            BaseControl con = idControlDict[id];
            //自己买装备了
            if (GameData.myControl.Model.Id == id)
            {
                view.UpdateView(hero);
            }
        }
        else
        {
            MessageTip.Instance.Show(response.ReturnCode.ToString());
        }


    }

    /// <summary>
    /// 响应伤害
    /// </summary>
    /// <param name="response"></param>
    private void OnDamage(OperationResponse response)
    {
        List<DamageModel> damages = JsonMapper.ToObject<List<DamageModel>>(response[0].ToString());
        foreach (var item in damages)
        {
            //目标
            int toId = item.toId;
            //获取控制器
            BaseControl con = idControlDict[toId];
            //受伤
            con.Model.CurrHp -= item.damage;
            con.OnHpChange();
            //实例化出来一个掉血数字
            HUDText.NewText("- " + item.damage.ToString(), con.transform, Color.red, 24, 20f, -1f, 2.2f, bl_Guidance.Up);

            //受伤的是自身
            if (con == GameData.myControl)
            {
                //更新视图的显示
                view.UpdateView(con.Model as HeroModel);
                //如果死亡了 就会白屏幕(未作)
            }
            else //别人掉血
            {

            }

            if (item.isDead == true)
            {
                //代表死亡
                con.DeathResponse();
            }
        }
    }

    private void OnSkill(OperationResponse response)
    {
        if (response.ReturnCode == 0)
        {
            OnAttack((int)response[0],(int)response[1]);
        }else if (response.ReturnCode == 1)
        {
            //是技能
            OnSkillAttack((int)response[0], (int)response[1], (int)response[2], (float)response[3], (float)response[4], (float)response[5]);
        }
    }

    /// <summary>
    /// 技能响应
    /// </summary>
    /// <param name="fromId"></param>
    /// <param name="targetId"></param>
    private void OnSkillAttack(int skillId, int fromId, int targetId,float x,float y,float z)
    {
        //获得攻击者
        BaseControl con = idControlDict[fromId];
        //判断技能类型
        if (targetId == -1)
        {
            Vector3 targetPos = new Vector3(x,y,z);
            con.SkillResponse(skillId,null,targetPos);
        }
        else
        {
            //跟踪技能
            //TODO
        }
        //如果是自己释放就刷新ui显示
        if (con == GameData.myControl)
        {
            view.UpdateCoolDown(skillId);
        }
    }
    /// <summary>
    /// 接受普攻响应
    /// </summary>
    /// <param name="fromId"></param>
    /// <param name="targetId"></param>
    private void OnAttack(int fromId,int targetId)
    {
        //释放者控制器
        BaseControl formCon = idControlDict[fromId];
        //被攻击者控制器
        BaseControl TargetCon = idControlDict[targetId];
        //调用攻击方法
        formCon.AttackResponse(TargetCon.transform);
    }

    /// <summary>
    /// 接受到移动响应
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    private void OnWalk(int playerId,float x,float y,float z)
    {
        BaseControl con = idControlDict[playerId];
        con.Move(new Vector3(x,y,z));
    }

    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="team1Hero">队伍1的英雄</param>
    /// <param name="team1Build">队伍1的建筑</param>
    private void OnGetInfo(HeroModel[] heros,BuildModel[] builds)
    {
        //把战斗房间内的模型 先保存到本地
        this.Heros = heros;
        this.Builds = builds;

        int myTeam = this.GetTeam(heros, GameData.Player.id);
        
        //创建游戏物体
        GameObject go = null;

        #region 英雄 
        HeroModel hero;
        for (int i = 0; i < heros.Length; i++)
        {
            hero = heros[i];
            //创建英雄的游戏物体 首先要加载预设
            GameObject prefab = Resources.Load<GameObject>(Paths.RES_HERO + hero.Name);

            if (hero.Team == 1)
            {
                go = Instantiate(prefab, team1HeroPoints[0].position,Quaternion.identity);
                go.transform.SetParent(team1Parent);
            }
            else if (hero.Team == 2)
            {
                go = Instantiate(prefab, team2HeroPoints[0].position, Quaternion.identity);
                go.transform.SetParent(team2Parent);
            }

            //初始化控制器
            BaseControl con = go.GetComponent<BaseControl>();
            con.Init(hero, hero.Team == myTeam);
            //判断这个英雄是不是自己的英雄
            if (hero.Id == GameData.Player.id)
            {
                //保存当前的英雄
                GameData.myControl = con;
                //初始化战斗UI
                view.InitView(hero);
                //聚焦到自己的英雄
                Camera.main.GetComponent<CameraControl>().FocusOn();
            }
            //添加到字典里
            this.idControlDict.Add(hero.Id, con);
        }
        #endregion

        #region 建筑
        for (int i = 0; i < builds.Length; i++)
        {
            BuildModel build = builds[i];
            if (build.Team == 1)
            {
                go = team1Builds[build.TypeId - 1];
                go.SetActive(true);
            }
            else if (build.Team == 2)
            {
                go = team2Builds[build.TypeId - 1];
                go.SetActive(true);
            }
            //初始化控制器
            BaseControl con = go.GetComponent<BaseControl>();
            con.Init(build, build.Team == myTeam);
            //添加到字典里
            idControlDict.Add(build.Id, con);
        }


        #endregion

    }

    private int GetTeam(HeroModel[] heros, int playerId)
    {
        foreach (HeroModel item in heros)
        {
            if (item.Id == playerId)
            {
                return item.Team;
            }
        }
        return -1;
    }
}

