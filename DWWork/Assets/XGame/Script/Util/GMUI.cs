using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ezfun_resource;


using SheetType = System.Collections.Generic.Dictionary<object, global::ProtoBuf.IExtensible>;
public class GMUI : WindowRoot
{
    public UILabel cmdLabel;
    public UILabel argsLabel;
    public override void InitWindow(bool open = true, int state = 0)
    {
        InitCamera(open);
        base.InitWindow(open, state);

        if (open)
        {
            InitWindowDetail();
        }
        /*		else
		{
			EZFunWindowMgr.Instance.ClearWindow();
		}
*/
    }

    public override void ClearEventHandler()
    {
        base.ClearEventHandler();
    }



    protected override void CreateWindow()
    {
        base.CreateWindow();
    }


    protected override void HandleWidgetClick(GameObject gb)
    {
        if (!CheckBtnCanClick(gb))
        {
            return;
        }

        base.HandleWidgetClick(gb);

        string click_name = gb.name;

        switch (click_name)
        {
            case "btn_reset":
                Debug.Log("btn_change_team clicked");
                cmdLabel.text = "";
                argsLabel.text = "";
                break;
            case "btn_send":
                Debug.Log("btn_fight clicked");
                SendMessage();
                break;

            case "back_btn":
                InitWindow(false);
                break;
           
        }

    }

    void HandleGoToOnlineFuben()
    {
      
    }

    void InitWindowDetail()
    {
        cmdLabel = GetTrans("cmd_label").GetComponent<UILabel>();
        argsLabel = GetTrans("args_label").GetComponent<UILabel>();
        //if (AutoAttackSys.Instance.m_isUseNavAgent)
        //{
        //    SetLabel(GetTrans(GetTrans("changeFindWayMethod"), "Label"), "custom");
        //}
        //else
        {
            SetLabel(GetTrans(GetTrans("changeFindWayMethod"), "Label"), "navmesh");
        }

        SetSprite(GetTrans("btn_attr_log"), /*CAttributeDataMgr.m_logFlag ? */"Ico_SysSet_Light" /*: "Ico_SysSet_Dark"*/);


        //		CAttributeDataMgr.AddSelfAttribute();
        //		Debug.LogError("总" + (CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.Total) - 
        //		                      CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.BuffEffect)));
        //		Debug.LogError("等级" + CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.Original));
        //		Debug.LogError("技能" + 0);
        //		Debug.LogError("副将" + (CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.CardAdvanceAttribute) +
        //		               CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.CardWeaponAttribute)));
        //		Debug.LogError("饰品" + CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.Equip));
        //		Debug.LogError("翅膀+时装" + (CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.Wing) + 
        //		                       CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.Avatar)));
        //		Debug.LogError("情愿" + (CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.CardBroAdd)*CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.Original)));
        //		Debug.LogError("宝石" + CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.GemAdd));
        //		Debug.LogError("阵法" + CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.PhalanxAdd));
        //		Debug.LogError("装备" + CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.WeaponValue));
        //		Debug.LogError("圣兽" + CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.Constellation));
        //		Debug.LogError("宠物" + CPlayer.GetSelf().GetAttrValue(CreatureAttributeType.PhysicalAttack, CreatureAttributeValueType.PetAdd));

    }

    public void SendMessage()
    {
        if (cmdLabel.text.Length <= 0 || argsLabel.text.Length <= 0)
        {
            Debug.LogError("no input");
            return;
        }
      
    }

    // ask fuben info
    //private void sendFuBenInfoReq()
    //{
    //    CNetSys.Instance.SendNetMsg(ezfun.CS_CMD.CS_LOGIC_CMD_PWORLD_DATA, null, (ezfun.SCPackage msg) =>
    //    {
    //        CFuBenSys.Instance.saveNetData(msg.body.pworldDataRsp);

    //    });
    //}

    int findMaxLevelId()
    {
        return 1;
    }
}

