using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StartMethod : MonoBehaviour　// SystemControllerにアタッチ
{
    [SerializeField] private ChangeAircraft changeAircraft;
    [SerializeField] private FlightSettingController flightSettingController;
    [SerializeField] private SettingController settingController;
    [SerializeField] private ModelInstantiater modelInstantiater;
    private GameObject PlaneParent;

    void OnEnable(){
        flightSettingController.OnEnables();

        settingController.OnEnables();

        modelInstantiater.OnEnables();

        changeAircraft.OnEnables();
    }
}
