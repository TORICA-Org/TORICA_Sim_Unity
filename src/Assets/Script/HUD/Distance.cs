using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Distance : MonoBehaviour
{
    private Text scoreText;
    private Rigidbody PlaneRigidbody;
    
    
    // Start is called before the first frame update
    void Start()
    {
        scoreText = this.GetComponent<Text>();
        PlaneRigidbody = GameManager.instance.game.Plane.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float Distance = (PlaneRigidbody.position-GameManager.instance.game.PlatformPosition).magnitude;
        if(GameManager.instance.game.FlightMode=="BirdmanRally"){Distance-=10f;}

        scoreText.text = "\n" + Distance.ToString("0.000");
        if(GameManager.instance.game.RudderErrorMode == 0){
            
        }        
    }
}
