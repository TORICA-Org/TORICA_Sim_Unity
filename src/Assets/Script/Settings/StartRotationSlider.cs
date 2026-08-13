using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StartRotationSlider : MonoBehaviour
{
    private Text scoreText;
    private AerodynamicParameters aero;
    private Slider CurrentSlider;

    // Use this for initialization
    void Start()
    {
        scoreText = GameObject.Find("StartRotation").GetComponent<Text>();
        aero = GameManager.instance.aero;
        CurrentSlider = GetComponent<Slider>();

        if(GameManager.instance.game.SettingChanged){
            CurrentSlider.value = Config.TakeoffYaw;
        }else{
            Config.TakeoffYaw = CurrentSlider.value;
        }

        scoreText.text = Config.TakeoffYaw.ToString("0.000");
    }

    public void Method()
    {
        Config.TakeoffYaw = CurrentSlider.value;
        scoreText.text = Config.TakeoffYaw.ToString("0.000");
        GameManager.instance.game.SettingChanged = true;
        //aero.transform.rotation = Quaternion.Euler(0.0f, Config.TakeoffYaw, Config.TakeoffPitch);
        aero.Aircraft.transform.rotation = Quaternion.Euler(Config.TakeoffRoll, Config.TakeoffYaw, Config.TakeoffPitch);
    }
}
