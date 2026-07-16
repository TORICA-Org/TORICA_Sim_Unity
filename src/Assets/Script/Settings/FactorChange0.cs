/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactorChange0 : MonoBehaviour
{

    private Slider CurrentSlider;


    // Start is called before the first frame update
    void Start()
    {
        CurrentSlider = GetComponent<Slider>();
        
        if(GameManager.instance.SettingChanged){
            CurrentSlider.value = GameManager.instance.massRightFactor;
        }else{
            GameManager.instance.massRightFactor = GameManager.instance.DefaultFactor;
        }
    }


    void Update()
    {
        if(CurrentSlider.value != GameManager.instance.massRightFactor){
            CurrentSlider.value = GameManager.instance.massRightFactor;
        }
    }


    public void Method()
    {
        GameManager.instance.massRightFactor = CurrentSlider.value;
        GameManager.instance.SettingChanged = true;
    }

}
*/
