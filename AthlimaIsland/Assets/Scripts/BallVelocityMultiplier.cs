using System.Collections;
using UnityEngine;

public class BallVelocityMultiplier : MonoBehaviour
{
    [SerializeField] private Rigidbody thisRigidbody;
    [SerializeField] private float velocityThrowMultiplier = 5f;
    [SerializeField] private float maxDistanceToCamera = 5f;

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger) || OVRInput.GetUp(OVRInput.RawButton.LHandTrigger))
        {
            if (Vector3.Distance(Camera.main.transform.position, transform.position) < maxDistanceToCamera)
            {
                StartCoroutine(nameof(DelayedVelocityMultiplication));
            }
        }
    }

    private IEnumerator DelayedVelocityMultiplication()
    {
        yield return new WaitForSeconds(0.1f);
        float oldSpeed = thisRigidbody.velocity.magnitude;
        Vector3 direction = thisRigidbody.velocity.normalized;
        thisRigidbody.AddForce(direction * velocityThrowMultiplier, ForceMode.Impulse);
        Debug.Log("Thrown! velocity from " + oldSpeed + " to " + thisRigidbody.velocity.magnitude);
    }
}
