using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class StickyObject : MonoBehaviour
{
    [SerializeField] Transform stickParent;

    private PlayerController player;
    private CinemachineTargetGroup targetGroup;
    private Rigidbody rb;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        targetGroup = GameObject.FindGameObjectWithTag("TargetGroup").GetComponent<CinemachineTargetGroup>();

        rb = GetComponent<Rigidbody>();

        if (stickParent == null)
        {
            stickParent = transform;
        }
    }

    //Set collided stickyobject's parent to this
    //Set stickyobject to stuck and attach to parent with joint
    public void StickToParent(Transform newParent)
    {
        transform.parent = newParent;

        gameObject.layer = LayerMask.NameToLayer("StuckObject");

        Joint joint = newParent.AddComponent<FixedJoint>();
        joint.connectedBody = rb;
    }

    //Parent touched stickyobject to this
    //Unstick component and remove StickObject and parent Joint components
    public void UnstickFromParent(Transform parent)
    {
        
        transform.SetParent(null, false);
        gameObject.layer = 0;
        Destroy(parent.GetComponent<FixedJoint>());
    }

    //Add self to camera target group and add own radius to player view radius
    public void AddToCam(float radius)
    {
        targetGroup.AddMember(transform, 1, radius);

        float playerRadius = radius / 2;

        if (GetComponent<PlayerController>() != null)
        {
            targetGroup.m_Targets[0].radius = Mathf.Clamp(radius, 5f, radius);
        }
        else
        {
            targetGroup.m_Targets[0].radius += playerRadius;
        }
    }

    //Remove self from camera target group and subtract own radius from player view radius
    public void RemoveFromCam()
    {
        targetGroup.RemoveMember(transform);

        float playerRadius = transform.localScale.magnitude / 2;

        float newPlayerRadius = targetGroup.m_Targets[0].radius - playerRadius;
        targetGroup.m_Targets[0].radius = newPlayerRadius > 5f ? newPlayerRadius : 5f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float size = collision.transform.localScale.magnitude;
        float radius = size / 2;

        if (collision.transform.CompareTag("Stickable"))
        {
            //Only process StickyObject->Stickable object collisions
            if (collision.transform.GetComponent<StickyObject>())
                return;

            //If colliding object is bigger than player totalSize, reduce size of colliding object and then detach this stickyobject from the katamari
            if (player.totalSize <= size)
            {
                Debug.Log("Too small! " + player.totalSize + " < " + size);

                if (GetComponent<PlayerController>() != null)
                {
                    //DIE INSTANTLY
                    return;
                }

                //RemoveFromCam();
                //UnstickFromParent(transform.parent);

                //player.totalSize -= transform.localScale.magnitude;

                Vector3 scale = transform.localScale;
                Vector3 colScale = collision.transform.localScale;
                collision.transform.localScale = new Vector3(colScale.x - scale.x, colScale.y - scale.y, colScale.z - scale.z);

                //Destroy(this);
                return;
            }

            //If player totalSize beats colliding object, add it to the katamari
            Debug.Log("Another for the collection....");

            StickyObject targetStickyObject = collision.gameObject.AddComponent<StickyObject>();

            targetStickyObject.AddToCam(radius);
            targetStickyObject.StickToParent(stickParent);

            player.totalSize += radius;
        }
    }
}
