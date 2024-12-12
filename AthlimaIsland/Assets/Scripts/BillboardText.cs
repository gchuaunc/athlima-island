using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardText : MonoBehaviour
{
    // Start is called before the first frame update
    public bool yUp = true;
    // Update is called once per frame
    void Update()
    {
        // Might want to change this to Camera.main
        Camera camera = Camera.current;

        GetComponent<RectTransform>().LookAt(camera.transform);
        if (yUp)
        {
            GetComponent<RectTransform>().rotation = Quaternion.Euler(0, GetComponent<RectTransform>().rotation.eulerAngles.y, 0);
        }
    }
}
