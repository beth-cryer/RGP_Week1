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
    [SerializeField] private SizeBar sizeBar;
    [SerializeField] private GameObject victoryText;

    private bool go = false;
    private Rigidbody rb;
    private Beyblade bb;

    private float h = 0f;
    private float v = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bb = GetComponent<Beyblade>();

        bb.OnSizeChange += sizeBar.SetSizeBar;
        bb.OnSizeChange += checkVictory;
    }

    private void checkVictory(float size)
    {
        if (size > 500f)
        {
            victoryText.SetActive(true);
        }
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
    }

    private void FixedUpdate()
    {
        if (go)
        {
            var hSpeed = h * Vector3.right * moveSpeed;
            var vSpeed = v * Vector3.forward * moveSpeed;

            bb.Move(hSpeed, vSpeed);
        }
    }
}
