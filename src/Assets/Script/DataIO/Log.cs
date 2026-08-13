using UnityEngine;
using System;
using System.IO; // ファイル入出力

static public class Log
{
    // `Log.txt`を保存する`Application.dataPath`へのフルパス.
    private static string _sourceDirectory = Directory.GetParent(Application.dataPath).FullName;
    // `Log.txt`の名前.
    private static string _fileName = "Log.txt";
    // `Log.txt`のフルパスを構築.
    private static string _fullPath = Path.Combine(_sourceDirectory, _fileName);

    static public void Append(string message)
    {
        string logMessage = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} - {message}";
        logMessage += Environment.NewLine;
        try
        {
            File.AppendAllText(_fullPath, logMessage); // 全てのstringを書き込む（エンコードはUTF-8）
            // サンプルコード -> https://learn.microsoft.com/ja-jp/dotnet/standard/io/how-to-write-text-to-a-file#example-write-and-append-text-with-the-file-class
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error flushing Log: " + e);
            return;
        }
        Debug.Log("Log flushed: " + logMessage);
    }
}
