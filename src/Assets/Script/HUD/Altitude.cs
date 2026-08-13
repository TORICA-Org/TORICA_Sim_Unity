using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Altitude : MonoBehaviour
{
    private Text scoreText;
    private Rigidbody PlaneRigidbody;
    private AerodynamicParameters aero;//AerodynamicCalculatorスクリプトにアクセスするための変数

    // Start is called before the first frame update
    void Start()
    {
        scoreText = this.GetComponent<Text>();
        PlaneRigidbody = GameManager.instance.game.Plane.GetComponent<Rigidbody>();
        aero = GameManager.instance.aero;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = aero.ALT.ToString("0.000");
    }
}
