using System.Collections.Generic;
using UnityEngine;

public class PinsManager : MonoBehaviour
{
    [SerializeField] private Rigidbody[] pins;
    [SerializeField] private float timeBeforeEndFrame = 3f;
    [SerializeField] private float restVelocityThreshold = 0.1f;
    [SerializeField] private float restAngularVelocityThreshold = 0.1f;

    public static PinsManager Instance;

    public List<IPinEndListener> PinListeners { get; private set; }
    public bool[] PinStates { get; private set; }

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
        PinListeners = new List<IPinEndListener>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        int i = 0;
        foreach (Rigidbody pin in pins)
        {
            if (pin.velocity.magnitude <= restVelocityThreshold && pin.angularVelocity.magnitude <= restAngularVelocityThreshold)
            {
                Vector3 rotation = pin.transform.localRotation.eulerAngles;
                float xRot = rotation.x % 360f;
                float zRot = rotation.z % 360f;
                bool isXDown = Mathf.Abs(xRot) > 45f && Mathf.Abs(360 - xRot) > 45f;
                bool isZDown = Mathf.Abs(zRot) > 45f && Mathf.Abs(360 - zRot) > 45f;
                if (isXDown || isZDown)
                {
                    //Debug.Log("pin " + pin.gameObject.name + " knocked down xr=" + xRot + " zr=" + zRot);
                    if (!PinStates[i])
                    {
                        PinStates[i] = true;
                        // time to frame end resets because change detected
                    }
                }
            }
            i++;
        }
    }
}
