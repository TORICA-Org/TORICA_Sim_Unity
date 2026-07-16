using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SerialHandler : IDisposable
{
    public bool Available = false;
    private bool refresh;
    private bool frameError;
    public string status;

    public float massForwardRaw = 0.0f;
    public float massBackwardRaw = 0.0f;
    public float rudderRaw = 0.0f;

    public float rudder = 0.0f;

    public bool holdingPositiveInput = false;
    public bool holdingNegativeInput = false;

    public System.Diagnostics.Stopwatch positiveHoldTime;
    public System.Diagnostics.Stopwatch negativeHoldTime;

    private readonly float LONG_PRESS_THRESHOLD = 1.0f;

    // public delegate void SerialDataReceivedEventHandler(string message);
    // public event SerialDataReceivedEventHandler OnDataReceived;

    //ポート名
    //例
    //Linuxでは/dev/ttyUSB0
    //windowsではCOM1
    //Macでは/dev/tty.usbmodem1421など
    public static string portNamePre = "";
    public int baudRate    = 115200;

    protected SerialPort serialPort_;
    protected Thread thread_;
    protected bool isRunning_ = false;

    protected string message_;
    protected bool isNewMessageReceived_ = false;

    public SerialHandler()
    {
        positiveHoldTime = new();
        negativeHoldTime = new();
        Open();
    }

    public void Update()
    {
        if (portNamePre != Config.SerialPort && Config.SerialPort != "None") {
            SetPort();
            portNamePre = Config.SerialPort;
        }

        float rudderSlope = (1 - 0)/(Config.RudderMax - Config.RudderZero); // 傾き(0~1)/(ラダー変化量)
        if (Config.RudderReverse)
        {
            rudderSlope *= -1; // 傾きを負に反転
        }
        rudder = rudderSlope * (rudderRaw - Config.RudderZero); // ラダー入力の割合(0~1)
        rudder = Mathf.Max(Mathf.Min(rudder, 1.0f), -1.0f);
        
        if (isNewMessageReceived_) {
            OnDataReceived(message_);
            isNewMessageReceived_ = false;
        }

        if(Available){
            status = "フレーム使用可能、搭乗してください";
            Debug.Log(status);
        }
        else{
            status = "マイコンとの接続、ポート番号を確認して再接続してください";
            Debug.LogWarning(status);
        }

        if (!Available) {
            return;
        }

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
        holdingNegativeInput = (negativeHoldTime.ElapsedMilliseconds/1000 >= LONG_PRESS_THRESHOLD);
        // Debug.Log($"{rudder}, {holdingPositiveInput}, {holdingNegativeInput}");

    }

    void IDisposable.Dispose()
    {
        Close();
    }

    protected virtual void Open()
    {
        
        Debug.Log("Opening...");
         try{
            serialPort_ = new SerialPort(Config.SerialPort, baudRate, Parity.None, 8, StopBits.One);
            serialPort_.DtrEnable= true;
            serialPort_.NewLine = "\n"; // 改行コードをLFに指定

            serialPort_.ReadTimeout = 1000;
            //serialPort_.WriteTimeout = 100;
            //または
            //serialPort_ = new SerialPort(portName, baudRate);

            serialPort_.Open();

            isRunning_ = true;

            thread_ = new Thread(Read);
            thread_.Start(); // 別スレッドでの処理を開始
            Available = true;
        }
        catch(System.Exception e)
        {
            Debug.LogWarning(e.Message);
            status = "マイコンとの接続、ポート番号を確認して再接続してください";
            Debug.LogWarning(status);
            Available = false;
        }
    }

    protected void Close()
    {
        Debug.Log("Closing...");
        isNewMessageReceived_ = false;
        isRunning_ = false;

        if (thread_ != null && thread_.IsAlive) {
            thread_.Join();
        }

        if (serialPort_ != null && serialPort_.IsOpen) {
            serialPort_.Close();
            serialPort_.Dispose();
        }
    }

    protected void Read() // 別スレッドで実行される
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen) { // 別スレッドのため問題なし
            try {
                //message_ = serialPort_.ReadExisting();
                message_ = serialPort_.ReadLine();
                message_ = message_.Trim(); // 改行コードが含まれない文字列に（CRLFが来てもいいように）
                // Debug.Log(message_);
                isNewMessageReceived_ = true;
                if(!refresh && !frameError){
                    Available = true;
                    refresh = true;
                }
            } catch (System.Exception e) {
                Debug.LogWarning(e.Message);
                if(!refresh && !frameError){
                    Available = false;
                    refresh = true;
                }
            }
        }

    }

    public void Write(string message)
    {
        try {
            serialPort_.Write(message);
        } catch (System.Exception e) {
            Debug.LogWarning(e.Message);
        }
    }

    public void SetPort()
    {
        status = "再設定中";
        Close();
        Debug.Log("Closed!");
        frameError = false;
        refresh = false;
        Open();
        Debug.Log("Opened!");
        //SceneManager.LoadScene("FlightScene");
    }

    //受信した信号(message)に対する処理
    void OnDataReceived(string message)
    {
        var data = message.Split(new string[] { "\n" }, System.StringSplitOptions.None);
        try
        {
            try{
                //データをリストに書き込む
                // massRightNow = ExtractFromData(data[0],0);
                // massLeftNow = ExtractFromData(data[0],1);
                // massBackwardRightNow = ExtractFromData(data[0],2);
                // massBackwardLeftNow = ExtractFromData(data[0],3);
                // JoyStickNow = ExtractFromData(data[0],4);

                massForwardRaw = ExtractFromData(data[0], 0);
                massBackwardRaw = ExtractFromData(data[0], 1);
                rudderRaw = ExtractFromData(data[0], 2);

                // if (GameManager.instance.FrameUseable && GameManager.instance.JoyStickFirst){//ジョイスティックオフセット取得処理
                //     GameManager.instance.JoyStick0 = JoyStickNow;
                //     GameManager.instance.JoyStickFirst = false;
                // }
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

    float ExtractFromData(string trans_data,int k)//get 受け取った文字列データ k={0:右, 1:左, 2:中央, 3:ジョイスティック},return kに対応する数値(float)
    {
            string[] replaceStrings = Regex.Split(trans_data, @",", RegexOptions.IgnoreCase);
            return float.Parse(replaceStrings[k]);
    }

}
