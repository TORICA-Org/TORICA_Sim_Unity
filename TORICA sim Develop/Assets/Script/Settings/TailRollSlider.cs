using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TailRollSlider: MonoBehaviour
{
    private Text scoreText;
    private AerodynamicCalculator script;
    private Slider CurrentSlider;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GameObject.Find("TailRoll").GetComponent<Text>();
        script = MyGameManeger.instance.Plane.GetComponent<AerodynamicCalculator>();
        CurrentSlider = GetComponent<Slider>();

        if (MyGameManeger.instance.SettingChanged)
        {
            CurrentSlider.value = MyGameManeger.instance.TailRoll;
        }
        else
        {
            MyGameManeger.instance.TailRoll = CurrentSlider.value;
        }

        scoreText.text = MyGameManeger.instance.TailRoll.ToString("0.000");
    }

    // Update is called once per frame
    void Update()
    {
        MyGameManeger.instance.TailRoll = CurrentSlider.value;
        scoreText.text = MyGameManeger.instance.TailRoll.ToString("0.000");
        MyGameManeger.instance.SettingChanged = true;
        script.transform.rotation = Quaternion.Euler(MyGameManeger.instance.TailRoll, MyGameManeger.instance.StartRotation, MyGameManeger.instance.TailRotation);
    }
}
