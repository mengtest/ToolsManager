using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DWSDKRoot : MonoBehaviour {

    public static DWSDKRoot _instance;
    public static DWSDKRoot Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gb = new GameObject("DWSDKRoot");
                GameObject.DontDestroyOnLoad(gb);
                _instance = EZFunTools.GetOrAddComponent<DWSDKRoot>(gb);

            }
            return _instance;
        }
    }


    public void Init()
    {

    }

    #region ios buy
    void purchaseFailed(string msg)
    {
        Debug.LogError("purchase failed " + msg);
        HandlePurchase("0");
    }
    void purchaseFinished(string msg)
    {
        
    }
    void BuyIAPSuccess(string productAndTranIdentifier)
    {
        HandlePurchase("1," + productAndTranIdentifier);
    }

    void BuyIAPFail()
    {
        HandlePurchase("0");
    }
    void OCLog(string msg)
    {
        Debug.Log("purchase log:" + msg);
    }

    void BuyIAPFailUnlegalCountry()
    {
        Debug.LogError("BuyIAPFailUnlegalCountry ");
    }
    #endregion


    public void HandlePurchase(string cbstr)
    {
        LuaRootSys.Instance.CallLuaFunc("CIAPSys.HandleBuyIAPCB",cbstr);
    }

}




