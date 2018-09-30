using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleMobileAds.Api;
public class adMobManager : MonoBehaviour {

	 public string android_banner_id;
    public string ios_banner_id;
 
    public string android_interstitial_id;
    public string ios_interstitial_id;
 
    private BannerView bannerView;
    private InterstitialAd interstitialAd;
	
    public void Start()
    {
		MobileAds.Initialize("ca-app-pub-7605786350289600~6072272654");
        // RequestBannerAd();
        // RequestInterstitialAd();
		// ShowBannerAd();
    }
    public void RequestAD(){
        RequestBannerAd();
        RequestInterstitialAd();
    }
    public void RequestBannerAd()
    {
        string adUnitId = string.Empty;

		

#if UNITY_ANDROID
        adUnitId = android_banner_id;
#elif UNITY_IOS
        adUnitId = ios_bannerAdUnitId;
#else
		adUnitId = android_banner_id;
#endif
 
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder()
        .AddTestDevice("35E3F72D19C13997D32062438FC85BB4")
		.AddTestDevice("A76DF6A47725940F")
		.Build();
 
        bannerView.LoadAd(request);
		bannerView.Hide();
    }
 
    private void RequestInterstitialAd()
    {
        string adUnitId = string.Empty;
 
#if UNITY_ANDROID
        adUnitId = android_interstitial_id;
#elif UNITY_IOS
        adUnitId = ios_interstitialAdUnitId;
#endif
 
        interstitialAd = new InterstitialAd(adUnitId);
        // AdRequest request = new AdRequest.Builder().Build();
		  AdRequest request = new AdRequest.Builder()
        .AddTestDevice("35E3F72D19C13997D32062438FC85BB4")
        .AddTestDevice("A76DF6A47725940F")
		.Build();
 
        interstitialAd.LoadAd(request);
 
        interstitialAd.OnAdClosed += HandleOnInterstitialAdClosed;
    }
 
    public void HandleOnInterstitialAdClosed(object sender, EventArgs args)
    {
        print("HandleOnInterstitialAdClosed event received.");
 
        interstitialAd.Destroy();
 
        RequestInterstitialAd();
    }
 
    public void ShowBannerAd()
    {
        bannerView.Show();
    }
 
    public void ShowInterstitialAd()
    {
        if (!interstitialAd.IsLoaded())
        {
            RequestInterstitialAd();
            return;
        }
 
        interstitialAd.Show();
    }
}
