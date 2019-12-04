using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalInput;
    public GameObject cueBall;

    private float rotateAngle = -5;
    private float speed = 0.5f;
    private float force = 2;
    // private Vector3 cueBallNewPos = new Vector3(0, 1.9632f, 0);
    private Vector3 cueBallNewPos = new Vector3(0, 2f, 0);

    private GameFlowManager gameFlowManager;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        gameFlowManager = GameObject.Find("GameFlowManager").GetComponent<GameFlowManager>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Change condition to involving kinematic or change cueballcontroller to getkeyup
        if (gameFlowManager.ready && gameObject.activeSelf) {
            // // Reset cue
            // Vector3 ballPos = cueBall.transform.position;
            // transform.position = new Vector3(ballPos.x, 2.2931f, ballPos.z - 3.485f);
            // transform.rotation = Quaternion.Euler(5.533f, 0, 0);
            // gameObject.SetActive(true);

            // Get user input to rotate cue around cue ball
            horizontalInput = Input.GetAxis("Horizontal");
            transform.RotateAround(cueBall.transform.position, Vector3.up, horizontalInput * rotateAngle);

            // Shoot by holding space key (for strength control) and releasing it
            if (Input.GetKey(KeyCode.Space)) {
                transform.Translate(Vector3.forward * Time.deltaTime * -speed);
            }
            else if (Input.GetKeyUp(KeyCode.Space)) {
                // Make cue stick disappear
                gameObject.SetActive(false);

                // Shoot cue ball
                float dist = Vector3.Distance(transform.position, cueBall.transform.position);
                cueBall.GetComponent<Rigidbody>().AddForce(transform.forward * force * dist, ForceMode.Impulse);
                gameFlowManager.ready = false;
                gameFlowManager.ballsMoving = true;
            }
        }
    }
}
