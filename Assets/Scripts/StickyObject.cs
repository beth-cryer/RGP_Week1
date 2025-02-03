using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class StickyObject : MonoBehaviour
{
    [SerializeField] Transform stickParent;

    public PlayerController player;
    public Enemy enemy;
    private Beyblade bb;

    public Joint parentJoint;
    private CinemachineTargetGroup targetGroup;
    private Rigidbody rb;

    private Collider col;

    private void Awake()
    {
        //Initial check for if this StickyObject is a Player or Enemy
        GetOwnerBeyblade(GetComponent<PlayerController>(), GetComponent<Enemy>()); //if not, then collision code will pass this instead

        targetGroup = GameObject.FindGameObjectWithTag("TargetGroup").GetComponent<CinemachineTargetGroup>();

        rb = GetComponent<Rigidbody>();

        if (stickParent == null)
        {
            stickParent = transform;
        }

        col = GetComponent<Collider>();
        ColliderCooldown(0.2f);
    }

    public void ColliderCooldown(float secs)
    {
        col.enabled = false;
        StartCoroutine(Cooldown(secs));
    }

    private IEnumerator Cooldown(float secs)
    {
        yield return new WaitForSeconds(secs);
        col.enabled = true;
    }

    public void GetOwnerBeyblade(PlayerController ownerPlayer, Enemy ownerEnemy)
    {
        if (player == null) player = ownerPlayer;
        if (enemy == null) enemy = ownerEnemy;

        bb = player ? player.GetComponent<Beyblade>() : (enemy ? enemy.GetComponent<Beyblade>() : null);
    }

    //Set collided stickyobject's parent to this
    //Set stickyobject to stuck and attach to parent with joint
    public void StickToParent(Transform newParent, Transform jointTarget)
    {
        //if (newParent) transform.parent = newParent;

        gameObject.layer = LayerMask.NameToLayer(player ? "StuckObject" : "EnemyStuckObject");

        rb.useGravity = false;

        Joint joint = jointTarget.AddComponent<FixedJoint>();
        joint.connectedBody = rb;

        parentJoint = joint;
    }

    //Parent touched stickyobject to this
    //Unstick component and remove StickObject and parent Joint components
    public void UnstickFromParent(Transform parent)
    {

        //transform.SetParent(null, false);
        rb.useGravity = true;
        gameObject.layer = 0;
        Destroy(parent.GetComponent<FixedJoint>());
    }

    //Add self to camera target group and add own radius to player view radius
    public void AddToCam(float radius)
    {
        targetGroup.AddMember(transform, 1, 1);

        float playerRadius = radius / 4;

        if (GetComponent<PlayerController>() != null)
        {
            targetGroup.m_Targets[0].radius = Mathf.Clamp(radius, 2.5f, radius);
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

        float playerRadius = transform.localScale.x / 4;

        float newPlayerRadius = targetGroup.m_Targets[0].radius - playerRadius;
        targetGroup.m_Targets[0].radius = newPlayerRadius > 2.5f ? newPlayerRadius : 2.5f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Stickable"))
        {
            StickyObject stickyObject = collision.transform.GetComponent<StickyObject>();
            if (stickyObject != null) return;

            float size = collision.transform.localScale.magnitude;

            float width = (collision.transform.localScale.x + collision.transform.localScale.y) / 2;
            float radius = width / 2;

            float requiredStickSize = size * 1.75f;

            //If colliding object is bigger than player totalSize, don't stick
            if (bb.TotalSize < requiredStickSize)
            {
                Debug.Log("Too small! " + bb.TotalSize + " < " + requiredStickSize);

                SoundController.Instance.PlayCollide();

                return;

                /*
                if (GetComponent<PlayerController>() != null)
                    return;

                //If object is already sticky and attached to an enemy, unstick it from the enemy
                
                if (stickyObject && stickyObject.enemy)
                {
                    stickyObject.UnstickFromParent(stickyObject.transform.parent);
                }

                //RemoveFromCam();
                //UnstickFromParent(transform.parent);

                //player.totalSize -= transform.localScale.magnitude;
                
                Vector3 scale = transform.localScale;
                Vector3 colScale = collision.transform.localScale;
                collision.transform.localScale = new Vector3(colScale.x - scale.x, colScale.y - scale.y, colScale.z - scale.z);

                Destroy(this);
                */
            }

            //If player totalSize beats colliding object, add it to the katamari
            Debug.Log("THE BEYBLADE GROWS. " + bb.TotalSize + " > " + requiredStickSize);

            SoundController.Instance.PlayStick();

            //If objext already sticky and attached to enemy,
            if (stickyObject && stickyObject.enemy)
            {
                stickyObject.enemy.transform.parent = stickParent; //parent enemy to this instead of parenting the colliding object
                stickyObject.StickToParent(null, stickParent);     //then stick colliding object to this with a joint
            }
            else
            {
                StickyObject targetStickyObject = collision.gameObject.AddComponent<StickyObject>();
                targetStickyObject.GetOwnerBeyblade(player, enemy);

                targetStickyObject.AddToCam(radius);
                targetStickyObject.StickToParent(stickParent, stickParent);
                targetStickyObject.ColliderCooldown(0.2f);
            }

            bb.TotalSize += width;
        }
    }
}
