using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TailSetDegSlider : MonoBehaviour
{
    private Text scoreText;
    private AerodynamicCalculator script;
    private Slider CurrentSlider;

    // Use this for initialization
    void Start()
    {
        scoreText = GameObject.Find("TailSetDeg").GetComponent<Text>();
        script = MyGameManeger.instance.Plane.GetComponent<AerodynamicCalculator>();
        CurrentSlider = GetComponent<Slider>();

        if(MyGameManeger.instance.SettingChanged){
            CurrentSlider.value = MyGameManeger.instance.TailSetDeg;
        }else{
            MyGameManeger.instance.TailSetDeg = CurrentSlider.value;
        }
        
        scoreText.text = MyGameManeger.instance.TailSetDeg.ToString("0.000");
    }

    public void Method()
    {
        CurrentSlider.value = Mathf.Round(CurrentSlider.value / 0.5f) * 0.5f;

        MyGameManeger.instance.TailSetDeg = CurrentSlider.value;
        scoreText.text = MyGameManeger.instance.TailSetDeg.ToString("0.000");
        MyGameManeger.instance.SettingChanged = true;
    }
}
