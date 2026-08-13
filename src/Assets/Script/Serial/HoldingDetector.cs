using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoldingDetector{

    private GameParameters game;
    private CameraManager cm;
    private readonly float IGNORE_INPUT_TIME = 3.0f;

    private System.Diagnostics.Stopwatch stopwatch;

    private readonly float IGNORE_CALIBRATE_TIME = 0.5f;
    private double lastCalibrateTime = 0.0;

    public HoldingDetector(GameParameters _game, CameraManager _cm)
    {
        game = _game;
        cm = _cm;
        stopwatch = new();
        stopwatch.Start(); // ストップウォッチをスタート
    }

    public void PositiveHolding()
    {
        if (game.timeInCurrentStatus >= IGNORE_INPUT_TIME)
        {
            if (game.status == GameParameters.Status.Splashdown) // 長押し＋着水
            {
                Debug.Log("CMD: Reset");
                game.status = GameParameters.Status.Preparation;
                game.SettingMode = 2;
                SceneManager.LoadScene("FlightScene");
            }
            if (game.status == GameParameters.Status.Preparation && stopwatch.Elapsed.TotalSeconds - lastCalibrateTime >= IGNORE_CALIBRATE_TIME)
            { // 正の長押し＋準備 && 前回のキャリブレーションからIGNORE_CALIBRATE_TIME以上経過
                lastCalibrateTime = stopwatch.Elapsed.TotalSeconds;
                Debug.Log("CMD: Calibrate");
                if (cm == null)
                {
                    cm = GameObject.Find("CameraManager").GetComponent<CameraManager>();
                }
                if (cm != null)
                {
                    cm.CalibrateVR();
                }
                GameManager.instance.pilot.Reset();
            }
        }
    }

    public void NegativeHolding()
    {
        if (game.timeInCurrentStatus >= IGNORE_INPUT_TIME)
        {
            if (game.status == GameParameters.Status.Splashdown) // 長押し＋着水
            {
                Debug.Log("CMD: Reset");
                game.status = GameParameters.Status.Preparation;
                game.SettingMode = 2;
                SceneManager.LoadScene("FlightScene");
            }
            if (game.status == GameParameters.Status.Preparation)
            {
                Debug.Log("CMD: Start");
                game.status = GameParameters.Status.Flight;
            }
        }
    }

}
