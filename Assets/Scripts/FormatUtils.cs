using UnityEngine;

public static class FormatUtils
{
    public static string FormatNumber(double value)
    {
        if (value < 100000)
            return Mathf.FloorToInt((float)value).ToString();

        return value.ToString("0.###e0");
    }
}
