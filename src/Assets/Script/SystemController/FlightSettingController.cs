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
    private bool OnStartTrigger;

    // Start is called before the first frame update

    public void OnEnables()
    {
        FlightSetting = GameObject.Find("FlightSetting");

        FlightSetting.SetActive(true);
    }

    void Start()
    {
        GameManager.instance.game.status = GameParameters.Status.Preparation;
        SaveCsvScript = this.GetComponent<SaveCsvScript>();
    }

    void Update()
    {
        // if(-0.5f >= ((gm.serial.rudderRaw-GameManager.instance.game.JoyStick0)/GameManager.instance.game.JoyStickFactor) && ((gm.serial.rudderRaw-GameManager.instance.game.JoyStick0)/GameManager.instance.game.JoyStickFactor) >= -1.0f && !GameManager.instance.game.EnterFlight){
            // OnStartTrigger = true;
        // }

        if( (Input.GetButtonDown("StartButton") || OnStartTrigger) && GameManager.instance.game.status != GameParameters.Status.Flight){
            GameManager.instance.game.status = GameParameters.Status.Flight;
            FlightSetting.SetActive(false);
            SaveCsvScript.SetFile();
            // OnStartTrigger = false;
        }
    }
}
