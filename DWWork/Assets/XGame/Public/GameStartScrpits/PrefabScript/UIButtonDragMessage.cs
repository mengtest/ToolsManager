using UnityEngine;
[AddComponentMenu("NGUI/Interaction/Button DragMessage")]
public class UIButtonDragMessage : MonoBehaviour
{
    public GameObject target;
    public string functionName;

    public bool sendOnce = true;
    public bool isSend = false;

    void Start()
    {
        sendOnce = true;
    }

    void OnDrag(Vector2 delta) 
    {
        Send(delta);
    }

    void Send(Vector2 delta)
    {
        if (isSend && sendOnce)
            return;
        if (string.IsNullOrEmpty(functionName))
        {
            functionName = "HandleWidgetDrag";
        }
        if (target == null)
        {
            var handle = transform.GetComponentInParent<UIEventHandleProxy>();
            if (handle == null)
            {
                if (functionName == "HandleWidgetDrag")
                {
                    Transform target_trans = transform;
                    while (!target_trans.name.Contains("ui_window") && target_trans.parent != null)
                    {
                        target_trans = target_trans.parent;
                    }
                    if (!target_trans.name.Contains("ui_window"))
                    {
                        DebugEz.LogWarning("[Can not find parent named ui_window]");
                        return;
                    }
                    target = target_trans.gameObject;
                }
                else
                {
                    target = gameObject;
                }
            }
            else
            {
                target = handle.gameObject;
            }
        }
        target.SendMessage(functionName, delta, SendMessageOptions.DontRequireReceiver);
        isSend = true;
    }

    void OnPress(bool isPressed) 
    {
        isSend = false;
    }

}
