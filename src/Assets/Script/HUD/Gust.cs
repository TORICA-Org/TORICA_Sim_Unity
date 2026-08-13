using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gust : MonoBehaviour
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
    void Update()
    {
        string DirectionText;
        if (aero.LocalGustDirection >= 0)
        {
          DirectionText = "R ";
        }
        else
        {
          DirectionText = "L ";
        }
        DirectionText += Mathf.Abs(aero.LocalGustDirection).ToString("0");

        scoreText.text =
            "\n" + aero.LocalGustMag.ToString("0.000") + " m/s"
            + "\n" + DirectionText + " deg";
    }
}
