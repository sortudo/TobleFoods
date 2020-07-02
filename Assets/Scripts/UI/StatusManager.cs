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
    public AudioClip NextRound;
    public AudioClip GameOver;
    public AudioClip NextRound_voice;
    public AudioClip GameOver_voice;

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
        // If the game is running
        if (timer > 0 && !paused)
        {
            
            timer -= Time.deltaTime;
            // If the timer is over it will start the end game
            if (timer <= 0)
            {
                SoundSpeed(1.0f);
                EndGame();
            }      
            else if(timer <= 15) // If there are 15 seconds left, the background music will speed up
            {
                SoundSpeed(1.0f + (0.5f * ((15 - timer) / 15)));
            }
            else
            {
                // If the music pitch is distorted, bring back to normal
                if (Music.instance.music.pitch < 1.0f)
                    Music.instance.music.pitch = Music.instance.music.pitch + Time.deltaTime;
            }

            // Set timer string
            minutes = Mathf.FloorToInt(timer / 60F);
            seconds = Mathf.FloorToInt(timer - minutes * 60);
            string nicetime = string.Format("{0:0}:{1:00}", minutes, seconds);
            timer_ui.text = nicetime;
        }
        
        // The background music will slow down when the game is paused
        if (Music.instance.music.pitch > 0.5f && paused && timer >= 15)
        {
            Music.instance.music.pitch = Music.instance.music.pitch - Time.deltaTime;
        }

        // If the game is over
        if(gameover)
        {
            Music.instance.music.Stop();
            StatusSound.instance.PlayStatus(GameOver);
            Narrator.instance.PlayNarrator(GameOver_voice, true);
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
        if (progress.value == 1.0f && !gameover)
        {
            SoundSpeed(1.0f);
            StatusSound.instance.PlayStatus(NextRound);
            Narrator.instance.PlayNarrator(NextRound_voice, true);

            // The difficulty of each round increase depending on round number and time left
            goal += 1000 + (int.Parse(round_ui.text)*500) + (int)(timer*10);
            timer = 120;

            // Update the progress bar and score
            progress_points = 0;
            current_points += (int)timer * 5; // Earns points depending on the time left
            score_ui.text = current_points.ToString();
            progress.value = (float)progress_points / goal;

            // Set next round
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

    // Function that change the pitch of game' sounds
    public void SoundSpeed(float pitch)
    {
        Music.instance.music.pitch = pitch;
        SFXManager.instance.sfx.pitch = pitch;
        StatusSound.instance.status.pitch = pitch;
        Narrator.instance.narrator.pitch = pitch;
    }
}
