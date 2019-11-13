using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueController : MonoBehaviour
{
    public Rigidbody rb;

    public float horizontalInput;
    public GameObject cueBall;
    public GameObject table;

    private float rotateAngle = -5;
    private float speed = 0.5f;
    private float force = 200;
    private bool shooting = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.IgnoreCollision(table.GetComponent<Collider>(), GetComponent<Collider>());
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate cue around cue ball
        horizontalInput = Input.GetAxis("Horizontal");
        transform.RotateAround(cueBall.transform.position, Vector3.up, horizontalInput * rotateAngle);

        // Shoot by holding space key (for strength control) and releasing it
        if (Input.GetKey(KeyCode.Space)) {
            transform.Translate(Vector3.forward * Time.deltaTime * -speed);
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {
            shooting = true;
        }

        // Accelerate cue stick if released
        if (shooting) {
            rb.AddForce(transform.forward * force);
        }
    }

    // Stop cue stick when it hits cue ball
    private void OnCollisionEnter(Collision collision) {
        rb.isKinematic = true;
        shooting = false;
    }
}
