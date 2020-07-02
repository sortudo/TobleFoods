using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script responsible to control the timer, round and progress bar
public class StatusManager : MonoBehaviour
{
    public static StatusManager instance = null;

    // UI texts
    public Text score_ui;
    public Text round_ui;
    public Text timer_ui;
    public Text gameover_score;
    public Text gameover_highscore;

    // Other canvas
    public GameObject GameOver_canvas;
    public bool paused;
    public bool gameover;

    // Clips
    public AudioClip NextRound_clip;
    public AudioClip EndGame_clip;

    // Progress bar
    private int current_points;
    private int progress_points;
    public Slider progress;
    public int goal;
    public int next_round;

    // Timer
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

        timer = 121; // 2 minutes

        progress.value = 0;

        next_round = 1;

        gameover = false;
    }

    private void Update()
    {
        if (timer > 0 && !paused)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                EndGame(); 

            // Set timer string
            minutes = Mathf.FloorToInt(timer / 60F);
            seconds = Mathf.FloorToInt(timer - minutes * 60);
            string nicetime = string.Format("{0:0}:{1:00}", minutes, seconds);
            timer_ui.text = nicetime;
        }
        
        // If the game is over
        if(gameover)
        {
            Music.instance.music.Stop();
            StatusSound.instance.PlayStatus(EndGame_clip);
            gameover = false;
            timer_ui.text = "0:00";
            GameOver_canvas.SetActive(true);

            // Show the scores
            gameover_score.text = current_points.ToString();
            if(current_points > PlayerPrefs.GetInt("HighScore"))
                PlayerPrefs.SetInt("HighScore", current_points);
            gameover_highscore.text = PlayerPrefs.GetInt("HighScore").ToString();
        }

        // If the player reached the goal
        if (progress.value == 1.0f)
        {
            StatusSound.instance.PlayStatus(NextRound_clip);

            // The difficulty of each round increase depending on round number and time left
            goal += 1000 + (int.Parse(round_ui.text)*500) + (int)(timer*10);
            timer = 120;

            // Update the progress bar and score
            progress_points = 0;
            current_points += (int)timer * 5; // Earns points depending on the time left
            score_ui.text = current_points.ToString();
            progress.value = (float)progress_points / goal;

            next_round = int.Parse(round_ui.text) + 1;
            round_ui.text = next_round.ToString();
        }
    }

    // Function that update the progress bar and score with the new_points received
    public void UpdateScore(int new_points)
    {
        current_points += new_points;
        progress_points += new_points;
        score_ui.text = current_points.ToString();

        progress.value = (float)progress_points / goal;
    }

    // Function that pause the game
    public void PauseGame()
    {
        paused = true;
    }

    // Function that resume the game
    public void ResumeGame()
    {
        paused = false;
    }

    // Function that end the game
    public void EndGame()
    {
        timer = 0;
        gameover = true;
    }
}
