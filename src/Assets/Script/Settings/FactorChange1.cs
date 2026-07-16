/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactorChange1 : MonoBehaviour
{

    private Slider CurrentSlider;


    // Start is called before the first frame update
    void Start()
    {
        CurrentSlider = GetComponent<Slider>();
        
        if(GameManager.instance.SettingChanged){
            CurrentSlider.value = GameManager.instance.massLeftFactor;
        }else{
            GameManager.instance.massLeftFactor = GameManager.instance.DefaultFactor;
        }
    }


    void Update()
    {
        if(CurrentSlider.value != GameManager.instance.massLeftFactor){
            CurrentSlider.value = GameManager.instance.massLeftFactor;
        }
    }


    public void Method()
    {
        GameManager.instance.massLeftFactor = CurrentSlider.value;
        GameManager.instance.SettingChanged = true;
    }

}
*/
