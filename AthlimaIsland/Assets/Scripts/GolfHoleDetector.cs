using UnityEngine;

public class GolfHoleDetector : MonoBehaviour
{
    [SerializeField] private GameObject winGameObject; // object to be activated on win

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("ball"))
        {
            // win
            winGameObject.SetActive(true);
            Debug.Log("Win level!");
        }
    }
}
