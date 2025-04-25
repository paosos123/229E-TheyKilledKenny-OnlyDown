using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class AnalyticManager : MonoBehaviour
{
    public static AnalyticManager instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private async void Initialize()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
    }

    public void OnItemUse(string itemName)
    {
        int itemAmount = PlayerPrefs.GetInt("ItemUse_" + itemName, 0);
        itemAmount++;
        PlayerPrefs.SetInt("ItemUse_" + itemName, itemAmount);

        // สร้าง Event และส่งไป Analytics
        CustomEvent itemUseEvent = new CustomEvent("itemUse")
        {
        {"itemName", itemName},
        {"itemAmount", itemAmount }
        };

        AnalyticsService.Instance.RecordEvent(itemUseEvent);
        Debug.Log("Item use record");
    }

    public void WatchAd()
    {
        int watchCount = PlayerPrefs.GetInt("watchCount", 0);
        watchCount++;
        PlayerPrefs.SetInt("watchCount", watchCount);

        CustomEvent watchAdEvent = new CustomEvent("watchAd")
        {
            {"watchCount", watchCount}
        };
        AnalyticsService.Instance.RecordEvent(watchAdEvent);
        Debug.Log("Watch Ad Record");
    }

    public void GameStart()
    {
        int gameStartCount = PlayerPrefs.GetInt("gameStartCount", 0);
        gameStartCount++;
        PlayerPrefs.SetInt("gameStartCount", gameStartCount);

        CustomEvent gameStartEvent = new CustomEvent("gameStartCount")
        {
            {"gameStartCount", gameStartCount}
        };
        AnalyticsService.Instance.RecordEvent(gameStartEvent);
        Debug.Log("Game Start Record");
    }

    public void GameOver()
    {
        int gameOverCount = PlayerPrefs.GetInt("gameOverCount", 0);
        gameOverCount++;
        PlayerPrefs.SetInt("gameOverCount", gameOverCount);

        CustomEvent gameOverEvent = new CustomEvent("gameOver")
        {
            {"gameOverCount", gameOverCount}
        };
        AnalyticsService.Instance.RecordEvent(gameOverEvent);
        Debug.Log("Game Over Record");
    }
}
