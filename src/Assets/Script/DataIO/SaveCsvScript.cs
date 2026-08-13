using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class SaveCsvScript : MonoBehaviour
{
    private StreamWriter sw;
    public Text text;
    bool OnClose;
    //[SerializeField] private FlightSettingCloseButton closeButton;

    DateTime TodayNow;

    public void SetFile()
    {
        if(Config.ExportLog)
        {
            OnClose = false;
            TodayNow = DateTime.Now;
            //string fileName = TodayNow.Year.ToString() + TodayNow.Month.ToString() + TodayNow.Day.ToString() + "-" + DateTime.Now.ToLongTimeString() + "-" + GameManager.instance.game.PlaneName;
            string fileName = TodayNow.ToString("yyyy-MM-dd HH-mm-ss") +" "+ GameManager.instance.game.PlaneName;
            //string fileName = "A";
            //string path = Application.dataPath + "/airdata.csv";
            string path = Application.dataPath + "/" + fileName + ".csv";

            text.text = path;

            sw = new StreamWriter(path, false, Encoding.GetEncoding("UTF-8"));
            string[] s1 = { "Time","Airspeed", "ALT", "Alpha" , "Beta" , "Theta" , "Phi"};
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
        }
    }

    public void SaveData(string txt1, string txt2, string txt3, string txt4, string txt5, string txt6, string txt7)
    {
        string[] s1 = { txt1, txt2, txt3, txt4, txt5, txt6 ,txt7 };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
    }

    void Update()
    {
        if(GameManager.instance.game.status == GameParameters.Status.Splashdown && OnClose == false && Config.ExportLog){
            sw.Close();
            OnClose = true;
        }

        if(Input.GetKeyDown("p") && GameManager.instance.game.status != GameParameters.Status.Flight){
            Config.ExportLog = !Config.ExportLog;
        }

    }
}

