using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenegame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        // 3. หา Build Index ของ Scene ปัจจุบัน
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 2. โหลด Scene เดิมอีกครั้งโดยใช้ Build Index
        SceneManager.LoadScene(currentSceneIndex);
        AnalyticManager.instance.GameStart();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void LoadNextLevel()
    {
        // 1. หา Build Index ของ Scene ปัจจุบัน
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 2. คำนวณ Build Index ของ Scene ถัดไป
        int nextSceneIndex = currentSceneIndex + 1;

        // 3. ตรวจสอบว่ามี Scene ถัดไปใน Build Settings หรือไม่
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            PlayerPrefs.SetInt("levelAt", nextSceneIndex);
            Debug.Log($"Loading next scene with build index {nextSceneIndex}");
        }
        else
        {
            Debug.Log("This is the last scene. No next scene available.");
        }
    }
        
}
