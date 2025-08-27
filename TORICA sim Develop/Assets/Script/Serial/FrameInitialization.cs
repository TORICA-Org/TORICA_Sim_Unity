/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.UI;

//ロードセルの初期値を複数回とって中央値を選ぶことで外れ値でない値を初期値に設定する為のプログラム
public class FrameInitialization : SerialReceive
{
    private float[] massRightList = new float[20];//ロードセルからとった複数回のデータを格納するリスト
    private float[] massLeftList = new float[20];
    private float[] massBackwardRightList = new float[20];
    private float[] massBackwardLeftList = new float[20];
    private float[] JoyStickList = new float[20];
    public Text text;//通信状況を伝えるテキスト
    [SerializeField]private float loadTime = 1f;
    public GameObject startButton;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetZero(0));//開始と共に初期値測定開始
    }

    IEnumerator SetZero(int i)//これを20回実行しデータを測定する iは反復回数
    {
        yield return new WaitForSeconds(loadTime/20);
        if(i<20){
            massRightList[i]=massRightNow;
            massLeftList[i]=massLeftNow;
            massBackwardRightList[i]=massBackwardRightNow;
            massBackwardLeftList[i]=massBackwardLeftNow;
            JoyStickList[i]=JoyStickNow;

            i++;
            StartCoroutine(SetZero(i));
        }
        else{//20回測定を行ったら中央値を保存
                MyGameManeger.instance.massRight0 = findMedian(massRightList);
                MyGameManeger.instance.massLeft0 = findMedian(massLeftList);
                MyGameManeger.instance.massBackwardRight0 = findMedian(massBackwardRightList);
                MyGameManeger.instance.massBackwardLeft0 = findMedian(massBackwardLeftList);
                MyGameManeger.instance.JoyStick0 = findMedian(JoyStickList);
                
                if(MyGameManeger.instance.massRight0 != 0 && MyGameManeger.instance.massLeft0 != 0 && MyGameManeger.instance.massBackwardRight0 != 0 && MyGameManeger.instance.massBackwardLeft0 != 0){
                    MyGameManeger.instance.FrameUseable = true;
                    text.text = "初期設定完了";

                    //オフセットが独自に必要ならコメントアウトする
                
                    MyGameManeger.instance.massRight0 = 0;
                    MyGameManeger.instance.massLeft0 = 0;
                    MyGameManeger.instance.massBackwardRight0 = 0;
                    MyGameManeger.instance.massBackwardLeft0 = 0;
                    
                }
                else{
                    Debug.LogWarning("フレーム使用不可能");
                    text.text = "フレームと接続できませんでした。フレームを使用しない操作に切り替えます。";
                }
                startButton.SetActive(true);
        }
    }
    
    private float findMedian(float[] a)//get 配列a,return 配列の中央値
    {
        Array.Sort(a);
 
        var count = a.Length;
       
        //奇数の場合
        if(count % 2 != 0) 
            return (float)a[count / 2];
     
        //偶数の場合
        return (float)(a[(count - 1) / 2]  + a[count / 2]) / 2.0f;
    }

    public void SetZeroGate()//public IEnumeratorでエラー吐いたので作ったやつ(無知・怠惰)
    {
        StartCoroutine(SetZero(0));
    }
}
*/