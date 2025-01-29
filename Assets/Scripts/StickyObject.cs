using Cinemachine;
using UnityEngine;

public class StickyObject : MonoBehaviour
{
    [SerializeField] Transform stickParent;

    private CinemachineTargetGroup targetGroup;

    private void Start()
    {
        targetGroup = GameObject.FindGameObjectWithTag("TargetGroup").GetComponent<CinemachineTargetGroup>();

        if (stickParent == null)
        {
            stickParent = transform;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Stickable"))
        {
            float radius = collision.transform.localScale.magnitude / 2;
            targetGroup.AddMember(collision.transform, 1, radius);

            float playerRadius = radius / 2;
            if (GetComponent<PlayerController>() != null)
            {
                targetGroup.m_Targets[0].radius = Mathf.Clamp(radius, 5f, radius);
            }
            else
            {
                targetGroup.m_Targets[0].radius += playerRadius;
            }

            collision.transform.parent = stickParent;
            collision.collider.excludeLayers = LayerMask.GetMask("Ground", "Player");

            collision.gameObject.layer = LayerMask.NameToLayer("StuckObject");
            collision.gameObject.AddComponent<StickyObject>();

            Joint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = collision.rigidbody;

        }
    }
}
