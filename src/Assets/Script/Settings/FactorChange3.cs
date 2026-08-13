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
        
        if(GameManager.instance.game.SettingChanged){
            CurrentSlider.value = GameManager.instance.game.massBackwardLeftFactor;
        }else{
            GameManager.instance.game.massBackwardLeftFactor = GameManager.instance.game.DefaultFactor;
        }
    }


    void Update()
    {
        if(CurrentSlider.value != GameManager.instance.game.massBackwardLeftFactor){
            CurrentSlider.value = GameManager.instance.game.massBackwardLeftFactor;
        }
    }


    public void Method()
    {
        GameManager.instance.game.massBackwardLeftFactor = CurrentSlider.value;
        GameManager.instance.game.SettingChanged = true;
    }

}
*/
