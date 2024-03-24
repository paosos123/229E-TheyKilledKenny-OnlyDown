using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    public movement _movement;
    [SerializeField] private GameObject ball;
    private bool soundPlayed = false; //soundisnotplayed
    [SerializeField] private GameObject slowBar;
    [SerializeField]List<GameObject> gameOver;
    [SerializeField]List<GameObject> gameWin;
    [SerializeField] private GameObject endPoint;
    public AudioClip cilp,cilp1;
    
    public AudioSource source;
    public void GameOverSetup()
    {
        foreach (GameObject gameObject in gameOver)
        {
            // ตั้งค่า GameObject ให้ Active
            gameObject.SetActive(true);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }

    
    public void GameOver()
    {
        if (ball == null&&_movement.gameOverCon)
        {
            if (!soundPlayed)
            {
                source.PlayOneShot(cilp);
                soundPlayed = true;
            }

            GameOverSetup();
         slowBar.SetActive(false);
        }
    }
    public void GameWinSetup()
    {
        foreach (GameObject gameObject in gameWin)
        {
            // ตั้งค่า GameObject ให้ Active
            gameObject.SetActive(true);
        }
    }
    public void Win()
    {
        if (_movement.winCon&&ball==null)
        {
            if (!soundPlayed)
            {
                source.PlayOneShot(cilp1);
               
                soundPlayed = true;
            }

            GameWinSetup();
            slowBar.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        GameOver();
        Win();
    }
   
}
