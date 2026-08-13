using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class AirData : MonoBehaviour
{
    private GameManager gm;

    private AerodynamicParameters aero;//AerodynamicCalculatorスクリプトにアクセスするための変数
    private Rigidbody PlaneRigidbody;
    [System.NonSerialized] public decimal frameNumber;//インターバル管理用、0.02秒に1値が増加する
    [System.NonSerialized] public decimal interval = 0.1m;//リストに追加する間隔[s]
    [System.NonSerialized] public int ListNumber;//リストの要素番号

    private float time;
    private StreamWriter sw;

    SaveCsvScript SaveCsvScript;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        aero = GameManager.instance.aero;
        PlaneRigidbody = GameManager.instance.game.Plane.GetComponent<Rigidbody>();

        GameManager.instance.game.AirspeedList.Clear();
        GameManager.instance.game.AltList.Clear();
        GameManager.instance.game.AlphaList.Clear();
        GameManager.instance.game.BetaList.Clear();
        GameManager.instance.game.ThetaList.Clear();
        GameManager.instance.game.PhiList.Clear();
        GameManager.instance.game.CenterOfGList.Clear();
        GameManager.instance.game.drList.Clear();
        gm.game.Distance = 0.0f;

        SaveCsvScript = this.GetComponent<SaveCsvScript>();
    }

    // Update is called once per frame
    void FixedUpdate()//0.02秒ごとに実行
    {
        time += Time.deltaTime;

        // Calculate rotation
        float q1 = GameManager.instance.game.Plane.transform.rotation.x;
        float q2 = -GameManager.instance.game.Plane.transform.rotation.y;
        float q3 = -GameManager.instance.game.Plane.transform.rotation.z;
        float q4 = GameManager.instance.game.Plane.transform.rotation.w;
        float C11 = q1 * q1 - q2 * q2 - q3 * q3 + q4 * q4;
        float C22 = -q1 * q1 + q2 * q2 - q3 * q3 + q4 * q4;
        float C12 = 2f * (q1 * q2 + q3 * q4);
        float C13 = 2f * (q1 * q3 - q2 * q4);
        float C32 = 2f * (q2 * q3 - q1 * q4);
        float phi = -Mathf.Atan(-C32 / C22) * Mathf.Rad2Deg;
        float theta = -Mathf.Asin(C12) * Mathf.Rad2Deg;
        float psi = -Mathf.Atan(-C13 / C11) * Mathf.Rad2Deg;

        //リストに追加
        GameManager.instance.game.AirspeedList.Add((float)Math.Round(aero.Airspeed, 2, MidpointRounding.AwayFromZero));
        GameManager.instance.game.AltList.Add((float)Math.Round(aero.ALT, 2, MidpointRounding.AwayFromZero));
        GameManager.instance.game.AlphaList.Add((float)Math.Round(aero.alpha, 2, MidpointRounding.AwayFromZero));
        GameManager.instance.game.BetaList.Add((float)Math.Round(aero.beta, 2, MidpointRounding.AwayFromZero));
        GameManager.instance.game.ThetaList.Add((float)Math.Round(theta, 2, MidpointRounding.AwayFromZero));
        GameManager.instance.game.PhiList.Add((float)Math.Round(phi, 2, MidpointRounding.AwayFromZero));
        GameManager.instance.game.CenterOfGList.Add((float)Math.Round(aero.centerOfMass, 2, MidpointRounding.AwayFromZero));
        GameManager.instance.game.drList.Add((float)Math.Round(aero.dr, 2, MidpointRounding.AwayFromZero));

        if (PlaneRigidbody != null)
        {
            gm.game.Distance = (PlaneRigidbody.position - gm.game.PlatformPosition).magnitude;
        }

        /*
        GameManager.instance.game.AirspeedList.Add(RoundFloat(script.Airspeed, 2));
        GameManager.instance.game.AltList.Add(RoundFloat(script.ALT, 2));
        GameManager.instance.game.AlphaList.Add(RoundFloat(script.alpha, 2));
        GameManager.instance.game.BetaList.Add(RoundFloat(script.beta, 2));
        GameManager.instance.game.ThetaList.Add(RoundFloat(theta, 2));
        GameManager.instance.game.PhiList.Add(RoundFloat(phi, 2));
        */

        ListNumber++;//リストの要素番号を進める

        if (frameNumber % (interval / 0.02m) == 0)//interval[s]ごとにログに追加
        {
            if (Config.ExportLog && GameManager.instance.game.status == GameParameters.Status.Flight)
            {
                SaveCsvScript.SaveData(time.ToString("F1"), aero.Airspeed.ToString("F3"), aero.ALT.ToString("F3"), aero.alpha.ToString("F3"), aero.beta.ToString("F3"), theta.ToString("F3"), phi.ToString("F3"));
            }
        }
        frameNumber++;//0.02秒経過
    }

    /*
    float RoundFloat(float value, int decimals)
    {
        float factor = Mathf.Pow(10f, decimals);
        // 100 倍して四捨五入 → また 100 で割る
        return Mathf.Round(value * factor) / factor;
    }
    */

    /*
    void OnGUI()
    {
        if (GUILayout.Button("Show Log"))
        {
            int i = 0;
            while(true)
            {
                try
                {
                    float speed = GameManager.instance.game.AirspeedList[i];
                    float alt = GameManager.instance.game.AltList[i];
                    float alpha = GameManager.instance.game.AlphaList[i];
                    float beta = GameManager.instance.game.BetaList[i];
                    float theta = GameManager.instance.game.ThetaList[i]; ;
                    float phi = GameManager.instance.game.PhiList[i];
                    float centerOfG = GameManager.instance.game.CenterOfGList[i];
                    float dr = GameManager.instance.game.drList[i];
                    Debug.Log("speed: " + speed + ", alt: " + alt + ", alpha: " + alpha + ", beta: " + beta + ", theta: " + theta + ", phi: " + phi + ", centerOfG: " + centerOfG + ", dr: " + dr);
                    i++;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(e.Message);
                    Debug.Log(i);
                    break;
                }
            }
        }
    }
    */
}
