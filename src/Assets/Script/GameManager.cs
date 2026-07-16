using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [System.NonSerialized] public bool Landing = false;//着水しているか否か
    // [System.NonSerialized] public bool HUDActive = true;//HUDを有効にしているか否か
    // [System.NonSerialized] public bool HorizontalLineActive = false;//水平赤線を有効にしているか否か
    [System.NonSerialized] public bool SettingActive = false;//ゲーム設定の表示
    [System.NonSerialized] public bool FlightSettingActive = false;//フライト設定の表示

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

    [System.NonSerialized] public bool EnterFlight = false;//フライト開始
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
    [System.NonSerialized] public float JoyStick0 = 0;

    //ロードセルの調整用係数(この係数をロードセルの値に掛ける)
    // [System.NonSerialized] public float massLeftFactor = 1;
    // [System.NonSerialized] public float massRightFactor = 1;
    // [System.NonSerialized] public float massBackwardRightFactor = 1;
    // [System.NonSerialized] public float massBackwardLeftFactor = 1;
    [System.NonSerialized] public float massForwardFactor = 1.0f;
    [System.NonSerialized] public float massBackwardFactor = 1.0f;
    [System.NonSerialized] public float DefaultFactor = 1.00f;
 
    // AerodynamicCalculator.csから移動
    [System.NonSerialized] public float lengthForward = 0.660f;//フレーム前方(フレーム＋センサー部分)から桁(原点)位置[m]
    [System.NonSerialized] public float lengthBackward = -0.330f;//フレーム後方(フレームの端)から桁(原点)位置[m]
    [System.NonSerialized] public float centerOfMassPilotOffset; // 重心位置のオフセット値[m]

    //ジョイスティックの調整用係数(この係数をジョイスティックの値に割る)
    [System.NonSerialized] public float JoyStickFactor = 450;
    [System.NonSerialized] public bool JoyStickFirst = true;

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
    [System.NonSerialized] public bool FirstLoad;//シミュ起動後最初のシーンロードか否か
    [System.NonSerialized] public int SettingMode = 0;
    [System.NonSerialized] public bool TakeOff = false;
    [System.NonSerialized] public float SoundBolume = 0;
    [System.NonSerialized] public string FlightModel;
    [SerializeField] public string DefaultFlightModel = "isoSim2";

    // VRモード
    [System.NonSerialized] public bool VRMode = false;

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

    private GameParameters game;
    private AerodynamicParameters aero;
    private AerodynamicCalculator calc;
    public SerialHandler serial;
    private CameraManager cm;
    private AutoFactorSetter autoFactorSetter = null;
    private GameObject FlightSetting = null;

    private float timeInCurrentStatus = 0.0f;
    private readonly float IGNORE_INPUT_TIME = 1.0f;

    // ===== このインスタンスがロードされたときに実行される ==============
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            FirstLoad = true;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            GameManager.instance.FirstLoad = false;
            Destroy(this.gameObject);
        }
    }

    // ===== Updateの初回呼び出し前に実行される =============
    private void Start()
    {
        FlightSetting = GameObject.Find("FlightSetting");
        cm = GameObject.Find("CameraManager").GetComponent<CameraManager>();
        game = new();
        aero = new();
        //calc = new();
        serial = new();
        // GameObject aeroObj = GameObject.Find("");
    }

    // ===== 毎フレーム実行される ==============
    private void Update()
    {
        serial.Update();


        GameParameters.Status prev = game.status;
        
        bool Setting = SettingActive || FlightSettingActive;
        // ----- ゲームの状態を管理 -----
        if (Setting == true && EnterFlight == false && Landing == false)
        {
            game.status = GameParameters.Status.Preparation;
        }
        else if (Setting == false && EnterFlight == true && Landing == false)
        {
            game.status = GameParameters.Status.Flight;
        }
        else if (Setting == false && EnterFlight == true && Landing == true)
        {
            game.status = GameParameters.Status.Splashdown;
        }

        if (prev != game.status)
        {
            Debug.Log($"GameStatus: {game.status}");
            timeInCurrentStatus = 0f;
        }
        else
        {
            timeInCurrentStatus += Time.unscaledDeltaTime;
        }
        // Debug.Log(timeInCurrentStatus);


        if (timeInCurrentStatus >= IGNORE_INPUT_TIME && serial.Available) { // 現在のシーンで一定時間経過
            if ((serial.holdingPositiveInput || serial.holdingNegativeInput) && game.status == GameParameters.Status.Splashdown) // 長押し＋着水
            {
                Debug.Log("CMD: Reset");
                EnterFlight = false;
                SettingMode = 0;
                SceneManager.LoadScene("FlightScene");
            }
            if (serial.holdingPositiveInput && game.status == GameParameters.Status.Preparation)  
            { // 正の長押し＋準備
                Debug.Log("CMD: Caribrate");
                cm.CaribrateVR();
                if (FlightSetting == null)
                {
                    FlightSetting = GameObject.Find("FlightSetting");
                }
                if (FlightSetting != null && autoFactorSetter == null)
                {
                    autoFactorSetter = FlightSetting.transform.Find("Connection/AutoFactorSetterButton").GetComponent<AutoFactorSetter>();
                }
                if (autoFactorSetter != null)
                {
                    autoFactorSetter.OnPush();
                }
            }
            if (serial.holdingNegativeInput && game.status == GameParameters.Status.Preparation)
            { // 負の長押し＋準備
                Debug.Log("CMD: Start");
                EnterFlight = true;
                if (FlightSetting == null)
                {
                    FlightSetting = GameObject.Find("FlightSetting");
                }
                if (FlightSetting != null)
                {
                    FlightSettingActive = false;
                    FlightSetting.SetActive(FlightSettingActive);
                }
                Time.timeScale=(float)Convert.ToInt32(!FlightSettingActive & !Landing);
                // SaveCsvScript.SetFile();
            }
        }
    }

    public void FixedUpdate()
    {
        // ----- シミュレーションの実行 -----
    }
}
