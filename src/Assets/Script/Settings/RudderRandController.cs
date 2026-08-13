using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RudderRandController : MonoBehaviour
{
    [SerializeField] private Text RudderRandText;
    public void PushRudderRandButton(){
        GameManager.instance.game.RudderRand = !GameManager.instance.game.RudderRand;
        if(GameManager.instance.game.RudderRand){
            RudderRandText.text = "有効化中";
        }else{
            RudderRandText.text = "無効化中";
        }
    }

    void Start(){
        if(GameManager.instance.game.RudderRand){
            RudderRandText.text = "有効化中";
        }else{
            RudderRandText.text = "無効化中";
        }
    }
}
