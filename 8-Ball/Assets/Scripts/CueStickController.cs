using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueStickController : MonoBehaviour
{
    public float horizontalInput;
    public GameObject cueBall;
    public AudioSource audioSrc;
    public AudioClip shootSound;

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
            // Get user input to rotate cue around cue ball
            horizontalInput = Input.GetAxis("Horizontal");
            transform.RotateAround(cueBall.transform.position, Vector3.up, horizontalInput * rotateAngle);

            // Shoot by holding space key (for strength control) and releasing it
            if (Input.GetKey(KeyCode.Space)) {
                transform.Translate(Vector3.forward * Time.deltaTime * -speed);
            }
            else if (Input.GetKeyUp(KeyCode.Space)) {
                // Play sound effect
                audioSrc.PlayOneShot(shootSound, 1);

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
