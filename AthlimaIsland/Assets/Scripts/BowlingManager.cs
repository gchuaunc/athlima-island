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
        rolls = new int[21]; // max 21 rolls in a game, -1 means not there yet
        for (int i = 0; i < rolls.Length; i++)
        {
            rolls[i] = -1;
        }
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

        // if strike, skip next roll
        if (currentRoll % 2 == 0 && knockedDown == 10)
        {
            rolls[currentRoll] = knockedDown;
            rolls[currentRoll + 1] = 0;
            currentRoll += 2;
        } else
        {
            rolls[currentRoll++] = knockedDown;
        }

        BowlingScoreDisplay.Instance.UpdateScore(GetScoreCard());

        if (currentRoll != 20 && currentRoll % 2 == 0)
        {
            // new frame, reset pins (EXCEPT 3rd roll on 10th frame)
            pinsManager.StartNextFrame();
        } else
        {
            pinsManager.StartNextRoll();
        }
    }

    public string[][] GetScoreCard()
    {
        Debug.Log(string.Join(',', rolls));

        // initialize score card
        List<string[]> scoreCard = new List<string[]>();
        for (int i = 0; i < 10; i++)
        {
            if (i == 10)
            {
                scoreCard.Add(new[] { "", "", "", "" });
            } else
            {
                scoreCard.Add(new[] { "", "", "" });
            }
        }
        
        // populate score card values
        int rollIndex = 0;
        int cumulativeScore = 0;

        for (int frame = 0; frame < 10; frame++)
        {
            if (rolls[rollIndex] == -1) break; // done, current roll hasn't been thrown yet
            if (frame < 9) // frames 1-9
            {
                // is this a completed frame?
                if (rolls[rollIndex + 1] != -1)
                {
                    // check for spare and strike
                    if (IsStrike(rollIndex))
                    {
                        try
                        {
                            cumulativeScore += 10 + StrikeBonus(rollIndex);
                            scoreCard[frame][0] = "";
                            scoreCard[frame][1] = "X";
                            scoreCard[frame][2] = cumulativeScore.ToString();
                        } catch (System.InvalidOperationException)
                        {
                            scoreCard[frame][0] = "";
                            scoreCard[frame][1] = "X";
                            scoreCard[frame][2] = "";
                        }
                    }
                    else if (IsSpare(rollIndex))
                    {
                        try
                        {
                            cumulativeScore += 10 + SpareBonus(rollIndex);
                            scoreCard[frame][0] = "";
                            scoreCard[frame][1] = "X";
                            scoreCard[frame][2] = cumulativeScore.ToString();
                        }
                        catch (System.InvalidOperationException)
                        {
                            scoreCard[frame][0] = "";
                            scoreCard[frame][1] = "X";
                            scoreCard[frame][2] = "";
                        }
                    } else
                    {
                        scoreCard[frame][0] = rolls[rollIndex].ToString();
                        scoreCard[frame][1] = rolls[rollIndex + 1].ToString();
                        cumulativeScore += rolls[rollIndex] + rolls[rollIndex + 1];
                        scoreCard[frame][2] = cumulativeScore.ToString();
                    }
                    rollIndex += 2;
                }
                else
                {
                    // incomplete frame (awaiting roll 2)
                    scoreCard[frame][0] = rolls[rollIndex].ToString();
                    break; // done now
                }
            }
            else // 10th frame
            {
                for (int i = 0; i < 3; i++)
                {
                    if (rolls[rollIndex] == -1) break; // done, current roll hasn't been thrown
                    if (rolls[rollIndex] == 10)
                    {
                        scoreCard[frame][i] = "X";
                    } else if (i > 0 && rolls[rollIndex - 1] + rolls[rollIndex] == 10)
                    {
                        scoreCard[frame][i] = "/";
                    } else
                    {
                        scoreCard[frame][i] = rolls[rollIndex].ToString();
                    }
                    rollIndex++;
                }

                if (scoreCard[frame][0] == "X" || scoreCard[frame][1] == "/")
                {
                    // eligible for 3rd throw, is it complete?
                    if (rolls[rolls.Length - 1] != -1)
                    {
                        // done with third throw completed
                        cumulativeScore += CalculateFrameScoreForTenthFrame();
                        scoreCard[frame][3] = scoreCard[frame][2]; // move 3rd throw score to correct location
                        scoreCard[frame][2] = cumulativeScore.ToString();
                    }
                } else
                {
                    // done, ineligible for 3rd throw
                    cumulativeScore += CalculateFrameScoreForTenthFrame();
                    scoreCard[frame][2] = cumulativeScore.ToString();
                }
            }
        }

        return scoreCard.ToArray();
    }

    // Helper methods
    private int CalculateFrameScoreForTenthFrame()
    {
        if (rolls[rolls.Length - 3] == -1 || rolls[rolls.Length - 2] == -1)
        {
            throw new System.InvalidOperationException("Rolls had not been thrown yet while calculating 10th frame score");
        }
        if (rolls[rolls.Length - 1] == -1)
        {
            return rolls[rolls.Length - 2] + rolls[rolls.Length - 3];
        } else
        {
            return rolls[rolls.Length - 1] + rolls[rolls.Length - 2] + rolls[rolls.Length - 3];
        }
    }

    private bool IsStrike(int rollIndex)
    {
        if (rollIndex % 2 != 0)
        {
            throw new System.ArgumentException("rollIndex " + rollIndex + " was not aligned to a frame!");
        }
        return rolls[rollIndex] == 10 && rolls[rollIndex + 1] == 0;
    }

    private int StrikeBonus(int rollIndex)
    {
        if (rollIndex % 2 != 0)
        {
            throw new System.ArgumentException("rollIndex " + rollIndex + " was not aligned to a frame!");
        }
        if (rolls[rollIndex + 2] == -1 || rolls[rollIndex + 3] == -1)
        {
            throw new System.InvalidOperationException("Rolls had not been thrown yet while calculating strike bonus for " + rollIndex);
        }
        return rolls[rollIndex + 2] + rolls[rollIndex + 3];
    }

    private bool IsSpare(int rollIndex)
    {
        if (rollIndex % 2 != 0)
        {
            throw new System.ArgumentException("rollIndex " + rollIndex + " was not aligned to a frame!");
        }
        return rolls[rollIndex] + rolls[rollIndex + 1] == 10;
    }

    private int SpareBonus(int rollIndex)
    {
        if (rollIndex % 2 != 0)
        {
            throw new System.ArgumentException("rollIndex " + rollIndex + " was not aligned to a frame!");
        }
        if (rolls[rollIndex + 2] == -1 )
        {
            throw new System.InvalidOperationException("Roll had not been thrown yet while calculating spare bonus for " + rollIndex);
        }
        return rolls[rollIndex + 2];
    }
}
