using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorRandController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.instance.game.RudderError){
            if(Random.Range(0.0f,1.0f) < 0.1f){
                Debug.Log("MODE1");
                GameManager.instance.game.RudderErrorMode = 1;
                GameManager.instance.game.RudderErrorValue = Random.Range(-1.0f, 1.0f);//ラダー異常時にある値でラダーが固定される。
            }else if(Random.Range(0.0f,1.0f) < 0.5f){
                Debug.Log("MODE2");
                GameManager.instance.game.RudderErrorMode = 2;
                GameManager.instance.game.RudderErrorValue = Random.Range(-1.0f, 1.0f);//ラダー異常時にある値によくぶれるようになる
            }else if(Random.Range(0.0f,1.0f) < 0.6f){
                Debug.Log("MODE3");
                GameManager.instance.game.RudderErrorMode = 3;
                GameManager.instance.game.RudderErrorValue = RandomUtils.RangeByCentralLimit(-1.0f, 1.0f);//ラダー異常時にある値によくぶれるようになる
            }else{
                GameManager.instance.game.RudderErrorMode = 0;
            }   
        }
        if(GameManager.instance.game.CenterOfMassError){
            if(Random.Range(0.0f,1.0f) < 0.5f){
                GameManager.instance.game.CenterOfMassErrorValue = Random.Range(-0.1f, 0.1f);//重心異常時に足して定常重心がずれる。
            }else{
                GameManager.instance.game.CenterOfMassErrorValue = 0f;
            }
        }
        if(GameManager.instance.game.CenterOfMassRand){
            GameManager.instance.game.CenterOfMassRandValue = RandomUtils.RangeByCentralLimit(0.7f, 1.5f);//重心移動値にかけてゆらぎをもたらす
        }
        else{
            GameManager.instance.game.CenterOfMassRandValue = 1f;
        }
        if(GameManager.instance.game.GustRand){
            GameManager.instance.game.GustRandValue = RandomUtils.RangeByCentralLimit(-1.0f, 1.0f);//風速に足してゆらぎをもたらす
        }
        else{
            GameManager.instance.game.GustRandValue = 0f;
        }
        if(GameManager.instance.game.RudderRand){
            GameManager.instance.game.RudderRandValue = RandomUtils.RangeByCentralLimit(0.7f, 1.5f);//ラダー移動値にかけてゆらぎをもたらす
        }
        else{
            GameManager.instance.game.RudderRandValue = 1f;
        }
        if(GameManager.instance.game.CgeRand){
            GameManager.instance.game.CgeRandValue = RandomUtils.RangeByCentralLimit(0.7f, 1.5f);//地面効果係数にかけてゆらぎをもたらす
        }
        else{
            GameManager.instance.game.CgeRandValue = 1f;
        }

    }
}
