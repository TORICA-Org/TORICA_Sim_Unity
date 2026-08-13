using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadFactor : MonoBehaviour
{
    private Text scoreText;
    private AerodynamicParameters aero;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = this.GetComponent<Text>();
        aero = GameManager.instance.aero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        scoreText.text = aero.Airspeed.ToString("0.000");
    }
}
