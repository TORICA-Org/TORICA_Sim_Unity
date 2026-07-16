/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class SerialReceive : MonoBehaviour
{
    // [System.NonSerialized]public float massRightNow;
    // [System.NonSerialized]public float massLeftNow;
    // [System.NonSerialized]public float massBackwardRightNow;
    // [System.NonSerialized]public float massBackwardLeftNow;
    // [System.NonSerialized]public float JoyStickNow;

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


}
*/
