using System;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    private BannerView bannerView;
    public InterstitialAd interstitial;
    public RewardedAd rewardBasedVideo;

    public Text infotext;

    string bannerID = "ca-app-pub-3940256099942544/6300978111";//test
    string interstitialID = "ca-app-pub-1163304301495832/5326322993"; //test
    string rewardedAdID = "ca-app-pub-1163304301495832/2216509157"; //test
    string rewardedAdIDIOS = "ca-app-pub-1163304301495832/1697634405"; //test

    public void Start()
    {

#if UNITY_ANDROID
        string appId = "ca-app-pub-1163304301495832~2080777781";
#elif UNITY_IPHONE
        string appId = "ca-app-pub-3940256099942544~1458002511";
#else
        string appId = "unexpected_platform";
#endif

        MobileAds.SetiOSAppPauseOnBackground(true);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        //this.rewardBasedVideo = RewardBasedVideoAd.Instance;
    }

    private AdRequest CreateAdRequest()
    {
        //infotext.text += "CreateAdReq";
        return new AdRequest.Builder()
            .AddKeyword("game")
            .SetGender(Gender.Male)
            .SetBirthday(new DateTime(1985, 1, 1))
            .TagForChildDirectedTreatment(false)
            .AddExtra("color_bg", "9B30FF")
            .Build();
    }

    public void DestroyBanner()
    {
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }
    }

    public void RequestBanner()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = bannerID;
#elif UNITY_ANDROID
        string adUnitId = bannerID;
#elif UNITY_IPHONE
        string adUnitId = bannerID;
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner ad before creating a new one.
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        // Register for ad events.
        this.bannerView.OnAdLoaded += this.HandleAdLoaded;
        this.bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        this.bannerView.OnAdOpening += this.HandleAdOpened;
        this.bannerView.OnAdClosed += this.HandleAdClosed;
        this.bannerView.OnAdLeavingApplication += this.HandleAdLeftApplication;

        // Load a banner ad.
        this.bannerView.LoadAd(this.CreateAdRequest());
    }

    public void RequestInterstitial()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = interstitialID;
#elif UNITY_ANDROID
        string adUnitId = interstitialID;
#elif UNITY_IPHONE
        string adUnitId = interstitialID;
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up interstitial ad before creating a new one.
        if (this.interstitial != null)
        {
            this.interstitial.Destroy();
        }

        // Create an interstitial.
        this.interstitial = new InterstitialAd(adUnitId);

        // Register for ad events.
        this.interstitial.OnAdLoaded += this.HandleInterstitialLoaded;
        this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
        this.interstitial.OnAdOpening += this.HandleInterstitialOpened;
        this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
        this.interstitial.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;
        //infotext.text += "ReqIntersitial";
        // Load an interstitial ad.
        this.interstitial.LoadAd(this.CreateAdRequest());
    }

    public void RequestRewardBasedVideo()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = rewardedAdID;
#elif UNITY_ANDROID
        string adUnitId = rewardedAdID;
#elif UNITY_IPHONE
        string adUnitId = rewardedAdIDIOS;
#else
        string adUnitId = "unexpected_platform";
#endif
        rewardBasedVideo = new RewardedAd(adUnitId);
        // Called when an ad request has successfully loaded.
        rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the ad starts to play.
        rewardBasedVideo.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        //infotext.text += "ReqIntersitial";
        // Load an interstitial ad.
        ServerSideVerificationOptions options = new ServerSideVerificationOptions.Builder()
        .SetUserId(PlayerPrefs.GetString("Username"))
        .Build();
        rewardBasedVideo.SetServerSideVerificationOptions(options);
        this.rewardBasedVideo.LoadAd(this.CreateAdRequest());
    }

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
    }
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
        SceneManager.LoadScene("UserMenu");
    }


    public void HandleRewardBasedVideoFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message);
        SceneManager.LoadScene("UserMenu");
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
        SceneManager.LoadScene("UserMenu");
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type);
        SceneManager.LoadScene("UserMenu");
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
        SceneManager.LoadScene("UserMenu");
    }

    public void ShowInterstitial()
    {
        if (this.interstitial.IsLoaded())
        {
            //infotext.text += "Loaded";
            this.interstitial.Show();
        }
        else
        {
            //infotext.text += "Not ready";
            Debug.Log("Interstitial is not ready yet");
        }
    }

    public void ShowRewardedAd()
    {
        if (this.rewardBasedVideo.IsLoaded())
        {
            //infotext.text += "Loaded";
            this.rewardBasedVideo.Show();
        }
        else
        {
            //infotext.text += "Not ready";
            //Debug.Log("Interstitial is not ready yet");
        }
    }

    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    public void HandleAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeftApplication event received");
    }

    #endregion
    #region Interstitial callback handlers

    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialLoaded event received");
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //infotext.text += "Fail to load";
        Debug.Log("HandleInterstitialFailedToLoad event received with message: " + args.Message);
        //SceneManager.LoadScene("UserMenu");
        
    }

    public void HandleInterstitialOpened(object sender, EventArgs args)
    {
        Debug.Log("HandleInterstitialOpened event received");
    }

    public void HandleInterstitialClosed(object sender, EventArgs args)
    {
        //infotext.text += "Ads Closed";
        Debug.Log("HandleInterstitialClosed event received");
        //SceneManager.LoadScene("UserMenu");
    }

    public void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialLeftApplication event received");
        //SceneManager.LoadScene("UserMenu");
    }

    #endregion

}