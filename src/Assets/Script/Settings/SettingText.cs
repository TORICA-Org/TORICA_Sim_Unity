using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingText : MonoBehaviour
{
    private Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = this.GetComponent<Text>();

        RefreshText();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.game.SettingMode == 1){
            RefreshText();
        }
    }

    void RefreshText()
    {
        scoreText.text = "\n";
        scoreText.text += GameManager.instance.game.FlightMode+"\n\n";

        if(Config.MainCamera){
            scoreText.text += "FPS"+"\n\n";
        }else{
            scoreText.text += "TPS"+"\n\n";
        }
        if(Config.ShowHUD){
            scoreText.text += "ON"+"\n\n";
        }else{
            scoreText.text += "OFF"+"\n\n";
        }
        if(Config.UseMousePitchControl){
            scoreText.text += "Mouse"+"\n\n";
        }else{
            scoreText.text += "Keyboard"+"\n\n";
        }
    }
}
