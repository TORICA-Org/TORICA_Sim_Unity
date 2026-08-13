using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ConnectionSettingText : MonoBehaviour
{
    private Text scoreText;
    private AerodynamicParameters aero;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = this.GetComponent<Text>();
        aero = GameManager.instance.aero;

        RefreshText();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshText();
    }

    void RefreshText()
    {
        scoreText.text = "";
        //if(GameManager.instance.FrameUseable){
        //scoreText.text += Math.Round(aero.massRight, 3, MidpointRounding.AwayFromZero) + "\n";
        //scoreText.text += Math.Round(aero.massLeft, 3, MidpointRounding.AwayFromZero) + "\n";
        //scoreText.text += Math.Round(aero.massBackwardRight, 3, MidpointRounding.AwayFromZero) + "\n";
        //scoreText.text += Math.Round(aero.massBackwardLeft, 3, MidpointRounding.AwayFromZero) + "\n\n";
        //scoreText.text += Math.Round(aero.massRight + aero.massLeft, 3, MidpointRounding.AwayFromZero) + Math.Round(aero.massBackwardRight, 2, MidpointRounding.AwayFromZero) + Math.Round(aero.massBackwardLeft, 2, MidpointRounding.AwayFromZero) + "\n\n";
        scoreText.text += "\n\n\n\n\n\n\n";
        scoreText.text += Math.Round(aero.centerOfMass, 3, MidpointRounding.AwayFromZero) + "\n";
        scoreText.text += Math.Round(aero.centerOfMassPilot, 3, MidpointRounding.AwayFromZero) + "\n";
        scoreText.text += Math.Round(aero.dr, 3, MidpointRounding.AwayFromZero) + "\n\n";
        //}
        scoreText.text += GameManager.instance.game.VRMode ? "VRモード" : "非VRモード";
    }
}