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
        int itemAmount = 0;

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
        int watchCount = 0;
        CustomEvent watchAdEvent = new CustomEvent("watchAd")
        {
            {"watchCount", watchCount}
        };
        AnalyticsService.Instance.RecordEvent(watchAdEvent);
        Debug.Log("Watch Ad Record");
    }

    public void GameOver()
    {
        int gameOverCount = 0;
        CustomEvent gameOverEvent = new CustomEvent("gameOver")
        {
            {"gameOverCount", gameOverCount}
        };
        AnalyticsService.Instance.RecordEvent(gameOverEvent);
        Debug.Log("Game Over Record");
    }
}
