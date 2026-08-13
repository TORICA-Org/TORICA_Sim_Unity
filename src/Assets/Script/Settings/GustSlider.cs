using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GustSlider : MonoBehaviour
{
    private Text scoreText;
    private Slider CurrentSlider;

    // Use this for initialization
    void Start()
    {
        scoreText = GameObject.Find("Gust").GetComponent<Text>();
        CurrentSlider = GetComponent<Slider>();

        if(GameManager.instance.game.SettingChanged){
            CurrentSlider.value = Config.WindSpeed*10f;
        }else{
            Config.WindSpeed = CurrentSlider.value*0.1f;
        }

        scoreText.text = Config.WindSpeed.ToString("0.000");

    }

    // Update is called once per frame
    void Update()
    {
        if(CurrentSlider.value != Config.WindSpeed*10f){
            CurrentSlider.value = Config.WindSpeed*10f;
        }
    }

    public void Method()
    {
        Config.WindSpeed = CurrentSlider.value*0.1f;
        scoreText.text = Config.WindSpeed.ToString("0.000");
        GameManager.instance.game.SettingChanged = true;
    }
}
