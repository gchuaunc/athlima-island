using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinsManager : MonoBehaviour
{
    [SerializeField] private Rigidbody[] pins;
    [SerializeField] private float timeBeforeEndFrame = 3f;
    [SerializeField] private float restVelocityThreshold = 0.1f;
    [SerializeField] private float restAngularVelocityThreshold = 0.1f;
    [SerializeField] private float settleDelay = 0.5f; // time to let pins settle between resetting them and starting new frame
    [SerializeField] private BowlingBlocker bowlingBlocker; // for raising blocker after start of roll / frame
    [SerializeField] private GameObject ballPrefab;

    public static PinsManager Instance;
    public bool[] PinStates { get; private set; } // true = knocked down
    public bool[] PreviousPinStates { get; private set; }

    private List<IPinEndListener> pinListeners;
    private float timeTillEndOfFrame;
    private bool isFrameReady; // frame has started and waiting for the first pin to be knocked down
    private bool isFrameActive; // first pin has knocked down and waiting for timeout
    private (Vector3 position, Quaternion rotation)[] startingPinLocalPositions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogWarning("Warning: more than one PinsManager in the scene. Disabling this one");
            enabled = false;
            return;
        }
        pinListeners = new List<IPinEndListener>();
        startingPinLocalPositions = new (Vector3, Quaternion)[pins.Length];
        PinStates = new bool[pins.Length];
        PreviousPinStates = new bool[pins.Length];
        ResetPinStates();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetTimeToEndOfFrame();
        isFrameActive = false;
        isFrameReady = false;
        int i = 0;
        foreach (Rigidbody pin in pins)
        {
            startingPinLocalPositions[i++] = (pin.transform.localPosition, pin.transform.localRotation);
        }
        StartCoroutine(nameof(DelayedStartGame));
    }

    void FixedUpdate()
    {
        int i = 0;
        bool isSomePinMoving = false;
        foreach (Rigidbody pin in pins)
        {
            if (!PinStates[i] && pin.velocity.magnitude > restVelocityThreshold && pin.angularVelocity.magnitude > restAngularVelocityThreshold)
            {
                // this (un-knocked down) pin is moving, track and do appropriate action later
                isSomePinMoving = true;
            }

            // only check pins that are not already down for being knocked down
            if (!PinStates[i])
            {
                Vector3 rotation = pin.transform.localRotation.eulerAngles;
                float xRot = rotation.x;
                float zRot = rotation.z;
                bool isXDown = xRot > 45f && xRot < 315f;
                bool isZDown = zRot > 45f && zRot < 315f;
                if (isXDown || isZDown)
                {
                    PinStates[i] = true;
                    ResetTimeToEndOfFrame(); // time to frame end resets because pin was just knocked down
                }
            }
            i++;
        }
        if (isSomePinMoving)
        {
            if (isFrameActive)
            {
                ResetTimeToEndOfFrame();
            } else if (isFrameReady)
            {
                // first pin moved, begin frame
                FrameActive();
            }
        }
        if (isFrameActive)
        {
            timeTillEndOfFrame -= Time.fixedDeltaTime;
            if (timeTillEndOfFrame < 0)
            {
                Debug.Log("End of frame! Current=" + string.Join(',', PinStates) + " Prev=" + string.Join(',', PreviousPinStates));
                isFrameActive = false;
                isFrameReady = false;
                NotifyPinListeners();
            }
        }
    }

    // called both internally and by detectors
    public void FrameActive()
    {
        Debug.Log("Frame begins, first pin moved OR detector sensed ball in area");
        ResetTimeToEndOfFrame();
        isFrameActive = true;
    }

    private void ResetTimeToEndOfFrame()
    {
        timeTillEndOfFrame = timeBeforeEndFrame;
    }

    public void DestroyAllBowlingBalls()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("BowlingBall")) {
            Destroy(obj);
        }
    }

    /// <summary>
    /// Start first roll on frame (new frame).
    /// </summary>
    public void StartNextFrame()
    {
        Debug.Log("StartNextFrame called, preparing for next frame");
        isFrameActive = false;
        PreviousPinStates = new bool[pins.Length];
        StartCoroutine(nameof(DelayedFrameReady));
        ResetPins();
        DestroyAllBowlingBalls();
        SummonNewBall();
    }

    /// <summary>
    /// Start second roll on frame.
    /// </summary>
    public void StartNextRoll()
    {
        Debug.Log("StartNextRoll called, preparing for next roll");
        isFrameActive = false;
        PreviousPinStates = (bool[])PinStates.Clone();
        StartCoroutine(nameof(DelayedFrameReady));
        DestroyAllBowlingBalls();
        SummonNewBall();
    }

    public void SummonNewBall()
    {
        GameObject ball = Instantiate(ballPrefab, new Vector3(2.657f, 1.065f, -1.022f), Quaternion.identity);
        if (ball.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(new Vector3(0f, 0f, -20f), ForceMode.Impulse);
        }
    }

    private IEnumerator DelayedFrameReady()
    {
        yield return new WaitForSecondsRealtime(settleDelay);
        isFrameReady = true;
        bowlingBlocker.RaiseBlocker();
    }

    private IEnumerator DelayedStartGame()
    {
        yield return new WaitForSeconds(1f);
        StartNextFrame();
    }

    private void ResetPins()
    {
        ResetPinStates();
        int i = 0;
        foreach (Rigidbody pin in pins)
        {
            pin.velocity = Vector3.zero;
            pin.angularVelocity = Vector3.zero;
            pin.transform.SetLocalPositionAndRotation(startingPinLocalPositions[i].position, startingPinLocalPositions[i].rotation);
            i++;
        }
    }

    private void ResetPinStates()
    {
        for (int i = 0; i < PinStates.Length; i++)
        {
            PinStates[i] = false;
        }
    }

    public void RegisterPinListener(IPinEndListener listener)
    {
        Debug.Log("Registering pin end listener " + listener);
        pinListeners.Add(listener);
    }

    public void DeregisterPinListener(IPinEndListener listener)
    {
        pinListeners.Remove(listener);
    }

    private void NotifyPinListeners()
    {
        foreach (IPinEndListener pinListener in pinListeners)
        {
            pinListener?.OnPinEnd();
        }
    }
}
