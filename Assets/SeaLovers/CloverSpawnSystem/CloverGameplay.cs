using System.Collections;
using System.Collections.Generic;
using TMPro;
using ToyBoxHHH;
using UnityEngine;

public class CloverGameplay : MonoBehaviour
{
    
    // count the clovers and show it somewhere
    public CloverSpawner mainSpawner;
    public int cloversTotal => mainSpawner.fourLeafs;
    public int cloversCurrent = 0;
    
    [Header("Score Feedback")] 
    public TextMeshProUGUI scoreText;
    // maybe each player can pick clovers...?!?!

    [DebugButton]
    public void Restart()
    {
        cloversCurrent = 0;
        mainSpawner.ClearAll();
        mainSpawner.SpawnAll();
    }
    
    public void PickClover()
    {
        cloversCurrent++;
    }

    public void UpdateScoreFeedback()
    {
        scoreText.text = cloversCurrent + "/" + cloversTotal;
    }
    
}
