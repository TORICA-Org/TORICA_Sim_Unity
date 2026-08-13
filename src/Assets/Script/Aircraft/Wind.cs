using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Wind
{
    private static float direction = 0.0f;
    private static float magnitude = 0.0f;
    private static float lastRangeStart = 0.0f;
    private static float lastRangeEnd = 0.0f;
    private static string loadedRangeSpec = "";

    private static void ParseRangeSpec(float distance)
    {
        if (Config.WindRangeSpec == loadedRangeSpec) // 風の範囲指定が変更されていない.
        {
            if (distance > lastRangeStart && distance < lastRangeEnd) return; // 境界値を超えていない．
        }
        else // 風の範囲指定が変更された.
        {
            loadedRangeSpec = Config.WindRangeSpec;
        }

        if (loadedRangeSpec == "None")
        {
            direction = Config.WindDirection;
            magnitude = Config.WindSpeed;
            return;
        }


        // 風の範囲指定の例: "2:90@..100; 3:90@100.."
        string[] specs = loadedRangeSpec.Split(';');
        foreach (string spec in specs)        {
            string[] parts = spec.Split('@');
            if (parts.Length != 2) continue; // フォーマットエラー

            string[] windParts = parts[0].Split(':');
            if (windParts.Length != 2) continue; // フォーマットエラー

            if (!float.TryParse(windParts[0], out float specMagnitude)) continue; // 数値変換エラー

            if (!float.TryParse(windParts[1], out float specDirection)) continue; // 数値変換エラー

            string[] rangeParts = parts[1].Split(new string[] { ".." }, System.StringSplitOptions.None);
            if (rangeParts.Length != 2) continue; // フォーマットエラー

            float rangeStart = float.NegativeInfinity;
            float rangeEnd = float.PositiveInfinity;

            if (rangeParts[0] != "")
            {
                if (!float.TryParse(rangeParts[0], out rangeStart)) continue; // 数値変換エラー
            }
            if (rangeParts[1] != "")
            {
                if (!float.TryParse(rangeParts[1], out rangeEnd)) continue; // 数値変換エラー
            }
            if (distance >= rangeStart && distance < rangeEnd)
            {
                direction = specDirection;
                magnitude = specMagnitude;
                lastRangeStart = rangeStart;
                lastRangeEnd = rangeEnd;
                Debug.Log($"Wind setted. dir: {direction}, mag: {magnitude}, start: {lastRangeStart}, end: {lastRangeEnd}");
                return;
            }
        }
    }

    public static float GetDirection(float distance)
    {
        ParseRangeSpec(distance);
        return direction;
    }

    public static float GetMagnitude(float distance)
    {
        ParseRangeSpec(distance);
        return magnitude;
    }
}
