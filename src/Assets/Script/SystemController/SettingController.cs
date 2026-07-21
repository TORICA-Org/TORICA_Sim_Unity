using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SettingController : MonoBehaviour
{
    private GameObject Setting;
    private Slider TakeoffVelocitySlider;

    // Start is called before the first frame update
    public void OnEnables()
    {
        Setting = GameObject.Find("Setting");

        GameManager.instance.SettingActive = false;
        Setting.SetActive(GameManager.instance.SettingActive);
    }

    void Start(){
        //Time.timeScale=(float)Convert.ToInt32(!GameManager.instance.SettingActive & !GameManager.instance.Landing);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("tab") && !GameManager.instance.FlightSettingActive && !GameManager.instance.Landing){
            GameManager.instance.SettingActive = !GameManager.instance.SettingActive;
            Setting.SetActive(GameManager.instance.SettingActive);
            Time.timeScale=(float)Convert.ToInt32(!GameManager.instance.FlightSettingActive & !GameManager.instance.SettingActive & !GameManager.instance.Landing);
        }
        // if(Input.GetKeyDown("c")){
        //     Config.UseMousePitchControl = !Config.UseMousePitchControl;
        // }
    }
}
