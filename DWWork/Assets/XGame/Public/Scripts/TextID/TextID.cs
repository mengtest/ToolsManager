using System;
using System.Collections.Generic;
using UnityEngine;

 public class TextID : MonoBehaviour
 {
     [HideInInspector]
     [SerializeField]
     public string mText = "";

     [HideInInspector]
     [SerializeField]
     public int mTextID = -100000000;//无效

 }
