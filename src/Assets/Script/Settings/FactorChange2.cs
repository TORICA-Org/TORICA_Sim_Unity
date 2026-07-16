/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactorChange2 : MonoBehaviour
{

    private Slider CurrentSlider;


    // Start is called before the first frame update
    void Start()
    {
        CurrentSlider = GetComponent<Slider>();
        
        if(GameManager.instance.SettingChanged){
            CurrentSlider.value = GameManager.instance.massBackwardRightFactor;
        }else{
            GameManager.instance.massBackwardRightFactor = GameManager.instance.DefaultFactor;
        }
    }


    void Update()
    {
        if(CurrentSlider.value != GameManager.instance.massBackwardRightFactor){
            CurrentSlider.value = GameManager.instance.massBackwardRightFactor;
        }
    }


    public void Method()
    {
        GameManager.instance.massBackwardRightFactor = CurrentSlider.value;
        GameManager.instance.SettingChanged = true;
    }

}
*/
