using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneController : MonoBehaviour
{
    private GameObject Platform;
    private AerodynamicCalculator script;//AerodynamicCalculatorスクリプトにアクセスするための変数
    private GameManager gm = GameManager.instance;
    // Start is called before the first frame update
    void Start()
    {
        Platform = GameObject.Find("Platform");
        script = GameManager.instance.Plane.GetComponent<AerodynamicCalculator>();

        if(GameManager.instance.FlightMode == "TestFlight"){
            Platform.SetActive(false);  
        }
    }

    // Update is called once per frame
    void Update()
    {
        // "ResetButton"は Input Manager に設定されている.
        if(Input.GetMouseButton(2) || Input.GetButtonDown("ResetButton")){
            Time.timeScale=1f;
            GameManager.instance.EnterFlight = false;
            GameManager.instance.SettingMode = 0;
            SceneManager.LoadScene("FlightScene");
        }

        if(Input.GetKeyDown("m")){
            if(GameManager.instance.FlightMode == "BirdmanRally"){
                GameManager.instance.FlightMode = "TestFlight";
                Time.timeScale=1f;
                SceneManager.LoadScene("FlightScene");
            }else if(GameManager.instance.FlightMode == "TestFlight"){
                GameManager.instance.FlightMode = "BirdmanRally";
                Time.timeScale=1f;
                SceneManager.LoadScene("FlightScene");
            }

        }
    }
}
