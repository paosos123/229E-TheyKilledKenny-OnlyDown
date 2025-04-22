using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuGame : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPaenl;
    [SerializeField] GameObject levelSeleMenu;

    [SerializeField] GameObject bgMenu;
    public Button[] levelSelect;
    // Start is called before the first frame update
    void Start()
    {
        int levelAt = PlayerPrefs.GetInt("levelAt",1);
        for (int i = 0; i < levelSelect.Length; i++)
        {
            if(i+1 > levelAt )
                levelSelect[i].interactable = false;
        }
    }
    public void level(string ty)
    {
        SceneManager.LoadScene(ty);
    }
  
    public void PlayClick()
    {
        mainMenuPaenl.SetActive(false);
        levelSeleMenu.SetActive(true);
        bgMenu.SetActive(true);
    }

}
