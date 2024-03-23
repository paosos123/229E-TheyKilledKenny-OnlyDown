using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    private bool soundPlayed = false; //soundisnotplayed
    [SerializeField] private GameObject slowBar;
    [SerializeField]List<GameObject> gameOver;
    public void Setup()
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

    public void ShowGameOver()
    {
       
        
    }
    public void GameOver()
    {
        if (ball == null)
        {
            if (!soundPlayed)
            {
                // Play sound here
                // GetComponent<AudioSource>().Play();
                soundPlayed = true;
            }
         Setup();
         slowBar.SetActive(false);
        }
    }

    public void Win()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        GameOver();
    }
    
}
