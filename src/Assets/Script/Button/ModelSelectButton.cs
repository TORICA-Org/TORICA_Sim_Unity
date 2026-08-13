using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModelSelectButton : MonoBehaviour
{
    private GameObject LoadingText;
    private bool firstPush = false;

    // Start is called before the first frame update
    void Start()
    {
        LoadingText = GameObject.Find("LoadingText");
        LoadingText.SetActive(false);    
    }

    public void OnClick(int number)
    {
        //Debug.Log("Press Start!");
        if (!firstPush)
        {
            switch (number)
            {
                case 0:
                    GameManager.instance.game.PlaneName = "QX-18";
                    break;
                case 1:
                    GameManager.instance.game.PlaneName = "Tatsumi";
                    break;
                case 2:
                    GameManager.instance.game.PlaneName = "QX-20";
                    break;
                case 3:
                    GameManager.instance.game.PlaneName = "ARG-2";
                    break;
                case 4:
                    GameManager.instance.game.PlaneName = "UL01B";
                    break;
                case 5:
                    GameManager.instance.game.PlaneName = "ORCA18";
                    break;
                case 6:
                    GameManager.instance.game.PlaneName = "ORCA22";
                    break;
                case 7:
                    GameManager.instance.game.PlaneName = "Gardenia";
                    break;
                case 8:
                    GameManager.instance.game.PlaneName = "Aria";
                    break;
                case 9:
                    GameManager.instance.game.PlaneName = "Camellia";
                    break;
                case 10:
                    GameManager.instance.game.PlaneName = "Mio";
                    break;
                case 11:
                    GameManager.instance.game.PlaneName = "Ray";
                    break;
                default:
                    break;
            }
            LoadingText.SetActive(true);    
            SceneManager.LoadScene("FlightScene");
        }
    }
}
