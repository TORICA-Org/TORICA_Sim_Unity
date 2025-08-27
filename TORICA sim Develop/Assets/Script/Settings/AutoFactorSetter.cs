using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoFactorSetter : MonoBehaviour
{
    private float DefaultPitchGravity;
    private AerodynamicCalculator script;
    [SerializeField] private InputField inputField;//ポート番号入力フィールド
    // Start is called before the first frame update
    void Start()
    {
        script = MyGameManeger.instance.Plane.GetComponent<AerodynamicCalculator>();
    }

    public void OnPush(){
        /*
        if(script.massRightNow != 0){MyGameManeger.instance.massRightFactor = script.massLeftRightS/(script.massRightNow/1000);}
        else{MyGameManeger.instance.massRightFactor = 0;}

        if(script.massLeftNow != 0){MyGameManeger.instance.massLeftFactor = script.massLeftRightS/(script.massLeftNow/1000);}
        else{MyGameManeger.instance.massLeftFactor = 0;}

        if(script.massBackwardRightNow != 0){MyGameManeger.instance.massBackwardRightFactor = script.massBackwardS/(script.massBackwardRightNow/1000);}
        else{MyGameManeger.instance.massBackwardRightFactor = 0;}

        if(script.massBackwardLeftNow != 0){MyGameManeger.instance.massBackwardLeftFactor = script.massBackwardS/(script.massBackwardLeftNow/1000);}
        else{MyGameManeger.instance.massBackwardLeftFactor = 0;}
        */
        
        if(script.massRightNow != 0){MyGameManeger.instance.massRightFactor = script.massLeftRightS/((script.massRightNow+script.massLeftNow)/1000);}
        else{MyGameManeger.instance.massRightFactor = 0;}

        if(script.massLeftNow != 0){MyGameManeger.instance.massLeftFactor = script.massLeftRightS/((script.massRightNow+script.massLeftNow)/1000);}
        else{MyGameManeger.instance.massLeftFactor = 0;}

        if(script.massBackwardRightNow != 0){MyGameManeger.instance.massBackwardRightFactor = script.massBackwardS/((script.massBackwardRightNow+script.massBackwardLeftNow)/1000);}
        else{MyGameManeger.instance.massBackwardRightFactor = 0;}

        if(script.massBackwardLeftNow != 0){MyGameManeger.instance.massBackwardLeftFactor = script.massBackwardS/((script.massBackwardRightNow+script.massBackwardLeftNow)/1000);}
        else{MyGameManeger.instance.massBackwardLeftFactor = 0;}

        if(inputField.text != ""){
            MyGameManeger.instance.pilotMassReal = float.Parse(inputField.text);
            Debug.Log(MyGameManeger.instance.pilotMassReal);
        }        
    }
}
