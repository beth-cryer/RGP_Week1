using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] private Beyblade player;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] public float camDistance { get; private set; } = 25.0f;

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        player.GetComponent<PlayerController>().SetCam(this);
    }

    private void Update()
    {
        var pos = player.transform.position;

        transform.position = Vector3.SmoothDamp(transform.position, pos + Vector3.up * camDistance, ref velocity, smoothTime);
        transform.LookAt(pos);
    }
}
