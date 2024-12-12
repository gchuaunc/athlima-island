using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CubeButton : MonoBehaviour
{
    public string text;
    public GameObject cubeA;
    public GameObject cubeB;
    private float time = 0f;
    private float time_multiplier = 0.1f;
    private bool hovered = false;
    private float size_mult = 1f;

    public Action action;

    public enum Action { GO_GOLF, GO_BOWLING, GO_LOBBY }

    void Start()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    void Update()
    {
        time += Time.deltaTime * time_multiplier;
        size_mult = Mathf.Clamp(size_mult + Time.deltaTime * (hovered ? 1 : -1), 1f, 1.5f);
        cubeA.GetComponent<Transform>().localRotation = Quaternion.Euler(
            360f * getCurveAtPoint(time, 1.3f, 0.3f, 1.3f, 0.2f, 1.1f, 0.4f),
            360f * getCurveAtPoint(time, 2.3f, 0.3f, 0.4f, 0.1f, 2.2f, 0.2f),
            360f * getCurveAtPoint(time, 1.6f, 0.1f, 2.6f, 0.3f, 2.7f, 0.2f)
            );
        cubeA.GetComponent<Transform>().localScale = Vector3.one * size_mult * (0.4f + 0.6f * getCurveAtPoint(time, 0.1f, 0.4f, 0.1f, 7.3f));
        cubeB.GetComponent<Transform>().localRotation = Quaternion.Euler(
            360f * getCurveAtPoint(time, 2.2f, 0.3f, 4.6f, 0.1f, 1.4f, 0.2f),
            360f * getCurveAtPoint(time, 3.1f, 0.2f, 4.7f, 0.1f, 1.6f, 0.3f),
            360f * getCurveAtPoint(time, 3.7f, 0.1f, 3.4f, 0.2f, 2.1f, 0.3f)
            );
        cubeB.GetComponent<Transform>().localScale = Vector3.one * size_mult * (0.4f + 0.6f * getCurveAtPoint(time, 0.1f, 0.4f, 0.1f, 8.3f));
    }

    private float getCurveAtPoint(float time, params float[] curveArgs)
    {
        float sum = 0f;
        for (int i = 0; i < curveArgs.Length / 2; i++) {
            sum += curveArgs[i * 2] * Mathf.Sin(curveArgs[i*2 + 1] * time);
        }
        return sum;
    }

    public void SetHover(bool hover)
    {
        time_multiplier = hover ? .7f : 0.5f;
        hovered = hover;
        cubeA.GetComponent<Transform>().GetChild(0).gameObject.SetActive(hover);
        cubeB.GetComponent<Transform>().GetChild(0).gameObject.SetActive(hover);
    }

    public void TriggerAction()
    {
        switch (action)
        {
            case Action.GO_BOWLING:
                SceneManager.LoadScene("Bowling");
                break;
            case Action.GO_LOBBY:
                SceneManager.LoadScene("Lobby");
                break;
            case Action.GO_GOLF:
                SceneManager.LoadScene("Golf");
                break;
        }
    }
}
