using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneController : MonoBehaviour
{
    private GameObject Platform;
    private GameManager gm = GameManager.instance;
    // Start is called before the first frame update
    void Start()
    {
        Platform = GameObject.Find("Platform");

        if(GameManager.instance.game.FlightMode == "TestFlight"){
            Platform.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // "ResetButton"は Input Manager に設定されている.
        if(Input.GetMouseButton(2) || Input.GetButtonDown("ResetButton")){
            Time.timeScale=1f;
            GameManager.instance.game.status = GameParameters.Status.Preparation;
            GameManager.instance.game.SettingMode = 0;
            SceneManager.LoadScene("FlightScene");
        }

        if(Input.GetKeyDown("m")){
            if(GameManager.instance.game.FlightMode == "BirdmanRally"){
                GameManager.instance.game.FlightMode = "TestFlight";
                Time.timeScale=1f;
                SceneManager.LoadScene("FlightScene");
            }else if(GameManager.instance.game.FlightMode == "TestFlight"){
                GameManager.instance.game.FlightMode = "BirdmanRally";
                Time.timeScale=1f;
                SceneManager.LoadScene("FlightScene");
            }

        }
    }
}
