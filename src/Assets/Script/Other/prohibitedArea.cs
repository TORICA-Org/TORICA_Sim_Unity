using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class prohibitedArea : MonoBehaviour
{
    /*
    private GameObject Result;

    //[SerializeField] private Canvas canvas;
    // Start is called before the first frame update
    private void Start()
    {
        Result = GameObject.Find("Result");
    }
    */
    void Start()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        GameManager.instance.game.status = GameParameters.Status.Splashdown;
        //GameManager.instance.game.SettingMode = 0;
        //canvas.enabled = true;
        ResultScreen.terminationReason = "飛行禁止区域への進入";
    }

    // Update is called once per frame

    void Update()
    {
        //Result.SetActive(/*!GameManager.instance.game.SettingActive & */GameManager.instance.game.Landing);
    }
}