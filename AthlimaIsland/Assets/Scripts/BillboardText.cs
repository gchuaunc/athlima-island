using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardText : MonoBehaviour
{
    [SerializeField] private bool yUp = true;
    [SerializeField] private RectTransform rectTransform;

    private Camera mainCamera; // cache for performance

    private void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.LookAt((2 * transform.position) - mainCamera.transform.position);
        if (yUp)
        {
            rectTransform.rotation = Quaternion.Euler(0, rectTransform.rotation.eulerAngles.y, 0);
        }
    }
}
