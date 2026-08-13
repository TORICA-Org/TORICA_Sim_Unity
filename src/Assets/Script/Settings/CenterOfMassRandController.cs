using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CenterOfMassRandController : MonoBehaviour
{
    [SerializeField] private Text CenterOfMassRandText;
    public void PushCenterOfMassRandButton(){
        GameManager.instance.game.CenterOfMassRand = !GameManager.instance.game.CenterOfMassRand;
        if(GameManager.instance.game.CenterOfMassRand){
            CenterOfMassRandText.text = "有効化中";
        }else{
            CenterOfMassRandText.text = "無効化中";
        }
    }

    void Start(){
        if(GameManager.instance.game.CenterOfMassRand){
            CenterOfMassRandText.text = "有効化中";
        }else{
            CenterOfMassRandText.text = "無効化中";
        }
    }
}
