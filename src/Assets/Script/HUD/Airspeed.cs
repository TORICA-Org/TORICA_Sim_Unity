using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Airspeed : MonoBehaviour
{
    private Text scoreText;//Textコンポーネントを保持するための変数
    private AerodynamicParameters aero;//AerodynamicCalculatorスクリプトを保持するための変数

    // Start is called before the first frame update
    void Start()
    {
        scoreText = this.GetComponent<Text>();
        aero = GameManager.instance.aero;
    }

    // Update is called once per frame
    void FixedUpdate()//0.02秒ごとに実行
    {
        scoreText.text = aero.Airspeed.ToString("0.000");
    }
}
