using GameAnalyticsSDK;
using System.Diagnostics;

public static class Statics 
{
    /// <summary>
    /// Ad Events
    /// </summary>

    // LEVEL PROGRESSION EVENTS 

    public static void GA_LevelStartEvent(string Level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, Level);
        Debug.Print("StartEvent:" + Level);
    }

    public static void GA_LevelCompleteEvent(string Level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, Level);
        Debug.Print("CompleteEvent:" + Level);
    }

    public static void GA_LevelFailedEvent(string Level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, Level);
        Debug.Print("FailEvent:" + Level);
    }

}
