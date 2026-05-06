using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(v, 0, -h).normalized;

        rb.linearVelocity = new Vector3(
            move.x * speed,
            rb.linearVelocity.y,
            move.z * speed
        );
    }
}