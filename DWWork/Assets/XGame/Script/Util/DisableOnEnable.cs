using UnityEngine;
using System.Collections;

/* 
 * Use this script to ensure useless things stay turned off
 */

public class DisableOnEnable : MonoBehaviour
{
    void OnEnable()
    {
        gameObject.SetActive(false);
    }
}
