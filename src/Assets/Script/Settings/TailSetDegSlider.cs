using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TailSetDegSlider : MonoBehaviour
{
    private Text scoreText;
    private Slider CurrentSlider;

    // Use this for initialization
    void Start()
    {
        scoreText = GameObject.Find("TailSetDeg").GetComponent<Text>();
        CurrentSlider = GetComponent<Slider>();

        if(GameManager.instance.game.SettingChanged){
            CurrentSlider.value = GameManager.instance.game.TailSetDeg;
        }else{
            GameManager.instance.game.TailSetDeg = CurrentSlider.value;
        }

        scoreText.text = GameManager.instance.game.TailSetDeg.ToString("0.000");
    }

    public void Method()
    {
        CurrentSlider.value = Mathf.Round(CurrentSlider.value / 0.5f) * 0.5f;

        GameManager.instance.game.TailSetDeg = CurrentSlider.value;
        scoreText.text = GameManager.instance.game.TailSetDeg.ToString("0.000");
        GameManager.instance.game.SettingChanged = true;
    }
}
