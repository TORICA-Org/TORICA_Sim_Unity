/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactorChange3 : MonoBehaviour
{

    private Slider CurrentSlider;


    // Start is called before the first frame update
    void Start()
    {
        CurrentSlider = GetComponent<Slider>();
        
        if(GameManager.instance.SettingChanged){
            CurrentSlider.value = GameManager.instance.massBackwardLeftFactor;
        }else{
            GameManager.instance.massBackwardLeftFactor = GameManager.instance.DefaultFactor;
        }
    }


    void Update()
    {
        if(CurrentSlider.value != GameManager.instance.massBackwardLeftFactor){
            CurrentSlider.value = GameManager.instance.massBackwardLeftFactor;
        }
    }


    public void Method()
    {
        GameManager.instance.massBackwardLeftFactor = CurrentSlider.value;
        GameManager.instance.SettingChanged = true;
    }

}
*/
