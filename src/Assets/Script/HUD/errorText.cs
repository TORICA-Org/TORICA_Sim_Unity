using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class errorText : MonoBehaviour
{
    public Text text;
    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.game.error){
            text.text = GameManager.instance.game.errorText;
            GameManager.instance.game.error = false;
        }
    }
}
