using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameParameters
{
    public enum Status
    {
        Preparation,
        Flight,
        Splashdown,
        Pause,
    }
    public Status status = Status.Preparation;
    private Status previousStatus = Status.Preparation;


    // [System.NonSerialized] public bool Landing = false;//着水しているか否か
    // [System.NonSerialized] public bool HUDActive = true;//HUDを有効にしているか否か
    // [System.NonSerialized] public bool HorizontalLineActive = false;//水平赤線を有効にしているか否か
    // [System.NonSerialized] public bool SettingActive = false;//ゲーム設定の表示
    // [System.NonSerialized] public bool FlightSettingActive = false;//フライト設定の表示

    // [System.NonSerialized] public bool isMainDisplayTPS = true; // true:FPS false:TPS

    [System.NonSerialized] public bool SettingChanged = false;//設定変更
    // [System.NonSerialized] public bool MousePitchControl = false;//マウス操作の可否
    // [System.NonSerialized] public bool RandomWind = false;//ランダム風の可否

    // ----- CSV ----------------------------------------------------------------------------
    // [System.NonSerialized] public bool SaveCsv = false;//CSVファイルへの保存の可否.

    // [System.NonSerialized] public bool customPlaneDataEnabled = false; // CSV機体データ読み込み.
    // [System.NonSerialized] public string customPlaneDataPath = ""; // CSVファイルのパス.

    //[System.NonSerialized] public bool updatePlaneData = true; // 最初は諸元を読み込む必要がある.
    // --------------------------------------------------------------------------------------

    // [System.NonSerialized] public bool EnterFlight = false;//フライト開始
    // [System.NonSerialized] public float MouseSensitivity = 1.000f; // Magnitude of Gust [m/s]
    // [System.NonSerialized] public float GustMag = 0.000f; // Magnitude of Gust [m/s]
    // [System.NonSerialized] public float GustDirection = 0.000f; // Direction of Gust [deg]: -180~180
    // [System.NonSerialized] public float Airspeed_TO = 5.800f; // Airspeed at take-off [m/s]
    // [System.NonSerialized] public float alpha_TO = 0.000f; // Angle of attack at take-off [deg]
    [System.NonSerialized] public string PlaneName;
    [System.NonSerialized] public string FlightMode = "BirdmanRally";
    [System.NonSerialized] public GameObject Plane = null;
    [SerializeField] public string DefaultPlane = "Tatsumi";
    [System.NonSerialized] public Vector3 PlatformPosition = new Vector3(0f, 10.5f, 0f);
    // [System.NonSerialized] public float StartRotation = 0.0f;
    // [System.NonSerialized] public float TailRotation = 0.0f;
    // [System.NonSerialized] public float StartRoll = 0.0f; //離陸時ロールを追加
    [System.NonSerialized] public float TailSetDeg = -1.0f;
    [System.NonSerialized] public float FieldOfView = 90;

    //ロードセルのオフセット値
    // [System.NonSerialized] public float massLeft0 = 0;
    // [System.NonSerialized] public float massRight0 = 0;
    // [System.NonSerialized] public float massBackwardRight0 = 0;
    // [System.NonSerialized] public float massBackwardLeft0 = 0;
    // [System.NonSerialized] public float JoyStick0 = 0;

    //ロードセルの調整用係数(この係数をロードセルの値に掛ける)
    // [System.NonSerialized] public float massLeftFactor = 1;
    // [System.NonSerialized] public float massRightFactor = 1;
    // [System.NonSerialized] public float massBackwardRightFactor = 1;
    // [System.NonSerialized] public float massBackwardLeftFactor = 1;
    // [System.NonSerialized] public float massForwardFactor = 1.0f;
    // [System.NonSerialized] public float massBackwardFactor = 1.0f;
    // [System.NonSerialized] public float DefaultFactor = 1.00f;

    // AerodynamicCalculator.csから移動
    [System.NonSerialized] public float lengthForward = 0.660f;//フレーム前方(フレーム＋センサー部分)から桁(原点)位置[m]
    [System.NonSerialized] public float lengthBackward = -0.330f;//フレーム後方(フレームの端)から桁(原点)位置[m]
    // [System.NonSerialized] public float centerOfMassPilotOffset; // 重心位置のオフセット値[m] // `PilotPositionSensor,cs`へ移動

    //ジョイスティックの調整用係数(この係数をジョイスティックの値に割る)
    //[System.NonSerialized] public float JoyStickFactor = 450;
    //[System.NonSerialized] public bool JoyStickFirst = true;

    //[System.NonSerialized] public float massPilotReal = 0f;
    //エアデータ保存リスト
    [System.NonSerialized] public List<float> AirspeedList = new List<float>();
    [System.NonSerialized] public List<float> AltList = new List<float>();
    [System.NonSerialized] public List<float> AlphaList = new List<float>();
    [System.NonSerialized] public List<float> BetaList = new List<float>();
    [System.NonSerialized] public List<float> ThetaList = new List<float>();
    [System.NonSerialized] public List<float> PhiList = new List<float>();
    [System.NonSerialized] public List<float> CenterOfGList = new List<float>();
    [System.NonSerialized] public List<float> drList = new List<float>();
    [System.NonSerialized] public float Distance = 0.0f;

    //エラー関係
    [System.NonSerialized] public string errorText;
    [System.NonSerialized] public bool error = false;//エラーテキストが発行されるか否か

    // [System.NonSerialized] public bool FrameUseable = false;
    [System.NonSerialized] public int SettingMode = 0;
    // [System.NonSerialized] public bool TakeOff = false;
    [System.NonSerialized] public float SoundVolume = 0;
    // [System.NonSerialized] public string FlightModel;
    // [SerializeField] public string DefaultFlightModel = "isoSim2";

    // VRモード
    [System.NonSerialized] public bool VRMode = false;
    public bool isVRInitialized = false; // VRが初期化されたかどうかを示すフラグ

    // CameraManager.csから移動
    [System.NonSerialized] public Vector3 caribrationOffset = Vector3.zero; // キャリブレーションのオフセットを保持するフィールド.
    [System.NonSerialized] public Quaternion calibrationRotationOffset = Quaternion.identity; // 回転のキャリブレーションのオフセットを保持するフィールド.

    //トラブルモード
    [System.NonSerialized] public bool RudderError = false;
    [System.NonSerialized] public float RudderErrorValue = 0;
    [System.NonSerialized] public int RudderErrorMode = 0;
    [System.NonSerialized] public bool CenterOfMassError = false;
    [System.NonSerialized] public float CenterOfMassErrorValue = 0;

    //ランダムモード
    [System.NonSerialized] public bool CenterOfMassRand = false;
    [System.NonSerialized] public float CenterOfMassRandValue = 1;
    [System.NonSerialized] public bool RudderRand = false;
    [System.NonSerialized] public float RudderRandValue = 1;
    [System.NonSerialized] public bool GustRand = false;
    [System.NonSerialized] public float GustRandValue = 0;
    [System.NonSerialized] public bool CgeRand = false;
    [System.NonSerialized] public float CgeRandValue = 0;

    public System.Diagnostics.Stopwatch statusStopwatch;
    public float timeInCurrentStatus = 0.0f;

    public System.Diagnostics.Stopwatch flightStopwatch;
    public float timeInFlight = 0.0f;

    public delegate void OnStatusChangeDelegate();
    public event OnStatusChangeDelegate OnStatusChange;

    public delegate void OnEnterPreparationDelegate();
    public event OnEnterPreparationDelegate OnEnterPreparation;
    public delegate void OnEnterFlightDelegate();
    public event OnEnterFlightDelegate OnEnterFlight;
    public delegate void OnSplashdownDelegate();
    public event OnSplashdownDelegate OnSplashdown;
    public delegate void OnEnterPauseDelegate();
    public event OnEnterPauseDelegate OnEnterPause;

    static private Thread thread_;
    static private SynchronizationContext context = SynchronizationContext.Current;

    public GameParameters()
    {
        statusStopwatch = new System.Diagnostics.Stopwatch();
        statusStopwatch.Start();
        flightStopwatch = new System.Diagnostics.Stopwatch();
        thread_ = new Thread(InvokeEvent);
        thread_.Start();
    }

    private void InvokeEvent()
    {
        while (true)
        {
            if (status != previousStatus)
            {
                context.Post(d => OnStatusChange?.Invoke(), null);
                previousStatus = status;
                statusStopwatch.Restart();
                timeInCurrentStatus = 0.0f;

                if (status == Status.Preparation)
                {
                    flightStopwatch.Reset();
                    context.Post(d => OnEnterPreparation?.Invoke(), null);
                }
                else if (status == Status.Flight)
                {
                    flightStopwatch.Restart();
                    context.Post(d => OnEnterFlight?.Invoke(), null);
                }
                else if (status == Status.Splashdown)
                {
                    flightStopwatch.Stop();
                    context.Post(d => OnSplashdown?.Invoke(), null);
                }
                else if (status == Status.Pause)
                {
                    flightStopwatch.Stop();
                    context.Post(d => OnEnterPause?.Invoke(), null);
                }
            }

            timeInCurrentStatus = (float)statusStopwatch.Elapsed.TotalSeconds;
            timeInFlight = (float)flightStopwatch.Elapsed.TotalSeconds;
        }
    }
}
