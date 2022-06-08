using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using ToyBoxHHH;
using UnityEngine;
using Random = UnityEngine.Random;

public class CloverGameplay : MonoBehaviour
{
    public static CloverGameplay instance;


    // count the clovers and show it somewhere
    public CloverSpawner mainSpawner;
    public int cloversTotal => mainSpawner.fourLeafs;
    public int cloversCurrent = 0;

    public bool randomize = false;
    public int randomSeed = 1337;

    [Header("Score Feedback")] public TextMeshProUGUI scoreText;
    // maybe each player can pick clovers...?!?!

    private void Awake()
    {
        instance = this;
    }

    [DebugButton]
    public void Restart()
    {
        if (!randomize)
        {
            Random.InitState(randomSeed);
        }

        cloversCurrent = 0;
        mainSpawner.ClearAll();
        mainSpawner.SpawnAll();
    }

    public void CloverScoreUp()
    {
        cloversCurrent++;
        
        UpdateScoreFeedback();
    }

    public void UpdateScoreFeedback()
    {
        scoreText.text = cloversCurrent + "/" + cloversTotal;
    }
}