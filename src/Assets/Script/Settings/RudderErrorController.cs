using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RudderErrorController : MonoBehaviour
{
    [SerializeField] private Text RudderErrorText;
    public void PushRudderErrorButton(){
        GameManager.instance.game.RudderError = !GameManager.instance.game.RudderError;
        if(GameManager.instance.game.RudderError){
            RudderErrorText.text = "有効化中";
        }else{
            RudderErrorText.text = "無効化中";
        }
    }

    void Start(){
        if(GameManager.instance.game.RudderError){
            RudderErrorText.text = "有効化中";
        }else{
            RudderErrorText.text = "無効化中";
        }
    }
}
