using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class StanceTrainer : MonoBehaviour
{
    public GameObject sword;
    public int currentGuard = 0;
    public StanceObject.guardPose targetGuard;
    public int score;
    public int highScore;
    public TextMeshProUGUI stanceText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI timerText;
    private float timer = 3f;
    private float timerStart= 3f;
    
    // Start is called before the first frame update
    void Start()
    {
        currentGuard = Random.Range(0, 4);
        SetGuard(currentGuard);
    }

    // Update is called once per frame
    void Update()
    {
        CompareStances();
        if (score > 0)
        {
            timer -= Time.deltaTime;
            UpdateTexts();
        }

        if (timer <= 0)
        {
            timer = timerStart;
            score = 0;
            timerText.text = "Waiting to Start";
        }
        
    }

    private void CompareStances()
    {
        if (sword.GetComponent<SwordGuardData>().trackedGuard == targetGuard && GetComponent<TestingArea>().player_in_range)
        {
            ChangeRequiredStance();
            score++;
            timer = timerStart;
            if (score > highScore)
            {
                highScore = score;
            }
            UpdateTexts();
        }
    }

    private void UpdateTexts()
    {
        timerText.text = Math.Round(timer).ToString();
        scoreText.text = score.ToString();
        highScoreText.text = highScore.ToString();
    }

    private void ChangeRequiredStance()
    {
        bool newGuardSet = false;
        
            while (!newGuardSet)
            {
                int newGuard = Random.Range(0, 4);
                if (newGuard != currentGuard)
                {
                    currentGuard = newGuard;
                    newGuardSet = true;
                    SetGuard(currentGuard);
                }
            }
            // CHANGE SOME TEXT TO DISPLAY THE NEW STANCE AND THE SCORE
           
        
        
    }

    private void SetGuard(int newGuard)
    {
        switch (newGuard)
        {
            case 0:
                targetGuard = StanceObject.guardPose.Alber;
                stanceText.text = "Alber / Fool's Guard";
                break;
            case 1:
                targetGuard = StanceObject.guardPose.Ox;
                stanceText.text = "Ox";
                break;
            case 2:
                targetGuard = StanceObject.guardPose.Pflug;
                stanceText.text = "Pflug / Plow Guard";
                break;
            case 3:
                targetGuard = StanceObject.guardPose.Vomtag;
                stanceText.text = "Vom Tag / Roof Guard";
                break;
        }
    }
}
