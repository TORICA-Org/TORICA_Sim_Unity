using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

static public class SerialHandler
{
    static public bool Available = false;
    static public string status = "";

    static public float massForwardRaw = 0.0f;
    static public float massBackwardRaw = 0.0f;
    static public float rudderRaw = 0.0f;

    static public float rudder = 0.0f;

    static public bool holdingPositiveInput = false;
    static public bool holdingNegativeInput = false;

    public delegate void SerialEvent();
    static public event SerialEvent OnHoldingPositive;
    static public event SerialEvent OnHoldingNegative;

    static public System.Diagnostics.Stopwatch positiveHoldTime;
    static public System.Diagnostics.Stopwatch negativeHoldTime;

    static private readonly float LONG_PRESS_THRESHOLD = 1.0f;

    //ポート名
    //例
    //Linuxでは/dev/ttyUSB0
    //windowsではCOM1
    //Macでは/dev/tty.usbmodem1421など
    static private int baudRate = 115200;

    static private SerialPort serialPort_;
    static public List<string> serialPortsList;
    static public string serialPortsString = "";
    static private Thread thread_;
    static private bool isRunning_ = true;

    static private string message_;

    static private SynchronizationContext context = SynchronizationContext.Current;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] // ゲームのシーンがロードされる前にこのメソッドを呼び出すための属性.
    static private void Initialize()
    {
        serialPortsList = new();
        positiveHoldTime = new();
        negativeHoldTime = new();
        thread_ = new Thread(Loop);
        thread_.Start(); // 別スレッドでの処理を開始
        Application.quitting += Quit;
    }

    static private void Loop() // 別スレッドでの処理
    {
        while (isRunning_)
        {
            string serialPortsStringPre = serialPortsString;
            try
            {
                serialPortsList = SerialPort.GetPortNames().ToList(); // シリアルポートのリストを生成
                serialPortsString = string.Join(", ", serialPortsList);
            }
            catch(System.Exception e)
            {
                Debug.LogWarning(e.Message);
                serialPortsList.Clear();
            }
            if (string.IsNullOrWhiteSpace(serialPortsString))
            {
                serialPortsString = "None";
            }
            if (!string.Equals(serialPortsString, serialPortsStringPre))
            {
                Config.Flush();
            }

            if (serialPort_ != null && serialPort_.IsOpen && Available) // シリアルポート接続中
            {
                try {
                    message_ = serialPort_.ReadLine();
                    message_ = message_.Trim(); // 改行コードが含まれない文字列に（CRLFが来てもいいように）
                    // Debug.Log(message_);
                    OnDataReceived(message_);
                    MapRudder();
                    InvokeEvents();
                    Available = true;
                    status = "マイコンとの通信に成功しました．";
                } catch (System.Exception e) {
                    Debug.LogWarning(e.Message);
                    Available = false;
                    status = "マイコンとの通信に失敗しました．";
                }
            }
            else // シリアルポート未接続
            {
                Available = Open();
            }
        }
    }

    static private bool Open()
    {
        // Debug.Log("Opening...");
        if (string.Equals(Config.SerialPort, "None"))
        {
            // Debug.Log($"No SerialPort selected.({serialPortsString})");
            status = $"未接続（候補：{serialPortsString}）";
            return false;
        }
        try{
            serialPort_ = new SerialPort(Config.SerialPort, baudRate, Parity.None, 8, StopBits.One);
            serialPort_.DtrEnable= true;
            serialPort_.NewLine = "\n"; // 改行コードをLFに指定

            serialPort_.ReadTimeout = 100;
            //serialPort_.WriteTimeout = 100;
            //または
            //serialPort_ = new SerialPort(portName, baudRate);

            serialPort_.Open();

            status = "マイコンとの接続に成功しました．";
            return true;
        }
        catch(System.Exception e)
        {
            Debug.LogWarning(e.Message);
            status = "マイコンとの接続に失敗しました．再接続してください．";
            Debug.LogWarning(status);
            return false;
        }
    }

    static public bool Reload()
    {
        status = "再設定中";
        Close();
        Debug.Log("Closed!");
        Available = Open();
        Debug.Log("Opened!");
        return Available;
    }

    static private void MapRudder()
    {
        float rudderSlope = (1 - 0)/(Config.RudderMax - Config.RudderZero); // 傾き(0~1)/(ラダー変化量)
        if (Config.RudderReverse)
        {
            rudderSlope *= -1; // 傾きを負に反転
        }
        rudder = rudderSlope * (rudderRaw - Config.RudderZero); // ラダー入力の割合(0~1)
        rudder = Mathf.Max(Mathf.Min(rudder, 1.0f), -1.0f);
    }

    static private void InvokeEvents()
    {
        if (0.5f <= rudder) // 長押し判定
        {
            if (!positiveHoldTime.IsRunning)
            {
                positiveHoldTime.Start(); // 時間を加算
            }
            // Debug.Log($"positive: {positiveHoldTime.ElapsedMilliseconds/1000}");
        }
        else
        {
            positiveHoldTime.Reset(); // 離されたらタイマーをリセット
        }

        if (rudder <= -0.5f) // 長押し判定
        {
            if (!negativeHoldTime.IsRunning)
            {
                negativeHoldTime.Start(); // 時間を加算
            }
            // Debug.Log($"negative: {negativeHoldTime.ElapsedMilliseconds/1000}");
        }
        else
        {
            negativeHoldTime.Reset(); // 離されたらタイマーをリセット
        }

        holdingPositiveInput = (positiveHoldTime.ElapsedMilliseconds/1000 >= LONG_PRESS_THRESHOLD);
        if (holdingPositiveInput)
        {
            context.Post(
                (_) =>
                {
                    OnHoldingPositive?.Invoke();
                },
                null
            );
        }

        holdingNegativeInput = (negativeHoldTime.ElapsedMilliseconds/1000 >= LONG_PRESS_THRESHOLD);
        if (holdingNegativeInput)
        {
            context.Post(
                (_) =>
                {
                    OnHoldingNegative?.Invoke();
                },
                null
            );
        }
        
    }



    static private void Write(string message)
    {
        try {
            serialPort_.Write(message);
        } catch (System.Exception e) {
            Debug.LogWarning(e.Message);
        }
    }

    //受信した信号(message)に対する処理
    static private void OnDataReceived(string message)
    {
        var data = message.Split(new string[] { "\n" }, System.StringSplitOptions.None);
        try
        {
            try{
                //データをリストに書き込む
                massForwardRaw = ExtractFromData(data[0], 0);
                massBackwardRaw = ExtractFromData(data[0], 1);
                rudderRaw = ExtractFromData(data[0], 2);
                // Debug.Log(massForwardRaw+","+massBackwardRaw+","+rudderRaw);
            }
            catch(System.Exception e)//シリアル通信が不正の場合
            {
                Debug.LogWarning(e.Message);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);//シリアル通信がタイムアウトした場合
        }
    }

    static float ExtractFromData(string trans_data,int k) // カンマ区切りで文字列を分解
    {
            string[] replaceStrings = Regex.Split(trans_data, @",", RegexOptions.IgnoreCase);
            return float.Parse(replaceStrings[k]);
    }

    static public void Close()
    {
        // Debug.Log("Closing...");

        if (thread_ != null && thread_.IsAlive) {
            thread_.Join(200);
        }

        if (serialPort_ != null && serialPort_.IsOpen) {
            serialPort_.Close();
            serialPort_.Dispose();
        }
        Available = false;
    }

    static private void Quit() // 終了処理
    {
        isRunning_ = false;
        Close();
    }
}
