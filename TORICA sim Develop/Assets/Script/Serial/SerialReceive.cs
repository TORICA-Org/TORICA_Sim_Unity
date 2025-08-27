using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

//Arudinoからシリアル通信を受けて各種Now変数に格納するプログラム
public class SerialReceive : MonoBehaviour
{
    [System.NonSerialized]public float massRightNow;
    [System.NonSerialized]public float massLeftNow;
    [System.NonSerialized]public float massBackwardRightNow;
    [System.NonSerialized]public float massBackwardLeftNow;
    [System.NonSerialized]public float JoyStickNow;
    private GameObject SystemController;
    //https://qiita.com/yjiro0403/items/54e9518b5624c0030531
    //上記URLのSerialHandler.cのクラス
    
    private SerialHandler serialHandler;

    //private bool isFirst = true;

    // Start is called before the first frame update
    void Awake()
    {
        SystemController = GameObject.Find("SystemController");
        serialHandler = SystemController.GetComponent<SerialHandler>();
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;
    }

    //受信した信号(message)に対する処理
    void OnDataReceived(string message)
    {
        var data = message.Split(new string[] { "\n" }, System.StringSplitOptions.None);
        try
        {
            try{
                //データをリストに書き込む
                massRightNow = ExtractFromData(data[0],0);
                massLeftNow = ExtractFromData(data[0],1);
                massBackwardRightNow = ExtractFromData(data[0],2);
                massBackwardLeftNow = ExtractFromData(data[0],3);
                JoyStickNow = ExtractFromData(data[0],4);

                if(MyGameManeger.instance.FrameUseable && MyGameManeger.instance.JoyStickFirst){//ジョイスティックオフセット取得処理
                    MyGameManeger.instance.JoyStick0 = JoyStickNow;
                    MyGameManeger.instance.JoyStickFirst = false;
                }
                //Debug.Log(massRightNow+","+massLeftNow+","+massBackwardRightNow+","+massBackwardLeftNow);
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
