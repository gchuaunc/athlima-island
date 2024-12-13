using System;
using TMPro;
using UnityEngine;

public class BowlingLane : MonoBehaviour
{
    [SerializeField] private int laneNumber;
    [SerializeField] private TMP_Text text;

    [SerializeField] private GameObject[] guardRails;

    private bool guardRailsUp = false;

    // Start is called before the first frame update
    void Start()
    {
        text.text = laneNumber.ToString();
    }

    private void Update()
    {
        foreach (GameObject guardRail in guardRails) {
            guardRail.GetComponent<Transform>().localPosition = new Vector3(
                guardRail.GetComponent<Transform>().localPosition.x,
                Mathf.Clamp(guardRail.GetComponent<Transform>().localPosition.y + Time.deltaTime * 0.5f * (guardRailsUp ? 1 : -1), -0.7f, 0),
                guardRail.GetComponent<Transform>().localPosition.z
                );
        }
    }

    public void ToggleGuardRails()
    {
        Debug.Log(guardRailsUp);
        guardRailsUp = !guardRailsUp; 
    }
}
