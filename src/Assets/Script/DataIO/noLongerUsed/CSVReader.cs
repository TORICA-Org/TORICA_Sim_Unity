/*
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.UI;
using System.Text;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    private string path;
    private string fileName = "data.csv";
    public Text text;
    public static List<List<string>> CsvList = new List<List<string>>();

    void Start() {
        path = Application.dataPath + "/" + fileName;
        Debug.Log("File path: " + path);
        ReadFile();
    }

    void WriteFile(string txt) {
        FileInfo fi = new FileInfo(path);
        using (StreamWriter sw = fi.AppendText()) {
            sw.WriteLine(txt);
        }
    }

    void ReadFile() {
        path = Application.dataPath + "/" + fileName;

        if (!File.Exists(path)) {
            Debug.LogWarning("CSV file not found at: " + path);
            return;
        }

        FileInfo fi = new FileInfo(path);
        try {
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8)) {
                string readTxt = sr.ReadToEnd();
                Debug.Log("Read CSV content:\n" + readTxt);

                if (text != null) {
                    text.text = readTxt;
                } else {
                    Debug.LogWarning("UI Text component is not assigned.");
                }

                CsvList.Clear(); // データリストの初期化

                string[] lines = readTxt.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines) {
                    string[] values = line.Split(',');
                    CsvList.Add(new List<string>(values)); // 二次元リストとして格納
                }

                // デバッグ用ログ出力
                //for (int i = 0; i < CsvList.Count; i++) {
                //    Debug.Log($"Row {i}: " + string.Join(", ", CsvList[i]));
                //}
            }
        } catch (Exception e) {
            Debug.LogError("Error reading CSV: " + e);
        }
    }
}
*/