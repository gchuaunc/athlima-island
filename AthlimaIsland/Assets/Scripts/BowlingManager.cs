using System.Collections.Generic;
using UnityEngine;

public class BowlingManager : MonoBehaviour, IPinEndListener
{
    public static BowlingManager Instance { get; private set; }

    private PinsManager pinsManager; // cache instance
    private int[] rolls;
    private int currentRoll;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Warning: more than one BowlingManager in the scene, disabling this.");
            enabled = false;
            return;
        }
        rolls = new int[21]; // max 21 rolls in a game
        currentRoll = 0;
    }

    private void Start()
    {
        pinsManager = PinsManager.Instance;
        pinsManager.RegisterPinListener(this);
    }

    public void OnPinEnd()
    {
        Debug.Log("OnPinEnd called");
        if (currentRoll == rolls.Length) return; // end of game already

        // calculate how many pins just knocked down
        int knockedDown = 0;
        for (int i = 0; i < pinsManager.PinStates.Length; i++)
        {
            if (pinsManager.PinStates[i] && !pinsManager.PreviousPinStates[i])
            {
                knockedDown++;
            }
        }

        rolls[currentRoll++] = knockedDown;

        BowlingScoreDisplay.Instance.UpdateScore(GetScoreCard());
    }

    public string[][] GetScoreCard()
    {
        var scoreCard = new List<string[]>();
        int rollIndex = 0;
        int cumulativeScore = 0;

        for (int frame = 0; frame < 10; frame++)
        {
            if (frame < 9) // Frames 1-9
            {
                if (IsStrike(rollIndex))
                {
                    cumulativeScore += 10 + StrikeBonus(rollIndex);
                    scoreCard.Add(new[] { "X", "", cumulativeScore.ToString() });
                    rollIndex++;
                }
                else if (IsSpare(rollIndex))
                {
                    cumulativeScore += 10 + SpareBonus(rollIndex);
                    scoreCard.Add(new[] { rolls[rollIndex].ToString(), "/", cumulativeScore.ToString() });
                    rollIndex += 2;
                }
                else
                {
                    int frameScore = rolls[rollIndex] + rolls[rollIndex + 1];
                    cumulativeScore += frameScore;
                    scoreCard.Add(new[]
                    {
                        rolls[rollIndex].ToString(),
                        rolls[rollIndex + 1].ToString(),
                        cumulativeScore.ToString()
                    });
                    rollIndex += 2;
                }
            }
            else // 10th frame
            {
                var frameRolls = new List<string>();
                for (int i = 0; i < 3 && rollIndex < currentRoll; i++)
                {
                    if (rolls[rollIndex] == 10) frameRolls.Add("X");
                    else if (i > 0 && rolls[rollIndex - 1] + rolls[rollIndex] == 10) frameRolls.Add("/");
                    else frameRolls.Add(rolls[rollIndex].ToString());
                    rollIndex++;
                }

                cumulativeScore += CalculateFrameScoreForTenthFrame();
                frameRolls.Add(cumulativeScore.ToString());
                scoreCard.Add(frameRolls.ToArray());
            }
        }

        return scoreCard.ToArray();
    }

    private int CalculateFrameScoreForTenthFrame()
    {
        return rolls[rolls.Length - 1] + rolls[rolls.Length - 2] + rolls[rolls.Length - 3]; // TODO: spare and strike scoring
        int score = 0;
        for (int i = 0; i < 3 && currentRoll > i; i++)
        {
            score += rolls[currentRoll - 3 + i];
        }
        return score;
    }

    // Helper methods
    private bool IsStrike(int rollIndex)
    {
        return rolls[rollIndex] == 10;
    }

    private int StrikeBonus(int rollIndex)
    {
        return rolls[rollIndex + 2] + rolls[rollIndex + 3];
    }

    private bool IsSpare(int rollIndex)
    {
        return rolls[rollIndex] + rolls[rollIndex + 1] == 10;
    }

    private int SpareBonus(int rollIndex)
    {
        return rolls[rollIndex + 2];
    }
}
