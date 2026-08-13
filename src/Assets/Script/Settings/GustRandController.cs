using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GustRandController : MonoBehaviour
{
    [SerializeField] private Text GustRandText;
    public void PushGustRandButton(){
        GameManager.instance.game.GustRand = !GameManager.instance.game.GustRand;
        if(GameManager.instance.game.GustRand){
            GustRandText.text = "有効化中";
        }else{
            GustRandText.text = "無効化中";
        }
    }

    void Start(){
        if(GameManager.instance.game.GustRand){
            GustRandText.text = "有効化中";
        }else{
            GustRandText.text = "無効化中";
        }
    }
}
