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
        
        if(GameManager.instance.game.SettingChanged){
            CurrentSlider.value = GameManager.instance.game.massRightFactor;
        }else{
            GameManager.instance.game.massRightFactor = GameManager.instance.game.DefaultFactor;
        }
    }


    void Update()
    {
        if(CurrentSlider.value != GameManager.instance.game.massRightFactor){
            CurrentSlider.value = GameManager.instance.game.massRightFactor;
        }
    }


    public void Method()
    {
        GameManager.instance.game.massRightFactor = CurrentSlider.value;
        GameManager.instance.game.SettingChanged = true;
    }

}
*/
