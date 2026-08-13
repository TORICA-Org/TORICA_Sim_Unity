using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SettingCloseButton : MonoBehaviour
{
    private GameObject Setting;

    // Start is called before the first frame update
    void Start()
    {
        Setting = GameObject.Find("Setting");
    }

    public void OnClick()
    {
        GameManager.instance.game.status = GameParameters.Status.Flight;
    }

    private void Update()
    {
        if (GameManager.instance.game.status == GameParameters.Status.Flight)
        {
            Setting.SetActive(false);
        }
        else if (GameManager.instance.game.status == GameParameters.Status.Preparation)
        {
            Setting.SetActive(true);
        }
    }
}
