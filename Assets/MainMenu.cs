using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]GameObject gameCrdNotShowList;
    [SerializeField]GameObject gameCrdShow;
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ShowCrd()
    {
        gameCrdShow.SetActive(true);
        gameCrdNotShowList.SetActive(false);
    }

    public void Back()
    {
        {
            gameCrdShow.SetActive(false);
            gameCrdNotShowList.SetActive(true);
        }
    }
}
