using System;
using UnityEngine;

public static class DebugLogger 
{

    public static void SendDebugMessage(string messageToDisplay,
        DebugLogLevels debugLevel = DebugLogLevels.UnityConsole)
    {

        switch (debugLevel)
        {
            case DebugLogLevels.UnityConsole:
                Debug.Log(messageToDisplay);
                break;
            case DebugLogLevels.LogFile:
                break;
            case DebugLogLevels.Both:
                Debug.Log(messageToDisplay);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(debugLevel), debugLevel, null);
        }

    }

    public enum DebugLogLevels
    {
        UnityConsole,
        LogFile,
        Both,
    }
}
