using System;
using System.IO;
using System.Globalization;
using UnityEngine;

public /*static*/ class EventLogger
{
    public const string FILE_PATH_PREFIX = "C:/Users/yha088/Documents/ExperimentLogs/";//"Assets/Resources/Logs/experiment_log_"
    public const string FILE_PATH_SUFFIX = ".txt";
    public const string DATE_TIME_FILE_FORMAT = "MM-dd-yyyy_hh-mm-ss_tt";
    public const string CULTURE = "en-US";
    public static string filePath = "";
    public static bool initialized = false;

    public static string m_Path = "";

    public void Start()
    {
        //Get the path of the Game data folder
        m_Path = Application.dataPath;

        //Output the Game data path to the console
        Debug.Log("dataPath : " + m_Path);
    }
    public static void Initialize()
    {
        filePath = /*FILE_PATH_PREFIX*/m_Path + DateTime.Now.ToString(DATE_TIME_FILE_FORMAT) + FILE_PATH_SUFFIX;
        initialized = true;
    }

    public static void LogTimer(string tag, string line, TimeSpan ts)
    {
        Log(tag, String.Format("[{0:D3}:{1:D3}] {2}", ts.Seconds, ts.Milliseconds, line)); 
    }

    public static void Log(string tag, string line)
    {
        String output = String.Format("{0}: {1}", tag, line);
        Write(output);
        UnityEngine.Debug.Log(output);
        //UnityEngine.Debug.Log(m_Path);
    }

    private static void Write(string line)
    {
        if (!initialized) return;

        DateTime dateTime = DateTime.Now;
        CultureInfo culture = new CultureInfo(CULTURE);
        using StreamWriter file = new StreamWriter(filePath, append: true);
        file.WriteLine(String.Format("{0}: {1}", dateTime.ToString(culture), line));
        file.Close();
    }
}
