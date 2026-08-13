using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MouseSensitivitySlider : MonoBehaviour
{
    private Slider CurrentSlider;

    // Use this for initialization
    void Start()
    {
        CurrentSlider = GetComponent<Slider>();

        if(GameManager.instance.game.SettingChanged){
            CurrentSlider.value = Config.MouseSensitivity*10f;
        }else{
            Config.MouseSensitivity = CurrentSlider.value/10f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Method()
    {
        Config.MouseSensitivity = CurrentSlider.value/10f;
        GameManager.instance.game.SettingChanged = true;
    }
}
