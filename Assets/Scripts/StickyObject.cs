using UnityEngine;

public class StickyObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Stickable"))
        {
            collision.transform.parent = transform;
            collision.collider.excludeLayers = LayerMask.GetMask("Ground", "Player");

            collision.gameObject.layer = LayerMask.NameToLayer("StuckObject");
            collision.gameObject.AddComponent<StickyObject>();

            Joint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = collision.rigidbody;
        }
    }
}
