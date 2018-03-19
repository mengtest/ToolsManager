using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
 
public class DwChatRecoredCircel : MonoBehaviour
{
    public float speed;
    public Transform LoadingBar;
    public float currentAmount;

    // Use this for initialization
    void Start()
    {
         currentAmount = 0;
    }
 
   // Update is called once per frame
   void Update()
   {
        //Debug.Log("currentAmount ==== " + currentAmount);
        if (currentAmount < 100)
        {
            currentAmount += speed * Time.deltaTime;
        }
        else
        {
            DWChatTalk.getInstance().DW_RecordAudioStop();
            currentAmount = 0;
            //currentAmount += speed * Time.deltaTime;
        }
        LoadingBar.GetComponent<Image>().fillAmount = currentAmount / 100;
   }

}
