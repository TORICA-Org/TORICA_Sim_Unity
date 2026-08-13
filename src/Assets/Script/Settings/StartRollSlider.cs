using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StartRollSlider : MonoBehaviour
{
    private Text scoreText;
    private AerodynamicParameters aero;
    private Slider CurrentSlider;

    // Use this for initialization
    void Start()
    {
        scoreText = GameObject.Find("StartRoll").GetComponent<Text>();
        aero = GameManager.instance.aero;
        CurrentSlider = GetComponent<Slider>();

        if (GameManager.instance.game.SettingChanged) {
            CurrentSlider.value = Config.TakeoffRoll;
        } else  {
            Config.TakeoffRoll = CurrentSlider.value;
        }

        scoreText.text = Config.TakeoffRoll.ToString("0.000");
    }

    public void Method()
    {
        Config.TakeoffRoll = CurrentSlider.value;
        scoreText.text = Config.TakeoffRoll.ToString("0.000");
        GameManager.instance.game.SettingChanged = true;
        //aero.transform.rotation = Quaternion.Euler(0.0f, GameManager.instance.StartRotation, GameManager.instance.TailRotation);
        aero.Aircraft.transform.rotation = Quaternion.Euler(Config.TakeoffRoll, Config.TakeoffYaw, Config.TakeoffPitch);
    }
}
