using System;
using System.IO;
using System.Globalization;

public static class EventLogger
{
    public const string FILE_PATH = "Assets/Resources/Logs/test_log.txt";
    public const string CULTURE = "en-US";

    public static void Write(string line)
    {
        DateTime dateTime = DateTime.Now;
        CultureInfo culture = new CultureInfo(CULTURE);
        using StreamWriter file = new StreamWriter(FILE_PATH, append: true);
        file.WriteLine(dateTime.ToString(culture) + ": " + line);
        file.Close();
    }
}
