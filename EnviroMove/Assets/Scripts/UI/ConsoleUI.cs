using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ConsoleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text DebugFPS;
    [SerializeField] private int maxLines = 10;
    private int lineCount = 0;

    private string myLog;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("Hello World");
        }
        if(DebugFPS)DebugFPS.text = "FPS: " + (int)(1.0f / Time.unscaledDeltaTime);
    }
    
    public void OnEnable()
    {
        Application.logMessageReceived += Log;
    }
    
    public void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        if(type is LogType.Log or LogType.Warning)return;
        switch (type)
        {
            case LogType.Assert:
                logString = "<color=red>" + logString + "</color>";
                break;
            case LogType.Log:
                logString = "<color=white>" + logString + "</color>";
                break;
            case LogType.Warning:
                logString = "<color=yellow>" + logString + "</color>";
                break;
            case LogType.Exception:
                logString = "<color=red>" + logString + "</color>";
                break;
            case LogType.Error:
                logString = "<color=red>" + logString + "</color>";
                break;
        }
        myLog = myLog + "\n" + logString;
        lineCount++;
        
        if (lineCount > maxLines)
        {
            lineCount--;
            myLog = myLog.Substring(myLog.IndexOf("\n") + 1);
            // myLog = DeleteLine(myLog, 1);
        }
        
        text.text = myLog;
    }

    private string DeleteLine(string s, int i)
    {
        return s.Split(Environment.NewLine.ToCharArray(), i + 1).Skip(i).FirstOrDefault();
    }
}
