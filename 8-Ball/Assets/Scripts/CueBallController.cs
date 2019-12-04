using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueBallController : MonoBehaviour
{
    private Rigidbody rb;

    private float tableHeight = 1.9632f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move cue ball if foul
        if (rb.isKinematic) {
            // Get user input (arrow keys) to move cue ball
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            transform.Translate(new Vector3(1, 0, 0) * horizontalInput * Time.deltaTime, Space.World);
            transform.Translate(new Vector3(0, 0, 1) * verticalInput * Time.deltaTime, Space.World);

            // Release with space key
            if (Input.GetKeyDown(KeyCode.Space)) {
                transform.position = new Vector3(transform.position.x, tableHeight, transform.position.z);
                rb.isKinematic = false;
            }
        }
    }
}
