using UnityEngine;
using System.Collections;

public class ArtFollowCamera : MonoBehaviour
{
    private const string CameraColliderTag = "CameraCollider";

    private GameObject target;
    public float targetFeetHeight = 0.2f;
    public float targetHeadHeight = 1.5f;

    public float MaxDistance = 12f;
    public float MinDistance = 2f;
    public float XRotationSpeed = 0.15f;
    public float YRotationSpeed = 0.15f;
    public float YPushRotationSpeed = 0.07f;

    public float MaxAngle = 75f;
    public float PushStartAngle = 15f;
    public float MinAngle = 5f;

    public float WheelScrollSpeed = 1.5f;

    private Vector3 m_OriginPosition;
    private float m_CurrentDistance = 8f;
    private Vector3 m_OriginEulerAngles = new Vector3(41, 224, 0);

    private float m_CurMaxDistance;

    public GameObject dragColliderOBJ;
    private float m_targetCenterHight = 0.5f;
    private int m_occlusionLayer;

    // Use this for initialization
    void Start()
    {
        m_OriginEulerAngles = new Vector3(41, 224, 0);
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }

        m_CurMaxDistance = m_CurrentDistance;
        m_occlusionLayer = LayerMask.GetMask("Ground");

        UIEventListener.Get(dragColliderOBJ).onDrag = OnDrag;
    }

    void Update()
    {
        float deltaZ = Input.GetAxis("Mouse ScrollWheel");
        if (deltaZ != 0)
        {
            m_CurMaxDistance -= deltaZ * WheelScrollSpeed;
            m_CurMaxDistance = Mathf.Clamp(m_CurMaxDistance, MinDistance, MaxDistance);
            m_CurrentDistance -= deltaZ * WheelScrollSpeed;
            m_CurrentDistance = Mathf.Clamp(m_CurrentDistance, MinDistance, m_CurMaxDistance);
        }
    }

    void LateUpdate()
    {
        //         float x = Mathf.LerpAngle(transform.eulerAngles.x, m_OriginEulerAngles.x, Time.deltaTime * 4f);
        //         float y = Mathf.LerpAngle(transform.eulerAngles.y, m_OriginEulerAngles.y, Time.deltaTime * 4f);
        //         transform.eulerAngles = new Vector3(x, y, transform.eulerAngles.z);
        transform.eulerAngles = m_OriginEulerAngles;
        m_targetCenterHight = Mathf.Lerp(targetFeetHeight, targetHeadHeight, (MaxDistance - m_CurrentDistance) / (MaxDistance - MinDistance));
        Vector3 viewpoint = target.transform.position + target.transform.up * m_targetCenterHight;
        HandleRaysHitPush(viewpoint);
        transform.position = m_OriginPosition;
    }

    void OnDrag(GameObject gb, Vector2 rotation)
    {
        float toDistance = m_CurMaxDistance;

        float toY = m_OriginEulerAngles.y + rotation.x * XRotationSpeed;

        float deltaX = 0;

        if (m_OriginEulerAngles.x > PushStartAngle)
        {
            deltaX -= rotation.y * YRotationSpeed;
        }
        else
        {
            deltaX -= rotation.y * YPushRotationSpeed;
        }

        float toX = m_OriginEulerAngles.x + deltaX;

        if (m_OriginEulerAngles.x < PushStartAngle)
        {
            float deltaDistance = (m_CurMaxDistance - MinDistance) * deltaX / (PushStartAngle - MinAngle);
            toDistance = m_CurrentDistance + deltaDistance;
        }

        m_OriginEulerAngles.y = toY;
        m_OriginEulerAngles.x = Mathf.Clamp(toX, MinAngle, MaxAngle);
        m_CurrentDistance = Mathf.Clamp(toDistance, MinDistance, m_CurMaxDistance);
    }

    #region ÕÚµ²ÍÆÒÆ
    private Ray m_occlusionTestRay = new Ray();
    private RaycastHit[] m_occlusionBuffer = new RaycastHit[3];
    private void HandleRaysHitPush(Vector3 viewPoint)
    {
        m_OriginPosition = viewPoint - m_CurrentDistance * transform.forward;

        Vector3 startPos = viewPoint;
        Vector3 endPos = m_OriginPosition;
        Vector3 dir = endPos - startPos;
        m_occlusionTestRay.origin = startPos;
        m_occlusionTestRay.direction = dir;

        
        float hitDistance = -1;
        int hitCount = Physics.RaycastNonAlloc(m_occlusionTestRay, m_occlusionBuffer, m_CurrentDistance, m_occlusionLayer);
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = m_occlusionBuffer[i];
                if (hit.collider.CompareTag(CameraColliderTag))
                {
                    if (hit.distance <= MinDistance)
                    {
                        hitDistance = MinDistance;
                        m_OriginPosition = m_occlusionTestRay.GetPoint(MinDistance);
                    }
                    else
                    {
                        hitDistance = hit.distance;
                        m_OriginPosition = hit.point;
                    }

                    if (hitDistance > 0)
                    {
                        float occlosionCenterHeight = Mathf.Lerp(targetFeetHeight, targetHeadHeight, (MaxDistance - hitDistance) / (MaxDistance - MinDistance));
                        m_OriginPosition += Vector3.up * (occlosionCenterHeight - m_targetCenterHight);
                    }
                    break;
                }
            }
        }
    }

    #endregion
}
