using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TailRotationSlider : MonoBehaviour
{
    private Text scoreText;
    private AerodynamicParameters aero;
    private Slider CurrentSlider;

    // Use this for initialization
    void Start()
    {
        scoreText = GameObject.Find("TailRotation").GetComponent<Text>();
        aero = GameManager.instance.aero;
        CurrentSlider = GetComponent<Slider>();

        if(GameManager.instance.game.SettingChanged){
            CurrentSlider.value = Config.TakeoffPitch;
        }else{
            Config.TakeoffPitch = CurrentSlider.value;
        }

        scoreText.text = Config.TakeoffPitch.ToString("0.000");
    }

    public void Method()
    {
        Config.TakeoffPitch = CurrentSlider.value;
        scoreText.text = Config.TakeoffPitch.ToString("0.000");
        GameManager.instance.game.SettingChanged = true;
        //script.transform.rotation = Quaternion.Euler(0.0f, GameManager.instance.StartRotation, Config.TakeoffPitch);
        aero.Aircraft.transform.rotation = Quaternion.Euler(Config.TakeoffRoll, Config.TakeoffYaw, Config.TakeoffPitch);


    }
}
