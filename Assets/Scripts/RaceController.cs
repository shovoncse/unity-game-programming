using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ✅ Legacy Text Support

public class RaceController : MonoBehaviour
{
    public int lapsInRace;
    public Text LapInfoText;  // ✅ Using Legacy Text
    public Text CheckpointInfoText;
    public Text RaceOverText;

    private int nextCheckpointNumber;
    private int checkpointCount;
    private int lapCount;
    private float lapStartTime;
    private bool isRaceActive;
    private List<float> lapTimes = new List<float>();  // Stores lap times
    private Checkpoint activeCheckpoint;

    void Start()
    {
        isRaceActive = true;
        lapStartTime = Time.time;
        nextCheckpointNumber = 0;
        lapCount = 0;
        checkpointCount = transform.childCount;

        // ✅ Assign checkpoint numbers correctly
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
            // ✅ Ensure Text UI is Updating Properly
            LapInfoText.text = "Lap Time: " + TimeParser(Time.time - lapStartTime);
            CheckpointInfoText.text = "Checkpoint " + (nextCheckpointNumber + 1) + "/" + checkpointCount +
                                      "\nLap " + (lapCount + 1) + "/" + lapsInRace;
        }
        else
        {
            LapInfoText.text = "";
            CheckpointInfoText.text = "";
            RaceOverText.color = Color.Lerp(Color.red, Color.green, Mathf.PingPong(Time.time, 1));
        }
    }

    public void StartRace()
    {
        activeCheckpoint = transform.GetChild(nextCheckpointNumber).GetComponent<Checkpoint>();
        activeCheckpoint.isActiveCheckpoint = true;
        lapStartTime = Time.time;
    }

    public void CheckpointPassed()
    {
        activeCheckpoint.isActiveCheckpoint = false;
        nextCheckpointNumber++;

        if (nextCheckpointNumber < checkpointCount)
        {
            activeCheckpoint = transform.GetChild(nextCheckpointNumber).GetComponent<Checkpoint>();
            activeCheckpoint.isActiveCheckpoint = true;
        }
        else
        {
            lapTimes.Add(Time.time - lapStartTime); // ✅ Store lap time
            lapCount++;
            lapStartTime = Time.time;
            nextCheckpointNumber = 0;

            if (lapCount < lapsInRace)
            {
                activeCheckpoint = transform.GetChild(nextCheckpointNumber).GetComponent<Checkpoint>();
                activeCheckpoint.isActiveCheckpoint = true;
            }
            else
            {
                FinishRace();
            }
        }
    }

    private void FinishRace()
    {
        isRaceActive = false;
        float raceTotalTime = 0.0f;
        float fastestLapTime = lapTimes[0];

        for (int i = 0; i < lapsInRace; i++)
        {
            if (lapTimes[i] < fastestLapTime)
            {
                fastestLapTime = lapTimes[i];
            }
            raceTotalTime += lapTimes[i];
        }

        // ✅ Display final results
        RaceOverText.text = "🏁 RACE COMPLETE! 🏁\n\nTotal Time: " + TimeParser(raceTotalTime) +
                            "\nBest Lap: " + TimeParser(fastestLapTime);
    }

    private string TimeParser(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int msecs = Mathf.FloorToInt((time * 100) % 100);
        return $"{minutes}:{seconds:00}:{msecs:00}";
    }
}
