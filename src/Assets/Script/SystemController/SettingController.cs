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
        GameManager.instance.game.status = GameParameters.Status.Preparation;
        Setting.SetActive(false);
    }

    void Start(){
        //Time.timeScale=(float)Convert.ToInt32(!GameManager.instance.game.SettingActive & !GameManager.instance.game.Landing);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("tab") && GameManager.instance.game.status != GameParameters.Status.Flight){
            GameManager.instance.game.status = GameParameters.Status.Flight;
            Setting.SetActive(true);
        }
        // if(Input.GetKeyDown("c")){
        //     Config.UseMousePitchControl = !Config.UseMousePitchControl;
        // }
    }
}
