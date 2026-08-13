using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotPositionSensor
{
    private GameManager gm;
    private AerodynamicParameters aero;

    public float loadcellFactor = 1.0f;

    public PilotPositionSensor(AerodynamicParameters _aero)
    {
        gm = GameManager.instance;
        aero = _aero;
    }


    public void UpdateLoadcellFactor()
    {
        if (!SerialHandler.Available)
        {
            return;
        }

        if (Config.OverridePilotMass != 0.0f)
        {
            loadcellFactor = Config.OverridePilotMass / (SerialHandler.massForwardRaw + SerialHandler.massBackwardRaw);
        }
        else
        {
            loadcellFactor = 1.0f;
        }
    }

    public void Reset()
    {
        if (!SerialHandler.Available)
        {
            return;
        }

        float massForwardRaw = SerialHandler.massForwardRaw;
        float massBackwardRaw = SerialHandler.massBackwardRaw;

        if (gm.game.VRMode)
        {//HMDの質量を加算 -> 減算に修正
            massForwardRaw -= 0.588f;
        }

        Debug.Log($"massPilotDefault: {aero.massPilotDefault}, loadcellFactor: {loadcellFactor}");

        if (Config.OverridePilotMass != 0.0f)
        {
            aero.massPilotDefault = Config.OverridePilotMass;
        }
        else
        {
            aero.massPilotDefault = massForwardRaw + massBackwardRaw;
        }

        // 重心フレーム上での桁中心モーメントについて，（前後センサにかかる荷重によるモーメント）＝（パイロットの体重によるモーメント）とし，その両辺をパイロットの体重で割った式
        aero.centerOfMassPilotRaw = (massForwardRaw * gm.game.lengthForward + massBackwardRaw * gm.game.lengthBackward) / aero.massPilotDefault; // 補正前のパイロット重心[m]
        Debug.Log("Raw: " + aero.centerOfMassPilotRaw);

        // 機体が定常であるとき，（パイロットの体重によるモーメント）+（空虚重量〈パイロットなしの機体重量〉によるモーメント）=（設計上の重心位置と全備重量によるモーメント）である.
        // シミュレーター上での桁中心モーメントについて，
        // （パイロットの体重によるモーメント）＝（設計上の重心位置と全備重量によるモーメント）-（空虚重量〈パイロットなしの機体重量〉によるモーメント）
        // とし，その両辺をパイロットの体重で割った式
        //float centerOfMassPilotTheoretical = (-1 * AerodynamicCalculator.massAircraft * AerodynamicCalculator.centerOfMassAircraft) / gm.massPilotReal; // 定常におけるパイロット重心の理論値[m]
        aero.massAircraft = AircraftData.mass - aero.massPilotDefault;
        float massCurrent = aero.massPilot + aero.massAircraft;

        float centerOfMassPilotTheoretical = (massCurrent * aero.centerOfMassDefault - aero.massAircraft * aero.centerOfMassAircraft) / aero.massPilotDefault; // 定常におけるパイロット重心の理論値[m]
        Debug.Log("pilot_th: " + centerOfMassPilotTheoretical);

        // 補正値の算出
        aero.centerOfMassPilotOffset = centerOfMassPilotTheoretical - aero.centerOfMassPilotRaw; // パイロット重心の補正値
        Debug.Log("offset: " + aero.centerOfMassPilotOffset);
    }

}
