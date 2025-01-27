using UnityEngine;
using UnityEngine.ProBuilder;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float initialForce = 20f;

    [SerializeField]
    private float initialSpeed = 5f;

    [SerializeField] private Transform camPos;
    [SerializeField] private Transform camDir;

    private float currentSpeed = 0f;

    private bool go = false;
    private Rigidbody rb;

    public Vector3 spinAxis { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        spinAxis = transform.up;
        currentSpeed = initialSpeed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.useGravity = true;
            rb.AddForce(transform.forward * initialForce, ForceMode.Impulse);
            rb.AddTorque(transform.up * 720, ForceMode.Impulse);

            go = true;
        }
    }

    private void FixedUpdate()
    {
        if (go)
        {
            //rb.AddForce(transform.forward * currentSpeed, ForceMode.Force);
            Debug.DrawLine(transform.position, transform.position + spinAxis.normalized * 5f, Color.red);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            OnCollidePlayer(collision);
        }

        if (collision.transform.tag.Equals("Arena"))
        {
            OnCollideArena(collision);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag.Equals("Arena"))
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

        //Debug.Log(totalVelocity);

        rb.AddForce(normal * totalVelocity * angularSpeed * 10f, ForceMode.Impulse);
        rb.AddTorque(transform.up * -(totalVelocity * 60f), ForceMode.Impulse);
    }

    private void OnCollideArena(Collision collision)
    {
        Vector3 normal = AvgNormal(collision);
        spinAxis = normal;
    }
}
