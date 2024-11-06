using UnityEngine;
using UnityEngine.UI;

public class UserInactivityChecker : MonoBehaviour
{
    public Text timeText, timeLeftText;
    public float timeLeft = 25;
    public float time;
    public Image _panel;

    private void Start()
    {
      //  timeLeft = RemoteConfigSample.Instance.NoActAdTime;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            ResetTimer();
            return;
        }
        time += Time.deltaTime;
        var timeleft = timeLeft - time;
        var timeTextValue = Mathf.RoundToInt(timeLeft - time);
        timeLeftText.text = timeTextValue.ToString();
        if (timeleft <= 3)
        {
           timeText.gameObject.SetActive(true);
           timeLeftText.gameObject.SetActive(true);
            _panel.enabled = true;
        }
        if (time >= timeLeft)
        {
            CallInterstitial();
            ResetTimer();
        }
    }

    public void CallInterstitial()
    {
          AdsManager.instance.ShowInterstitial();
    }

    public void ResetTimer()
    {
        if (PlayerPrefs.GetInt("NoAds") == 1) return;

        time = 0;
      //  timeLeft = RemoteConfigSample.Instance.NoActAdTime;
        timeText.gameObject.SetActive(false);
        timeLeftText.gameObject.SetActive(false);
        _panel.enabled = false;
    }

 
}
