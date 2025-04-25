using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public AdsInitializer adsInitializer;
    public InterstitalAds interstitalAds;

    public static AdsManager Instance;

    private void Awake()
    {
        Instance = this;

        //interstitalAds = GetComponent<InterstitalAds>();
    }
}
