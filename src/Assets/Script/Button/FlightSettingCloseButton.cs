using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class FlightSettingCloseButton : MonoBehaviour
{
    private GameObject FlightSetting;
    //[System.NonSerialized] public bool firstPush = false;
    [SerializeField] private SaveCsvScript SaveCsvScript;

    // Start is called before the first frame update
    void Start()
    {
        FlightSetting = GameObject.Find("FlightSetting");
    }

    void Update()
    {

    }

    public void OnClick()
    {
        Debug.Log("FlightSettingCloseButton clicked!");
        Debug.Log($"EnterFlight: {GameManager.instance.EnterFlight}");
        if (!GameManager.instance.EnterFlight)
        {
            GameManager.instance.EnterFlight = true;
            //firstPush = true;
            GameManager.instance.FlightSettingActive = !GameManager.instance.FlightSettingActive;
            FlightSetting.SetActive(GameManager.instance.FlightSettingActive);
            Time.timeScale=(float)Convert.ToInt32(!GameManager.instance.FlightSettingActive & !GameManager.instance.Landing);
            SaveCsvScript.SetFile();
        }
    }
}
