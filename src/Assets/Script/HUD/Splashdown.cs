using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Splashdown : MonoBehaviour
{
    private GameObject Result;
    private GameObject SimpleResult;
    //[SerializeField] private Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        // Result = GameObject.Find("Result");
        // SimpleResult = GameObject.Find("SimpleResult");
        //canvas.enabled = false;

        // Result.SetActive(false);
        // SimpleResult.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        GameManager.instance.game.status = GameParameters.Status.Splashdown;
        //GameManager.instance.game.SettingMode = 0; // ?
        //canvas.enabled = true;
        ResultScreen.terminationReason = "着水";
        string sign = Config.WindDirection > 0f ? "R" : Config.WindDirection < 0f ? "L" : "";
        string info = $"WindDirection: {sign}{Config.WindDirection:0.0}deg, WindSpeed: {Config.WindSpeed:0.0}m/s";
        Log.Append($"[Splashdown!] Flew {GameManager.instance.game.Distance:0.00}m in {GameManager.instance.game.timeInFlight:0.0}s! ({info})");
    }

    // Update is called once per frame
    void Update()
    {
        // Result.SetActive(!GameManager.instance.game.SettingActive & GameManager.instance.game.Landing);
        // SimpleResult.SetActive(!GameManager.instance.game.SettingActive & GameManager.instance.game.Landing);
    }
}
