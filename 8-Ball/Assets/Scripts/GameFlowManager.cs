using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public GameObject cueBall;
    public GameObject eightBall;
    public GameObject cueStick;
    public AudioSource audioSrc;
    public AudioClip booSound;
    public AudioClip applauseSound;

    public bool ready = true;       // If player is ready to play
    public bool ballsMoving = false;
    private float ballsStopTime = 0;

    private float ballsMovingEps = 0.15f;
    private float ballsStopTimeThreshold = 2.5f;
    private float tableHeight = 1.9632f;
    private float tableHeightBelowEps = 0.004f;
    private float tableHeightEps = 0.003f;
    private Vector3 cueBallNewPos = new Vector3(0, 2.5f, 0);

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!ready) {
            ClearBallsIn();

            ballsStopTime += Time.deltaTime;

            // Check if all balls stopped moving
            if (ballsMoving) {
                ballsMoving = CheckBallsMoving();
                ballsStopTime = 0;
            }
            else if (!ballsMoving) {
                // If balls haven't moved for 3 seconds
                if (ballsStopTime > ballsStopTimeThreshold) {
                    // Debug.Log("Balls stopped moving");

                    // Ensure balls stop
                    foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball")) {
                        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
                        ballRb.velocity = Vector3.zero;
                        ballRb.angularVelocity = Vector3.zero;
                    }

                    ready = true;
                }
                // Check if balls started moving again
                else {
                    if (CheckBallsMoving()) {
                        ballsMoving = true;
                        ballsStopTime = 0;
                    }
                }
            }
        }
        // If game is ready
        else {
            if (!cueStick.activeSelf) {
                // Reset cue position if cue ball is ready
                if (cueBall.activeSelf && !cueBall.GetComponent<Rigidbody>().isKinematic) {
                    Vector3 ballPos = cueBall.transform.position;
                    cueStick.transform.position = new Vector3(ballPos.x, 2.2931f, ballPos.z - 3.485f);
                    cueStick.transform.rotation = Quaternion.Euler(5.533f, 0, 0);
                    cueStick.SetActive(true);
                }
                else if (!cueBall.activeSelf){
                    // Move cue ball if foul
                    cueBall.transform.position = cueBallNewPos;
                    cueBall.GetComponent<Rigidbody>().isKinematic = true;
                    cueBall.SetActive(true);
                }
            }
        }
    }

    bool CheckBallsMoving() {
        bool ballsFast = false;
        foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball")) {
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb.velocity.magnitude > ballsMovingEps
                || ball.transform.position.y > tableHeight + tableHeightEps) {
                ballsFast = true;
                break;
            }
            else {
                ballRb.velocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }
        }

        return ballsFast;
    }

    // Remove balls in pockets from scene
    void ClearBallsIn() {
        foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball")) {
            if (CheckBallIn(ball)) {
                if (ball == cueBall) {
                    ball.SetActive(false);
                    audioSrc.PlayOneShot(booSound, 1);
                }
                else if (ball == eightBall) {
                    Destroy(ball);

                    // Check if player won or not
                    int numBalls = GameObject.FindGameObjectsWithTag("Ball").Length;
                    numBalls -= cueBall.activeSelf ? 1 : 0;

                    if (numBalls > 0) {
                        audioSrc.PlayOneShot(booSound, 1);
                        Debug.Log("Game over! You lost :(");
                    }
                    else {
                        audioSrc.PlayOneShot(applauseSound, 1);
                        Debug.Log("Game over! You won :)");
                    }

                }
                else {
                    Destroy(ball);
                    audioSrc.PlayOneShot(applauseSound, 1);
                }
            }
        }
    }

    // Check if ball is in pocket
    bool CheckBallIn(GameObject ball) {
        return ball.transform.position.y < tableHeight - tableHeightBelowEps;
    }
}
