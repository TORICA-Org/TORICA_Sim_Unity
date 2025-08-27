using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SerialHandler : MonoBehaviour
{
    [SerializeField] private Text text;//通信状況を伝えるテキスト
    [SerializeField] private InputField inputField;//ポート番号入力フィールド
    private bool Connection;
    private bool refresh;
    private bool frameError;

    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived;

    //ポート名
    //例
    //Linuxでは/dev/ttyUSB0
    //windowsではCOM1
    //Macでは/dev/tty.usbmodem1421など
    public static string portName = "COM3";
    public int baudRate    = 115200;

    protected SerialPort serialPort_;
    protected Thread thread_;
    protected bool isRunning_ = false;

    protected string message_;
    protected bool isNewMessageReceived_ = false;

    void Awake()
    {
        Open();
    }

    void Start()
    {
        inputField.text = portName;
        Connection = MyGameManeger.instance.FrameUseable;
    }

    void Update()
    {
        if (isNewMessageReceived_) {
            OnDataReceived(message_);
        }
        isNewMessageReceived_ = false;

        
        if(refresh){
            if(Connection){
                string errorText = "フレーム使用可能、搭乗してください";
                Debug.LogWarning(errorText);
                text.text = errorText;
                MyGameManeger.instance.FrameUseable = true;
                refresh = false;
            }
            else{
                string errorText = "マイコンとの接続、ポート番号を確認して再接続してください";
                Debug.LogWarning(errorText);
                text.text = errorText;

                MyGameManeger.instance.FrameUseable = false;
                refresh = false;
            }
        }

        if(MyGameManeger.instance.FrameUseable != Connection){
            if(!MyGameManeger.instance.FrameUseable){

                string errorText = "センサーに接続されていません。配線を確認して再接続してください";
                Debug.LogWarning(errorText);
                text.text = errorText;

                MyGameManeger.instance.FrameUseable = false;
                refresh = false;
                frameError = true;
            }
            else{
                string errorText = "例外発生:開発者はFrameUseable変数の動向を確認してください";
                Debug.LogWarning(errorText);
                text.text = errorText;
            }
        }

    }

    void OnDestroy()
    {
        Close();
    }

    protected virtual void Open()
    {
         try{
            serialPort_ = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            serialPort_.DtrEnable= true;

            serialPort_.ReadTimeout = 1000;
            //serialPort_.WriteTimeout = 100;
            //または
            //serialPort_ = new SerialPort(portName, baudRate);

            serialPort_.Open();

            isRunning_ = true;

            thread_ = new Thread(Read);
            thread_.Start();
        }
        catch(System.Exception e)
        {
            Debug.LogWarning(e.Message);
            string errorText = "マイコンとの接続、ポート番号を確認して再接続してください";
            Debug.LogWarning(errorText);
            text.text = errorText;

            MyGameManeger.instance.FrameUseable = false;
            refresh = false;
        }
    }

    protected void Close()
    {
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

    protected void Read()
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen) {
            try {
                //message_ = serialPort_.ReadExisting();
                message_ = serialPort_.ReadLine();
                isNewMessageReceived_ = true;
                if(!refresh && !frameError){
                    Connection = true;
                    refresh = true;
                }
            } catch (System.Exception e) {
                Debug.LogWarning(e.Message);
                if(!refresh && !frameError){
                    Connection = false;
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
        text.text = "再設定中";
        Close();
        portName=inputField.text;
        frameError = false;
        refresh = false;
        Open();
        
        //SceneManager.LoadScene("FlightScene");
    }

}
