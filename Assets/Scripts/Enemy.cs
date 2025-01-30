using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float minDirectionChangeRate = 1.0f;
    [SerializeField] private float maxDirectionChangeRate = 5.0f;

    private Beyblade bb;

    private float h = 0f;
    private float v = 0f;

    void Start()
    {
        bb = GetComponent<Beyblade>();

        StartCoroutine(GetNewDirection());
    }

    private IEnumerator GetNewDirection()
    {
        while (true)
        {
            Debug.Log("Direction changed");

            h = Random.Range(-1f, 1f);
            v = Random.Range(-1f, 1f);

            float wait = Random.Range(minDirectionChangeRate, maxDirectionChangeRate);
            yield return new WaitForSeconds(wait);
        }
    }

    void FixedUpdate()
    {
        var hSpeed = h * Vector3.right * moveSpeed * Time.deltaTime;
        var vSpeed = v * Vector3.forward * moveSpeed * Time.deltaTime;

        bb.Move(hSpeed, vSpeed);
    }
}
