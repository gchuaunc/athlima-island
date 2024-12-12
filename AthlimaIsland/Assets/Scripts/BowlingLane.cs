using TMPro;
using UnityEngine;

public class BowlingLane : MonoBehaviour
{
    [SerializeField] private int laneNumber;
    [SerializeField] private TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        text.text = laneNumber.ToString();
    }
}
