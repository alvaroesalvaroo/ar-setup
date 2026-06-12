using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameConsole : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Text logText;
    [SerializeField] ScrollRect scrollRect;

    readonly List<string> logs = new List<string>();
    const int maxLogs = 50;

    void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        string color = type switch
        {
            LogType.Error     => "#FF4444",
            LogType.Exception => "#FF4444",
            LogType.Warning   => "#FFAA00",
            _ => "#FFFFFF"
        };

        logs.Add($"<color={color}>[{type}] {message}</color>");
        if (logs.Count > maxLogs) logs.RemoveAt(0);

        logText.text = string.Join("\n", logs);

        // Auto-scroll al final
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);
    }
}