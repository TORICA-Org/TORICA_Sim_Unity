/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialSend : MonoBehaviour
{
    //SerialHandler.cのクラス
    public SerialHandler serialHandler;

    private AerodynamicCalculator script;//AerodynamicCalculatorスクリプトにアクセスするための変数
    private Rigidbody PlaneRigidbody;
    [System.NonSerialized] public decimal frameNumber;//インターバル管理用、0.02秒に1値が増加する
    [System.NonSerialized] public decimal interval=1m;//リストに追加する間隔[s]

    void Start()
    {
        script = GameManager.instance.Plane.GetComponent<AerodynamicCalculator>();
        PlaneRigidbody = GameManager.instance.Plane.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (frameNumber%(interval/0.02m) == 0 && GameManager.instance.FrameUseable) //interval[s]ごとに送信
        {
            //Debug.Log(script.Airspeed+","+PlaneRigidbody.position.y);
            //serialHandler.Write(script.Airspeed+","+PlaneRigidbody.position.y);
        }
        frameNumber++;//0.02秒経過
    }
}
*/
