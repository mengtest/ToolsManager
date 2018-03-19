
using UnityEngine;


[DisallowMultipleComponent]
public class MonoBehaviourEx : MonoBehaviour
{
    bool IsAwake = false;


    public void Awake()
    {
        if (!IsAwake)
        {
            IsAwake = true;
            DoAwake();
        }
    }

    protected virtual void DoAwake()
    {
    }
}
