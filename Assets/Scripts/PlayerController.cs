using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;

    [SerializeField] private float initialForce = 20f;
    [SerializeField] private float initialSpeed = 5f;
    [SerializeField] private float groundAlignSpeed = 90.0f;

    [SerializeField] private float blastRadius = 3f;
    [SerializeField] private float blastForce = 50f;

    [SerializeField] private Transform cam;

    public float totalSize = 5f;

    private bool go = false;
    private Rigidbody rb;
    private Beyblade bb;

    private float h = 0f;
    private float v = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bb = GetComponent<Beyblade>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.useGravity = true;
            //rb.AddForce(transform.forward * initialForce, ForceMode.Impulse);
            //rb.AddTorque(transform.up * 360, ForceMode.Impulse);

            go = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //transform.Rotate(transform.up, 360f * Time.deltaTime);

        //Angle to slope
        RaycastHit hit;
        var grounded = Physics.Raycast(transform.position, -transform.up, out hit, 0.5f, LayerMask.GetMask("Ground"));

        if (grounded)
        {
            var slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            Quaternion forward = Quaternion.LookRotation(transform.forward, transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, slopeRotation * forward, groundAlignSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, blastRadius);
            foreach (Collider col in colliders)
            {
                if (col.gameObject == gameObject)
                    continue;

                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddExplosionForce(blastForce, explosionPos, blastRadius, 3.0F, ForceMode.Impulse);
            }
        }

        Debug.DrawLine(transform.position, transform.position + cam.transform.up * 5f, Color.red);
        Debug.DrawLine(transform.position, transform.position + cam.transform.right * 5f, Color.red);

        /*
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, cam.camDistance + 1.0f))
            {
                Debug.Log(hit.transform.name);
                Debug.Log("hit");
            }
        }
        */
        
    }

    private void FixedUpdate()
    {
        if (go)
        {
            var hSpeed = h * Vector3.right * moveSpeed;
            var vSpeed = v * Vector3.forward * moveSpeed;

            rb.AddForce(hSpeed, ForceMode.Acceleration);
            rb.AddForce(vSpeed, ForceMode.Acceleration);

            rb.AddTorque(transform.up * 30f * totalSize, ForceMode.Acceleration);
        }
    }

    public void SetCam(Transform cam)
    {
        this.cam = cam;
    }
    
}
