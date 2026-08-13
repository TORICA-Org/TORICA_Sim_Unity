using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWindController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(Config.RandomizeWind){
            Config.WindSpeed = Random.Range(0,60)*0.1f;
            Config.WindDirection =Random.Range(-12,12)*15f;
            GameManager.instance.game.SettingChanged = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("k")){
            Config.RandomizeWind = !Config.RandomizeWind;
            if(Config.RandomizeWind){
                Config.WindSpeed = Random.Range(0,60)*0.1f;
                Config.WindDirection =Random.Range(-12,12)*15f;
                GameManager.instance.game.SettingChanged = true;
            }
        }
    }
}
