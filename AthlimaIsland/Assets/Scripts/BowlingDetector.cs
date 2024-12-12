using UnityEngine;
using UnityEngine.Events;

public class BowlingDetector : MonoBehaviour
{
    [SerializeField] private UnityEvent actionOnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(gameObject.name + " OnTriggerEnter with " + other.name);
        if (other.CompareTag("BowlingBall"))
        {
            actionOnTrigger.Invoke();
        }
    }
}
