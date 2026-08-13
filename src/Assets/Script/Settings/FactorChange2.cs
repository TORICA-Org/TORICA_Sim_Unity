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
        
        if(GameManager.instance.game.SettingChanged){
            CurrentSlider.value = GameManager.instance.game.massBackwardRightFactor;
        }else{
            GameManager.instance.game.massBackwardRightFactor = GameManager.instance.game.DefaultFactor;
        }
    }


    void Update()
    {
        if(CurrentSlider.value != GameManager.instance.game.massBackwardRightFactor){
            CurrentSlider.value = GameManager.instance.game.massBackwardRightFactor;
        }
    }


    public void Method()
    {
        GameManager.instance.game.massBackwardRightFactor = CurrentSlider.value;
        GameManager.instance.game.SettingChanged = true;
    }

}
*/
