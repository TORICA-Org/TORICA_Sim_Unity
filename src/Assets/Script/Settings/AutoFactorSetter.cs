using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoFactorSetter : MonoBehaviour
{
    private float DefaultPitchGravity;
    private AerodynamicCalculator script;
    [SerializeField] private InputField inputField;//ポート番号入力フィールド

    private GameManager gm = GameManager.instance;

    // Start is called before the first frame update
    private void Start()
    {
        script = gm.Plane.GetComponent<AerodynamicCalculator>();
    }

    public void OnPush()
    {
        /*
        if(script.SerialHandler.massForwardRaw != 0){GameManager.instance.massRightFactor = script.massLeftRightS/(script.SerialHandler.massForwardRaw/1000);}
        else{GameManager.instance.massRightFactor = 0;}

        if(script.massLeftNow != 0){GameManager.instance.massLeftFactor = script.massLeftRightS/(script.massLeftNow/1000);}
        else{GameManager.instance.massLeftFactor = 0;}

        if(SerialHandler.massBackwardRaw != 0){GameManager.instance.massBackwardRightFactor = script.massBackwardS/(SerialHandler.massBackwardRaw/1000);}
        else{GameManager.instance.massBackwardRightFactor = 0;}

        if(script.massBackwardLeftNow != 0){GameManager.instance.massBackwardLeftFactor = script.massBackwardS/(script.massBackwardLeftNow/1000);}
        else{GameManager.instance.massBackwardLeftFactor = 0;}
        */

        /*
        if(script.SerialHandler.massForwardRaw != 0){gm.massRightFactor = script.massLeftRightS/((script.SerialHandler.massForwardRaw+script.massLeftNow)/1000);}
        else{gm.massRightFactor = 0;}

        if(script.massLeftNow != 0){gm.massLeftFactor = script.massLeftRightS/((script.SerialHandler.massForwardRaw+script.massLeftNow)/1000);}
        else{gm.massLeftFactor = 0;}

        if(SerialHandler.massBackwardRaw != 0){gm.massBackwardRightFactor = script.massBackwardS/((SerialHandler.massBackwardRaw+script.massBackwardLeftNow)/1000);}
        else{gm.massBackwardRightFactor = 0;}

        if(script.massBackwardLeftNow != 0){gm.massBackwardLeftFactor = script.massBackwardS/((SerialHandler.massBackwardRaw+script.massBackwardLeftNow)/1000);}
        else{gm.massBackwardLeftFactor = 0;}
        */

        //float pilotMass;
        /*
        if (inputField.text != "")
        {
            gm.massPilotReal = float.Parse(inputField.text);
            Debug.Log(gm.massPilotReal);
        }
        else
        {
            gm.massPilotReal = script.SerialHandler.massForwardRaw + SerialHandler.massBackwardRaw;
            if (gm.VRMode)
            {//HMDの質量を加算 -> 減算に修正
                gm.massPilotReal -= 0.588f;
            }
        }
        */
        if (inputField.text != "")
        {
            AerodynamicCalculator.massPilotDefault = float.Parse(inputField.text);
            Debug.Log(AerodynamicCalculator.massPilotDefault);
        }
        else
        {
            float massForward = 0f;
            if (gm.VRMode)
            {//HMDの質量を加算 -> 減算に修正
                massForward = SerialHandler.massForwardRaw - 0.588f;
            }
            else
            {
                massForward = SerialHandler.massForwardRaw;
            }

            AerodynamicCalculator.massPilotDefault = massForward + SerialHandler.massBackwardRaw;
        }

        // 重心フレーム上での桁中心モーメントについて，（前後センサにかかる荷重によるモーメント）＝（パイロットの体重によるモーメント）とし，その両辺をパイロットの体重で割った式
        AerodynamicCalculator.centerOfMassPilotRaw = (SerialHandler.massForwardRaw * gm.lengthForward + SerialHandler.massBackwardRaw * gm.lengthBackward) / AerodynamicCalculator.massPilotDefault; // 補正前のパイロット重心[m]
        Debug.Log("Raw: " + AerodynamicCalculator.centerOfMassPilotRaw);

        // 機体が定常であるとき，（パイロットの体重によるモーメント）+（空虚重量〈パイロットなしの機体重量〉によるモーメント）=（設計上の重心位置と全備重量によるモーメント）である.
        // シミュレーター上での桁中心モーメントについて，
        // （パイロットの体重によるモーメント）＝（設計上の重心位置と全備重量によるモーメント）-（空虚重量〈パイロットなしの機体重量〉によるモーメント）
        // とし，その両辺をパイロットの体重で割った式
        //float centerOfMassPilotTheoretical = (-1 * AerodynamicCalculator.massAircraft * AerodynamicCalculator.centerOfMassAircraft) / gm.massPilotReal; // 定常におけるパイロット重心の理論値[m]
        float massCurrent = 0f;
        if (SerialHandler.massForwardRaw != 0f && SerialHandler.massBackwardRaw != 0f)
        {
            massCurrent = AerodynamicCalculator.massPilot + AerodynamicCalculator.massAircraft;
        }
        else
        {
            massCurrent = AerodynamicCalculator.massDefault;
        }
        float centerOfMassPilotTheoretical = (massCurrent * AerodynamicCalculator.centerOfMassDefault - AerodynamicCalculator.massAircraft * AerodynamicCalculator.centerOfMassAircraft) / AerodynamicCalculator.massPilotDefault; // 定常におけるパイロット重心の理論値[m]
        Debug.Log("pilot_th: " + centerOfMassPilotTheoretical);

        // 補正値の算出
        gm.centerOfMassPilotOffset = centerOfMassPilotTheoretical - AerodynamicCalculator.centerOfMassPilotRaw; // パイロット重心の補正値
        Debug.Log("offset: " + gm.centerOfMassPilotOffset);

        // Debug.Log("massPilotReal: " + gm.massPilotReal);

        /*
        if (gm.VRMode)
        {//HMDの質量を加算 -> 減算に修正
            pilotMass = gm.massPilotReal - 0.588f;
        }
        else
        {
            pilotMass = gm.massPilotReal;
        }
        */

        //script.massLeftRightS = (pilotMass*AerodynamicCalculator.lengthBackward - AerodynamicCalculator.massAircraft*AerodynamicCalculator.centerOfMassAircraft) / (gm.lengthForward + AerodynamicCalculator.lengthBackward); // 前部荷重の理論値
        //script.massBackwardS = (pilotMass - script.massLeftRightS); // 後部荷重の理論値
        // Debug.Log("lengthForward + lengthBackward" + (gm.lengthForward + AerodynamicCalculator.lengthBackward));
        // Debug.Log("pitchGravityPilotS: " + script.pitchGravityPilotS);

        // Debug.Log("Front: " + script.massLeftRightS + " Rear: " + script.massBackwardS);

        /*
        if (script.SerialHandler.massForwardRaw != 0)
        {
            gm.massRightFactor = script.massLeftRightS / (script.SerialHandler.massForwardRaw / 1000);
            // Debug.Log(script.massLeftRightS + "(Front) / " + script.SerialHandler.massForwardRaw + "(Rear) / 1000 = " + gm.massRightFactor);
        }
        else
        {
            gm.massRightFactor = 0;
        }

        if (SerialHandler.massBackwardRaw != 0)
        {
            gm.massBackwardRightFactor = script.massBackwardS / (SerialHandler.massBackwardRaw / 1000);
            // Debug.Log(script.massBackwardS + "(Front) / " + SerialHandler.massBackwardRaw + "(Rear) / 1000 = " + gm.massBackwardRightFactor);
        }
        else
        {
            gm.massBackwardRightFactor = 0;
        }
        */
    }
}
