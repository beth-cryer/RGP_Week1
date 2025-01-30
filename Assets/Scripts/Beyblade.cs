using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beyblade : MonoBehaviour
{
    public Vector3 spinAxis { get; private set; }

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        spinAxis = transform.up;
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + spinAxis.normalized * 5f, Color.red);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            OnCollidePlayer(collision);
        }

        if (collision.transform.CompareTag("Arena"))
        {
            OnCollideArena(collision);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Arena"))
        {
            OnCollideArena(collision);
        }
    }

    private Vector3 AvgNormal(Collision collision)
    {
        Vector3 normals = Vector3.zero;
        int count = collision.contactCount;

        for (int i = 0; i < count; i++)
        {
            normals += collision.GetContact(i).normal;
        }

        return normals / count;
    }

    private void OnCollidePlayer(Collision collision)
    {
        Vector3 normal = AvgNormal(collision);
        Vector3 force = collision.impulse;

        float selfVelocity = Vector3.Dot(rb.velocity, normal);
        float otherVelocity = Vector3.Dot(collision.rigidbody.velocity, -normal);
        float totalVelocity = Mathf.Abs(selfVelocity + otherVelocity);

        float angularSpeed = rb.angularVelocity.magnitude;

        rb.AddForce(normal * totalVelocity * angularSpeed * 10f, ForceMode.Impulse);
        rb.AddTorque(transform.up * -(totalVelocity * 40f), ForceMode.Impulse);
    }

    private void OnCollideArena(Collision collision)
    {
        Vector3 normal = AvgNormal(collision);
        spinAxis = normal;
    }
}
