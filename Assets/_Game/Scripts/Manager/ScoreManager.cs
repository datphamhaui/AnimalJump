using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int Score { get; private set; }

    public void AddScore(int score)
    {
        Score += score;
    }

    public int GetBestScore()
    {
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (Score > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", Score);
            return Score;
        }
        else
        {
            return bestScore;
        }
    }
}
