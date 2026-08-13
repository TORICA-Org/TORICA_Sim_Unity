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
        
        if(GameManager.instance.game.SettingChanged){
            CurrentSlider.value = GameManager.instance.game.massLeftFactor;
        }else{
            GameManager.instance.game.massLeftFactor = GameManager.instance.game.DefaultFactor;
        }
    }


    void Update()
    {
        if(CurrentSlider.value != GameManager.instance.game.massLeftFactor){
            CurrentSlider.value = GameManager.instance.game.massLeftFactor;
        }
    }


    public void Method()
    {
        GameManager.instance.game.massLeftFactor = CurrentSlider.value;
        GameManager.instance.game.SettingChanged = true;
    }

}
*/
