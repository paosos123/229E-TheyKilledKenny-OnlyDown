using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System;
using TMPro;

public class RewardAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms

    public Action onAdSuccess;

    public Movement movementScript;

    public TextMeshProUGUI countdownText;
    private float countdownTime = 5f; // เวลานับถอยหลัง
    private bool isCountdownActive = false;


    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

        // Disable the button until the ad is ready to show:
        _showAdButton.interactable = false;
    }

    private void Start()
    {
        StartCoroutine(WaitForAd());
    }

    IEnumerator WaitForAd()
    {
        while(!Advertisement.isInitialized)
        {
            Debug.Log("Waiting");
            yield return new WaitForSeconds(1);
        }
        LoadAd();
    }

    // Call this public method when you want to get an ad ready to show.
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
            // Configure the button to call the ShowAd() method when clicked:
            _showAdButton.onClick.AddListener(ShowAd);
            // Enable the button for users to click:
            _showAdButton.interactable = true;
        }
    }

    // Implement a method to execute when the user clicks the button:
    public void ShowAd()
    {
        // Disable the button:
        _showAdButton.interactable = false;
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
        AnalyticManager.instance.WatchAd();
    }

    IEnumerator CountdownToContinue()
    {
        isCountdownActive = true;

        while (countdownTime > 0)
        {
            countdownText.text = Mathf.Ceil(countdownTime).ToString(); // แสดงเวลานับถอยหลัง
            countdownTime -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = "Go!"; // เมื่อถึง 0
        _showAdButton.interactable = true; // ปลดล็อคปุ่ม Continue
        isCountdownActive = false;
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            //onAdSuccess?.Invoke();
            //StartCoroutine(CountdownToContinue());
            movementScript.ContinueAfterAd();
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        // Clean up the button listeners:
        _showAdButton.onClick.RemoveAllListeners();
    }
}
