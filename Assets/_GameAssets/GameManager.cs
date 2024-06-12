using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    PlayerManager playerManager;
    [SerializeField] GameObject GameOverMenu;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject WinMenu;
    [SerializeField] TextMeshProUGUI scoreTxt;
    [SerializeField] TextMeshProUGUI highScore;
    [SerializeField] Coin[] coins;
    
    bool gameOver;
    bool pauseMenu;
    bool gamePaused;
    float score;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }
    private void Start()
    {
        gameOver = false;
        pauseMenu = false;
        gamePaused = false;
        PauseMenu.SetActive(pauseMenu);
        GameOverMenu.SetActive(gameOver);
        WinMenu.SetActive(false);
        Time.timeScale = 1;
        score = Time.time;
    }

    private void Update()
    {
        if (!gameOver)
        {
            if (playerManager.transform.position.y <= -10)
            {
                gameOver= true;
                //! Enable UI and pauseGame
                Time.timeScale = 0;
                gamePaused = true;
                GameOverMenu.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //! Pause The game and enable the ui
                pauseMenu = !pauseMenu;
                PauseMenu.SetActive(pauseMenu);
                Time.timeScale = (gamePaused) ? 1 : 0;
                gamePaused = !gamePaused;
                
            }
        }
    }
    private void UnPauseGame()
    {
        WinMenu.SetActive(false);
        gameOver = false;
        gamePaused = false;
        Time.timeScale = 1;
        GameOverMenu.SetActive(false);
        PauseMenu.SetActive(false);
        pauseMenu = false;

    }
    public void Restart()
    {
        UnPauseGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void CheckIfFinishedGame()
    {
        for (int i = 0; i < coins.Length; i++)
        {
            if (!coins[i].Collected)
            {
                return;
            }
        }
        score = Time.time - score;
        WinMenu.SetActive(true);
        Highscore();
        Time.timeScale = 0;
        gamePaused = true;
        Debug.Log("Finished");
    }
    void Highscore()
    {
        if (score < PlayerPrefs.GetFloat("Score"))
        {
            PlayerPrefs.SetFloat("Score", score);
            PlayerPrefs.Save();
        }
        scoreTxt.text = "Score : "+ score;
        highScore.text = "HighScore : " + PlayerPrefs.GetFloat("Score");
    }

}
