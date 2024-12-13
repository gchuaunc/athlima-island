using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public int level;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OpenScene()
    {
        if (level == 1)
        {
            SceneManager.LoadScene("Level 1");
        }
        else
        {
            SceneManager.LoadScene("Level 2");
        }
        
    }
}
