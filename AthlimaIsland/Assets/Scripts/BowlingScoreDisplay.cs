using TMPro;
using UnityEngine;

public class BowlingScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text[] scoreTexts = new TMP_Text[31];

    public static BowlingScoreDisplay Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.LogWarning("Warning: more than one BowlingScoreDisplay in scene, disabling this one");
            enabled = false;
            return;
        }
    }

    public void UpdateScore(string[][] scores)
    {
        int i = 0;
        for (int j = 0; j < scores.Length; j++)
        {
            scoreTexts[i++].text = scores[j][0];
            scoreTexts[i++].text = scores[j][1];
            scoreTexts[i++].text = scores[j][2];
            if (j == 10)
            {
                scoreTexts[i++].text = scores[j][3];
            }
        }
    }
}
