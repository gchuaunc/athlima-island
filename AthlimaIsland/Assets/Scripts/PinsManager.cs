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
                Debug.Log("Frame begins, first pin moved");
                ResetTimeToEndOfFrame();
                isFrameActive = true;
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

    private void ResetTimeToEndOfFrame()
    {
        timeTillEndOfFrame = timeBeforeEndFrame;
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
    }

    private IEnumerator DelayedFrameReady()
    {
        yield return new WaitForSecondsRealtime(settleDelay);
        isFrameReady = true;
    }

    private void ResetPins()
    {
        ResetPinStates();
        int i = 0;
        foreach (Rigidbody pin in pins)
        {
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
