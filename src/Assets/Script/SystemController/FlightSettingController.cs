using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class FlightSettingController : MonoBehaviour
{
    private GameObject FlightSetting;
    private SaveCsvScript SaveCsvScript;
    private AerodynamicCalculator script;//AerodynamicCalculatorスクリプトにアクセスするための変数
    private bool OnStartTrigger;
    private GameManager gm = GameManager.instance;

    // Start is called before the first frame update
    
    public void OnEnables()
    {
        FlightSetting = GameObject.Find("FlightSetting");

        GameManager.instance.FlightSettingActive = true;
        FlightSetting.SetActive(GameManager.instance.FlightSettingActive);
                
        Time.timeScale=(float)Convert.ToInt32(!GameManager.instance.FlightSettingActive &!GameManager.instance.SettingActive & !GameManager.instance.Landing);
    }
    
    void Start()
    {
        script = GameManager.instance.Plane.GetComponent<AerodynamicCalculator>();

        SaveCsvScript = this.GetComponent<SaveCsvScript>();
    }

    void Update()
    {
        // if(-0.5f >= ((gm.serial.rudderRaw-GameManager.instance.JoyStick0)/GameManager.instance.JoyStickFactor) && ((gm.serial.rudderRaw-GameManager.instance.JoyStick0)/GameManager.instance.JoyStickFactor) >= -1.0f && !GameManager.instance.EnterFlight){
            // OnStartTrigger = true;
        // }

        if( (Input.GetButtonDown("StartButton") || OnStartTrigger) && !GameManager.instance.EnterFlight){
            GameManager.instance.EnterFlight = true;
            GameManager.instance.FlightSettingActive = !GameManager.instance.FlightSettingActive;
            FlightSetting.SetActive(GameManager.instance.FlightSettingActive);
            Time.timeScale=(float)Convert.ToInt32(!GameManager.instance.FlightSettingActive & !GameManager.instance.Landing);
            SaveCsvScript.SetFile();
            // OnStartTrigger = false;
        }
    }
}
