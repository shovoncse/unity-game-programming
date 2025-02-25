using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceController : MonoBehaviour
{
    public int lapsInRace = 3;
    public TMP_Text LapInfoText;
    public TMP_Text CheckpointInfoText;
    public TMP_Text RaceOverText;

    private int nextCheckpointNumber;
    private int checkpointCount;
    private int lapCount;
    private float lapStartTime;
    private bool isRaceActive = true;
    private List<float> lapTimes = new List<float>();
    private Checkpoint activeCheckpoint;

    void Start()
    {
        isRaceActive = true;
        lapStartTime = Time.time;
        nextCheckpointNumber = 0;
        lapCount = 0;
        checkpointCount = transform.childCount;

        // Initialize checkpoints
        for (int i = 0; i < checkpointCount; i++)
        {
            Checkpoint cp = transform.GetChild(i).GetComponent<Checkpoint>();
            cp.checkpointNumber = i;
            cp.isActiveCheckpoint = false;
        }

        StartRace();
    }

    void Update()
    {
        if (isRaceActive)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (LapInfoText)
            LapInfoText.text = $"LAP TIME: {TimeParser(Time.time - lapStartTime)}";

        if (CheckpointInfoText)
            CheckpointInfoText.text = $"CHECKPOINT {nextCheckpointNumber + 1} / {checkpointCount}\nLAP {lapCount + 1} / {lapsInRace}";
    }

    public void StartRace()
    {
        lapStartTime = Time.time;
        nextCheckpointNumber = 0;
        lapCount = 0;

        // Activate first checkpoint
        if (checkpointCount > 0)
        {
            activeCheckpoint = transform.GetChild(nextCheckpointNumber).GetComponent<Checkpoint>();
            activeCheckpoint.isActiveCheckpoint = true;
        }
    }

    public void CheckpointPassed()
    {
        // Move to next checkpoint or finish lap
        activeCheckpoint.isActiveCheckpoint = false;
        nextCheckpointNumber++;

        if (nextCheckpointNumber < checkpointCount)
        {
            activeCheckpoint = transform.GetChild(nextCheckpointNumber).GetComponent<Checkpoint>();
            activeCheckpoint.isActiveCheckpoint = true;
        }
        else
        {
            // Lap completed
            lapTimes.Add(Time.time - lapStartTime);
            lapCount++;
            lapStartTime = Time.time;
            nextCheckpointNumber = 0;

            if (lapCount < lapsInRace)
            {
                // Start next lap
                activeCheckpoint = transform.GetChild(nextCheckpointNumber).GetComponent<Checkpoint>();
                activeCheckpoint.isActiveCheckpoint = true;
            }
            else
            {
                EndRace();
            }
        }
    }

    private void EndRace()
    {
        isRaceActive = false;

        // Calculate race results
        float totalRaceTime = 0f;
        float fastestLap = lapTimes[0];

        foreach (float lapTime in lapTimes)
        {
            totalRaceTime += lapTime;
            if (lapTime < fastestLap)
                fastestLap = lapTime;
        }

        // Update final result UI
        if (RaceOverText)
        {
            RaceOverText.text = $"🏆 RACE COMPLETE!\n\nTotal Time: {TimeParser(totalRaceTime)}\nBest Lap: {TimeParser(fastestLap)}";
            RaceOverText.color = Color.green;  // Ensure visibility
        }

        // Clear UI
        if (LapInfoText) LapInfoText.text = "";
        if (CheckpointInfoText) CheckpointInfoText.text = "";
    }

    private string TimeParser(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }
}
