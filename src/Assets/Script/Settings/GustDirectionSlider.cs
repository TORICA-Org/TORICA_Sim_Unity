using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GustDirectionSlider : MonoBehaviour
{
    private Text scoreText;
    private Slider CurrentSlider;
    private GameManager gm;

    // Use this for initialization
    void Start()
    {
        gm = GameManager.instance;

        scoreText = GameObject.Find("GustDirection").GetComponent<Text>();
        CurrentSlider = GetComponent<Slider>();

        //Debug.Log(gm.game.SettingChanged);
        if(gm.game.SettingChanged){
            CurrentSlider.value = Config.WindDirection/15f;
        }else{
            Config.WindDirection = CurrentSlider.value*15f;
        }

        Method();

    }

    // Update is called once per frame
    void Update()
    {
        if(CurrentSlider.value != Config.WindDirection/15f)
        {
            CurrentSlider.value = Config.WindDirection/15f;
        }
        //Debug.Log(Config.GustDirection);
    }

    public void Method()
    {
        Config.WindDirection = CurrentSlider.value*15f;

        string DirectionText = "";
        if (Config.WindDirection > 0)
        {
          DirectionText = "R ";
        }
        else if (Config.WindDirection < 0)
        {
          DirectionText = "L ";
        }
        scoreText.text = DirectionText + Mathf.Abs(Config.WindDirection).ToString("0");

        gm.game.SettingChanged = true;
    }
}
