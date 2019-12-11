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

    // These are accessed by CueStickController
    public bool ready = true;       // If player is ready to play
    public bool ballsMoving = false;

    private float ballsStopTime = 0;
    private bool isPlayer1Turn = true;
    private bool firstBallIn = false;
    private bool player1IsSolid;
    private int numSolidsLeft = 7;
    private int numStripesLeft = 7;

    private float cueStickBallDistZ = 3.485f;
    private float cueStickPosY = 2.4f;
    private float cueStickRotX = 7.3f;
    private float ballsMovingEps = 0.15f;
    private float ballsStopTimeThreshold = 2.5f;
    private float tableHeight = 1.9632f;
    private float tableHeightBelowEps = 0.004f;
    private float tableHeightEps = 0.0001f;
    private Vector3 cueBallNewPos = new Vector3(0, 2.5f, 0);

    // Start is called before the first frame update
    void Start()
    {
        PrintPlayerTurn();
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
                    // Reset cue position
                    Vector3 ballPos = cueBall.transform.position;
                    cueStick.transform.position = new Vector3(ballPos.x, cueStickPosY, ballPos.z - cueStickBallDistZ);
                    cueStick.transform.rotation = Quaternion.Euler(cueStickRotX, 0, 0);
                    cueStick.SetActive(true);

                    // Mark player turn
                    isPlayer1Turn = !isPlayer1Turn;
                    PrintPlayerTurn();
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

                    string resetPlayer = isPlayer1Turn ? "2" : "1";
                    Debug.Log("Foul! Player " + resetPlayer + " can place the white ball.");
                    audioSrc.PlayOneShot(booSound, 1);
                }
                else if (ball == eightBall) {
                    Destroy(ball);

                    // Check if player won or not
                    if (isPlayer1Turn) {
                        if (!firstBallIn) {
                            Debug.Log("Game over! Player 2 won.");
                        }
                        else if (player1IsSolid && numSolidsLeft == 0 || !player1IsSolid && numStripesLeft == 0) {
                            Debug.Log("Game over! Player 1 won.");
                        }
                        else {
                            Debug.Log("Game over! Player 2 won.");
                        }
                    }
                    else {
                        if (!firstBallIn) {
                            Debug.Log("Game over! Player 1 won.");
                        }
                        else if (!player1IsSolid && numSolidsLeft == 0 || player1IsSolid && numStripesLeft == 0) {
                            Debug.Log("Game over! Player 2 won.");
                        }
                        else {
                            Debug.Log("Game over! Player 1 won.");
                        }
                    }

                    // int numBalls = GameObject.FindGameObjectsWithTag("Ball").Length;
                    // numBalls -= cueBall.activeSelf ? 1 : 0;
                    //
                    // if (numBalls > 0) {
                    //     audioSrc.PlayOneShot(booSound, 1);
                    //     Debug.Log("Game over! You lost :(");
                    // }
                    // else {
                    //     audioSrc.PlayOneShot(applauseSound, 1);
                    //     Debug.Log("Game over! You won :)");
                    // }

                }
                else {      // Regular ball
                    bool isSolid = BallIsSolid(ball);

                    if (isSolid) {
                        numSolidsLeft--;

                        if (firstBallIn) {
                            if (isPlayer1Turn && player1IsSolid || !isPlayer1Turn && !player1IsSolid) {
                                audioSrc.PlayOneShot(applauseSound, 1);
                            }
                            else {
                                audioSrc.PlayOneShot(booSound, 1);
                            }
                        }
                    }
                    else {
                        numStripesLeft--;

                        if (firstBallIn) {
                            if (isPlayer1Turn && !player1IsSolid || !isPlayer1Turn && player1IsSolid) {
                                audioSrc.PlayOneShot(applauseSound, 1);
                            }
                            else {
                                audioSrc.PlayOneShot(booSound, 1);
                            }
                        }
                    }

                    Destroy(ball);

                    if (!firstBallIn) {
                        audioSrc.PlayOneShot(applauseSound, 1);

                        firstBallIn = true;

                        if (isPlayer1Turn) {
                            if (isSolid) {
                                player1IsSolid = true;
                                Debug.Log("Player 1 is solid.");
                            }
                            else {
                                player1IsSolid = false;
                                Debug.Log("Player 1 is stripe.");
                            }
                        }
                        else {
                            if (isSolid) {
                                player1IsSolid = false;
                                Debug.Log("Player 2 is solid.");
                            }
                            else {
                                player1IsSolid = true;
                                Debug.Log("Player 2 is stripe.");
                            }
                        }
                    }
                }
            }
        }
    }

    private void PrintPlayerTurn() {
        string playerNum = isPlayer1Turn ? "1" : "2";
        string message = "It's Player " + playerNum + "'s turn!";

        if (firstBallIn) {
            if (isPlayer1Turn && player1IsSolid || !isPlayer1Turn && !player1IsSolid) {
                message += " You're solid.";
            }
            else {
                message += " You're stripe.";
            }
        }

        Debug.Log(message);
    }

    // Check if ball is in pocket
    bool CheckBallIn(GameObject ball) {
        return ball.transform.position.y < tableHeight - tableHeightBelowEps;
    }

    // Returns true if ball is solid, false if stripe
    bool BallIsSolid(GameObject ball) {
        return int.Parse(ball.name.Split(' ')[1]) < 8;
    }
}
