using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script responsible to control the timer, round and progress bar
public class StatusManager : MonoBehaviour
{
    // UI texts
    public Text score_ui;
    public Text round_ui;
    public Text timer_ui;
    public Text gameover_score;
    public Text gameover_highscore;

    public Slider progress;
    public int goal;
    public GameObject GameOver_canvas;
    public static StatusManager instance = null;
    public int next_round;
    public bool paused;

    private int current_points;
    private int progress_points;
    private float timer;
    private int minutes;
    private int seconds;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        current_points = int.Parse(score_ui.text);

        timer = 121;

        progress.value = 0;

        next_round = 1;
    }

    private void Update()
    {
        if (timer > 0)
        { 
            if(!paused)
                timer -= Time.deltaTime;

            minutes = Mathf.FloorToInt(timer / 60F);
            seconds = Mathf.FloorToInt(timer - minutes * 60);
            string nicetime = string.Format("{0:0}:{1:00}", minutes, seconds);
            timer_ui.text = nicetime;
        }
        else 
        {
            timer_ui.text = "0:00";
            GameOver_canvas.SetActive(true);
            gameover_score.text = current_points.ToString();

            if(current_points > PlayerPrefs.GetInt("HighScore"))
                PlayerPrefs.SetInt("HighScore", current_points);
            gameover_highscore.text = PlayerPrefs.GetInt("HighScore").ToString();
        }

        if (progress.value == 1.0f)
        {
            goal += 1000 + (int.Parse(round_ui.text)*500) + (int)(timer*10);
            timer = 120;

            progress_points = 0;
            current_points += (int)timer * 5;
            score_ui.text = current_points.ToString();
            progress.value = (float)progress_points / goal;

            next_round = int.Parse(round_ui.text) + 1;
            round_ui.text = next_round.ToString();
        }
    }

    public void UpdateScore(int new_points)
    {
        current_points += new_points;
        progress_points += new_points;
        score_ui.text = current_points.ToString();

        progress.value = (float)progress_points / goal;
    }

    public void PauseGame()
    {
        paused = true;
    }

    public void ResumeGame()
    {
        paused = false;
    }
}
