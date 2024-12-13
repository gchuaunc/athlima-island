using UnityEngine;

public class GolfBallRespawner : MonoBehaviour
{
    [SerializeField] private Vector3 respawnLocation;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("ball"))
        {
            collision.collider.transform.position = respawnLocation;
            if (collision.gameObject.TryGetComponent(out Rigidbody rb))
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            } else
            {
                Debug.LogWarning("Tried to reset golf ball velocity after fall off but failed to find rigidbody!");
            }
        }
    }
}
