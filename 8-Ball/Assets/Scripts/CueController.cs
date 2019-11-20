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

    private bool ballsMoving = false;
    private float ballsMovingEps = 0.1f;

    private bool cueHit = false;
    private float timeSinceHit;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.IgnoreCollision(table.GetComponent<Collider>(), GetComponent<Collider>());
    }

    // Update is called once per frame
    void Update()
    {
        // Check if all balls stopped moving
        if (ballsMoving) {
            ballsMoving = false;
            foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball")) {
                Rigidbody ballRb = ball.GetComponent<Rigidbody>();
                if (ballRb.velocity.magnitude > ballsMovingEps) {
                    ballsMoving = true;
                    break;
                }
                else {
                    ballRb.velocity = Vector3.zero;
                    ballRb.angularVelocity = Vector3.zero;
                }
            }

            // Reset cue if balls stopped moving
            if (!ballsMoving) {
                // Reset position
                Debug.Log("Balls stopped moving");
                Vector3 ballPos = cueBall.transform.position;
                transform.position = new Vector3(ballPos.x, 2.2931f, ballPos.z - 3.485f);
                transform.rotation = Quaternion.Euler(5.533f, 0, 0);
            }
        }

        // Get user input to rotate cue around cue ball
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

        // Set ballsMoving after 3 seconds
        timeSinceHit += Time.deltaTime;
        if (cueHit && timeSinceHit > 3) {
            ballsMoving = true;
        }
    }

    // Stop cue stick when it hits cue ball and marks balls as moving
    private void OnCollisionEnter(Collision collision) {
        rb.isKinematic = true;
        shooting = false;
        cueHit = true;
        timeSinceHit = 0;
    }
}
