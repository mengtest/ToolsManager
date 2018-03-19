//     File      : AutoDestory.cs
//     brief     : ×Ô¶¯Ïú»Ù 
//     author    : jianing
//     version   : 1.0
//     date      : 2017/10/26
//     copyright : Copyright 2017 DW Inc.
// **************************************************************/


using UnityEngine;
using System.Collections;

public class AutoDestory : MonoBehaviour {

    public float AutoDestoryTime = 2;
    private float tickTime = 0;

    void Start() 
    {
        tickTime = 0;
    }

    void OnEnable()
    {
        tickTime = 0;
    }

    void Update()
    {
        tickTime+= Time.deltaTime;
        if (tickTime > AutoDestoryTime && gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }
}
