using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private Text timeText;//Textコンポーネントを保持するための変数
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        timeText = GameObject.Find("TimeValue").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.game.status == GameParameters.Status.Preparation)
        {
            timeText.text = "0.00";
        }
        else
        {
            timeText.text = gm.game.timeInFlight.ToString("0.00");
        }
    }
}
