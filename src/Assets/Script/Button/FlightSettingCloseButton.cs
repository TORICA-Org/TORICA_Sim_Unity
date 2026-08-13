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
        if (GameManager.instance.game.status == GameParameters.Status.Flight)
        {
            FlightSetting.SetActive(false);
        }
        else if (GameManager.instance.game.status == GameParameters.Status.Preparation)
        {
            FlightSetting.SetActive(true);
        }
    }

    public void OnClick()
    {
        GameManager.instance.game.status = GameParameters.Status.Flight;
        SaveCsvScript.SetFile();
    }
}
