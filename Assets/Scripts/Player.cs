using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float initialForce = 20f;

    private bool go = false;
    private Rigidbody rb;
    private Vector3 spinAxis;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        spinAxis = transform.up;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.useGravity = true;
            rb.AddForce(transform.forward * initialForce, ForceMode.Impulse);
            rb.AddTorque(spinAxis * 720, ForceMode.Impulse);

            go = true;
        }
    }

    private void FixedUpdate()
    {
        if (go)
        {
            rb.AddForce(rb.angularVelocity.magnitude * transform.right, ForceMode.Force);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 normals = Vector3.zero;
        int count = collision.contactCount;

        for(int i = 0; i < count; i++)
        {
            normals += collision.GetContact(i).normal;
        }

        Vector3 normal = normals / count;

        if (collision.transform.tag.Equals("Player"))
        {
            Vector3 force = collision.impulse;

            float selfVelocity = Vector3.Dot(rb.velocity, normal);
            float otherVelocity = Vector3.Dot(collision.rigidbody.velocity, -normal);
            float totalVelocity = Mathf.Abs(selfVelocity + otherVelocity);

            //Debug.Log(totalVelocity);

            rb.AddForce(normal * totalVelocity * 200.0f, ForceMode.Impulse);
            rb.AddTorque(spinAxis * totalVelocity * 120.0f, ForceMode.Impulse);
        }

        if (collision.transform.tag.Equals("Arena"))
        {
            spinAxis = normal;
        }
    }
}
