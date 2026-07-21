using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text;
using System.IO;
using System;

public class AerodynamicCalculator : MonoBehaviour
{
    //設計データ書き込み用変数
    //protected string customCsvPath;//ファイルパス

    //protected string fileName = "CustomPlaneData.csv";//ファイル名
    //public static List<List<string>> CsvList = new List<List<string>>();//CSVファイルリスト
    //protected bool CanReadCsv = false;

    // public

    [System.NonSerialized] public float Airspeed = 0.000f; // Airspeed [m/s]
    [System.NonSerialized] public float alpha = 0.000f; // Angle of attack [deg]
    [System.NonSerialized] public float beta = 0.000f; // Side slip angle [deg]
    [System.NonSerialized] public float de = 0.000f; // Elevator angle [deg]
    [System.NonSerialized] public float dr = 0.000f; // Rudder angle [deg]
    public float drRatio;
    [System.NonSerialized] public float dh = 0.000f; // Movement of c.g. [-]
    [System.NonSerialized] public float LocalGustMag = 0.000f; // Magnitude of local gust [m/s]
    [System.NonSerialized] public float LocalGustDirection = 0.000f; // Magnitude of local gust [m/s]
    [System.NonSerialized] public float nz = 0.000f; // Load factor [-]

    [System.NonSerialized] public float Groundspeed = 0.000f; // Groundspeed [m/s]
    [System.NonSerialized] public float ALT = 0.000f;

    //計算で用いるセンサー値

    // [System.NonSerialized] public float massLeft;//左ひずみの値[kg]
    [System.NonSerialized] public float massForward;//右ひずみの値[kg]
    [System.NonSerialized] public float massBackward;//後方左ひずみの値[kg]
    // [System.NonSerialized] public float massBackwardLeft;//後方右ひずみの値[kg]

    [System.NonSerialized] public float centerOfMass = 0.000f; // 全体重心計算結果[m] pitchGravity
    static public float centerOfMassPilotRaw = 0.2f; // 補正前重心計算結果[m] pitchGravityPilot
    [System.NonSerialized] public float centerOfMassPilot; // 補正済重心計算結果[m] 定常状態(pitchGravity=0)のパイロット重心 pitchGravityPilotS

    // GameManager.csへ移動
    //[System.NonSerialized] public float centerOfMassPilotOffset; // 重心位置のオフセット値[m]

    [System.NonSerialized] public float massLeftRightS;//定常状態の前センサーの値(合計値ではなく一つのセンサーの値)
    [System.NonSerialized] public float massBackwardS;//定常状態の後センサーの値(合計値ではなく一つのセンサーの値)

    // Phisics

    static protected float rho = 1.164f;
    static protected float hE0 = 10.500f; // Altitude at Take-off [m]

    // At Cruise without Ground Effect

    static protected float Airspeed0; // Magnitude of ground speed [m/s]
    static protected float alpha0; // Angle of attack [deg]
    static protected float CDp0; // Parasitic drag [-]
    static protected float Cmw0; // Pitching momentum [-]
    static protected float CLMAX; // Lift Coefficient [-]
    static protected float CL0 = 0.000f; // Lift Coefficient [-]
    static protected float CLw0 = 0.000f; // Lift Coefficient [-]
    static protected float CLt0 = 0.000f; // Tail Coefficient [-]
    static protected float epsilon0 = 0.000f; // Downwash

    // Plane

    static protected bool Downwash; // Conventional Tail: True, T-Tail: False
    static protected float CL = 0.000f; // Lift Coefficient [-]
    static protected float CD = 0.000f; // Drag Coefficient [-]
    static protected float Cx = 0.000f; // X Force Coefficient [-]
    static protected float Cy = 0.000f; // Y Force Coefficient [-]
    static protected float Cz = 0.000f; // Z Force Coefficient [-]
    static protected float Cl = 0.000f; // Rolling momentum [-]
    static protected float Cm = 0.000f; // Pitching momentum [-]
    static protected float Cn = 0.000f; // Yawing momentum [-]
    static protected float dh0 = 0.000f; // Initial Mouse Position

    // Wing

    static protected float Sw; // Wing area of wing [m^2]
    static protected float bw; // Wing span [m]
    static protected float cMAC; // Mean aerodynamic chord [m]
    static public float aw; // Wing Lift Slope [1/deg]

    static protected float ac;
    static protected float cg;

    static protected float hw; // Length between Wing a.c. and c.g. [-] ac-cg

    static protected float hw0;
    static protected float lt0;

    static protected float AR; // Aspect Ratio [-]
    static protected float ew; // Wing efficiency [-]
    static protected float CLw = 0.000f; // Lift Coefficient [-]

    // Tail

    static protected float St; // Wing area of tail [m^2]
    static protected float at; // Tail Lift Slope [1/deg]
    static protected float lt; // Length between Tail a.c. and c.g. [m]
    static protected float VH; // Tail Volume [-]
    static protected float deMAX; // Maximum elevator angle [deg]
    static protected float tau; // Control surface angle of attack effectiveness [-]
    static protected float CLt = 0.000f; // Lift Coefficient [-]

    // Fin

    static protected float drMAX; // Maximum rudder angle

    // Ground Effect

    static protected float CGEMIN; // Minimum Ground Effect Coefficient [-]
    static protected float CGE = 0f; // Ground Effect Coefficient: CDiGE/CDi [-]

    // Stability derivatives

    static protected float Cyb; // [1/deg]
    static protected float Cyp; // [1/rad]
    static protected float Cyr; // [1/rad]
    static protected float Cydr; // [1/deg]
    static protected float Cnb; // [1/deg]
    static protected float Cnp; // [1/rad]
    static protected float Cnr; // [1/rad]
    static protected float Cndr; // [1/deg]
    static protected float Clb; // [1/deg]
    static protected float Clp; // [1/rad]
    static protected float Clr; // [1/rad]
    static protected float Cldr; // [1/deg]

    // Gust

    static protected Vector3 Gust = Vector3.zero; // Gust [m/s]

    // Rotation

    static protected float phi; // ロール[deg]
    static protected float theta;  // ピッチ[deg]
    static protected float psi; // ヨー[deg]

    protected Rigidbody PlaneRigidbody;

    // ----- 設計値（重心センサーのキャリブレーションや慣性モーメントの算出に使用） -----
    // 全備

    static public float massDefault; // 設計上の全重量[kg]
    static public float centerOfMassDefault; // 設計上の全体重心位置[m]
    static public float IyyDefault; // 設計上のピッチ慣性モーメント[kg*m^2]

    // 空虚
    //static public float massAircraft; // 空虚の機体重量[kg] // 既出
    //static public float centerOfMassAircraft; // 空虚の機体重心位置[m] // 既出
    // パイロット
    static public float massPilotDefault; // 設計上のパイロット重量[kg]

    // ----------------------------------------------------------------------------

    //追加機体データ

    // GameManager.csに移動（DontDestroyであってほしい）
    //static public float gm.lengthForward = 0.660f;//フレーム前方(フレーム＋センサー部分)から桁(原点)位置[m]
    //static public float gm.lengthBackward = 0.330f;//フレーム後方(フレームの端)から桁(原点)位置[m]
    static public float centerOfMassAircraft;//機体のみ全重心(パイロットなし,ピッチのみ)[m]

    static public float massAircraft;//機体のみ全重量[kg]

    static public float massPilot;//設計上のパイロット重量[kg]

    //static protected float SensorPositionY = 0.645f;//桁中心から垂直に線を超音波センサーの位置までおろした時の線の長さ[m]
    //static protected float SensorPositionZ = 0.0f;//↑の到達点から超音波センサーまでの長さ[m]
    //static protected float AircraftHight = 0.74f;//プラホからコクピ下部までの長さ[m]

    static protected bool PlusData;//追加機体データが存在するか

    //計算結果データ

    static protected float hw2;//	主翼空力中心と全機重心の距離（cMACで無次元化）（再計算バージョン）

    //翼持ちデータ

    static protected float YMin;//翼持ちの最小荷重(機体のみ重量/2)
    static protected float YrMax;//右翼持ちの許容最大荷重
    static protected float YlMax;//左翼持ちの許容最大荷重
    static protected float YrMoment;//右翼持ち本人がかけるモーメント
    static protected float YlMoment;//左翼持ち本人がかけるモーメント

    static protected float YL;//機体中心から翼持ち棒までの長さ[m]


    public static GameObject Aircraft;
    static protected GameObject SensorPoint;
    
    protected bool AddTaleForce;

    private GameManager gm = GameManager.instance; // MyGameManagerをgmとして宣言
    private CameraManager cm;

    public void OnEnables()
    {
        if (gm.PlaneName != null)
        {
            if (this.gameObject.name == gm.PlaneName)
            {
                Aircraft = this.gameObject;
            }
        }
        else
        {
            if (this.gameObject.name == gm.DefaultPlane)
            {
                gm.PlaneName = gm.DefaultPlane;
                Aircraft = this.gameObject;
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        cm = GameObject.Find("CameraManager").GetComponent<CameraManager>();

        // Get rigidbody component
        PlaneRigidbody = this.GetComponent<Rigidbody>();
        this.transform.rotation = Quaternion.Euler(0.0f, Config.TakeoffYaw, 0.0f);

        SensorPoint = GameObject.Find("SensorPoint");

        //設計データ読み込み用
        // fileName = gm.PlaneName + ".csv";
        // customCsvPath = gm.PlaneName + ".csv";
        // customCsvPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "CustomPlaneData.csv");
        // Debug.Log("File path: " + customCsvPath);
        // ReadFile();

        // Input Specifications
        InputSpecifications();
        // ----- 設計値の保存 ---------------------------------------------------------------
        centerOfMassDefault = PlaneRigidbody.centerOfMass.x; // 設計上の全体重心位置[m]を保存
        // --------------------------------------------------------------------------------

        //pitchGravityPilotS = ((PlaneRigidbody.mass*pitchGravity)-(massAircraft*centerOfMassAircraft))/massPilot;
        //Debug.Log(massAircraft+","+centerOfMassAircraft+","+massPilot+","+gm.lengthForward+","+gm.lengthBackward);
        /*
        if (gm.massPilotReal == 0)
        {//体重入力省略の場合の処理
            gm.massPilotReal = massPilot;
        }
        */
        if (massAircraft != 0 && centerOfMassAircraft != 0 && massPilot != 0 && gm.lengthForward != 0 && gm.lengthBackward != 0)
        {
            PlusData = true;
            centerOfMassPilot = -1 * massAircraft * centerOfMassAircraft / massPilot;

            //今までのやつ
            /*
            massLeftRightS = (massPilot*(pitchGravityPilotS+gm.lengthBackward)/(gm.lengthForward+gm.lengthBackward))/2;
            massBackwardS = (massPilot - massLeftRightS*2)/2;
            */

            // =====AutoFactorSetter.csへ移植=====
            /*
            if(gm.VRMode){//HMDの質量を加算
                float massPilotVR=gm.massPilotReal+0.588f;
                massLeftRightS = (massPilotVR*(pitchGravityPilotS+gm.lengthBackward)/(gm.lengthForward+gm.lengthBackward)); // 前部荷重の理論値
                massBackwardS = (massPilotVR - massLeftRightS); // 後部荷重の理論値
            }
            else{
                float massPilotNonVR=gm.massPilotReal;
                massLeftRightS = (massPilotNonVR*(pitchGravityPilotS+gm.lengthBackward)/(gm.lengthForward+gm.lengthBackward)); // 前部荷重の理論値
                massBackwardS = (massPilotNonVR - massLeftRightS); // 後部荷重の理論値
            }
            */
        }
        else
        {
            PlusData = false;
        }

        YMin = massAircraft / 2;
        YrMax = 80.0f;
        YlMax = 80.0f;

        FlightModelStart();
    }

    private void Update()//フライトモデルに関わらず実行されるINPUT関連の処理
    {
        float pitchGravityBefore = centerOfMass;
        float pitchGravityPilotBefore = centerOfMassPilotRaw;

        if (Config.UseMousePitchControl)
        {//マウスコントロール
            if (PlusData)
            {
                //Debug.Log(PlusData);
                centerOfMassPilot = -massAircraft * centerOfMassAircraft / massPilot;
                centerOfMassPilotRaw = centerOfMassPilot + ((Input.mousePosition.y - dh0) * Config.MouseSensitivity) * 0.0002f;
            }
            //pitchGravity = ((pitchGravityPilot*massPilot)+(centerOfMassAircraft*massAircraft))/PlaneRigidbody.mass;
            centerOfMass = (gm.CenterOfMassErrorValue + ((Input.mousePosition.y - dh0) * Config.MouseSensitivity) * 0.0002f) * gm.CenterOfMassRandValue;
        }

        if (Input.GetAxisRaw("GStick") != 0)
        {//ゲームパッドコントロールのトリガー
            centerOfMassPilot = -massAircraft * centerOfMassAircraft / massPilot;
            centerOfMassPilotRaw = centerOfMassPilot - Input.GetAxisRaw("GStick") * 0.10f;
            centerOfMass = (gm.CenterOfMassErrorValue + ((centerOfMassPilotRaw * massPilot) + (centerOfMassAircraft * massAircraft)) / PlaneRigidbody.mass) * gm.CenterOfMassRandValue;
        }

        if (SerialHandler.Available)//フレームコントロール
        {
            /*
            massLeftNow = 20000f;
            massForwardNow = 20000f;
            massBackwardNow = 20000f;
            massBackwardLeftNow = 20000f;
            */

            //mass~Now ←センサー生データ
            //mass~Factor ←Rawを調整するための係数
            //mass~ ←NowにFactorの値をかけて計算に使用する値

            // マイコン側でkgに変換する
            massForward = gm.massForwardFactor * (SerialHandler.massForwardRaw);
            massBackward = gm.massBackwardFactor * (SerialHandler.massBackwardRaw);

            // massLeft = gm.massLeftFactor*(massLeftNow/1000);
            // massBackwardLeft = gm.massBackwardLeftFactor*(massBackwardLeftNow/1000);

            /*
            int safeModeCount = 0;
            int safeModeCount2 = 0;
            if(massForward == 0){
                safeModeCount++;
            }
            else{
                safeModeCount2 = 1;
            }

            if(massLeft == 0){
                safeModeCount++;
            }
            else{
                safeModeCount2 = 2;
            }
            if(massBackwardLeft == 0){
                safeModeCount++;
            }
            else{
                safeModeCount2 = 3;
            }

            if(massBackward == 0){
                safeModeCount++;
            }
            else{
                safeModeCount2 = 4;
            }

            if(safeModeCount == 3){
                Debug.Log("SafeMode");

                if(safeModeCount2 == 1){
                    massBackwardLeft = 53 - massForward;
                }

                if(safeModeCount2 == 2){
                    massBackwardLeft = 53 - massLeft;
                }

                if(safeModeCount2 == 3){
                    massLeft = 53 - massBackwardLeft;
                }

                if(safeModeCount2 == 4){
                    massLeft = 53 - massBackward;
                }
            }
            */

            // float NowMass = massLeft + massForward + massBackward + massBackwardLeft;
            massPilot = massForward + massBackward;

            /*
            // pitchGravity = (gm.CenterOfMassErrorValue + (((gm.lengthForward*massLeft)+(gm.lengthForward*massForward)-(gm.lengthBackward*(massBackward + massBackwardLeft))+(centerOfMassAircraft*massAircraft))/(massLeft+massForward+(massBackward + massBackwardLeft)+massAircraft)))*gm.CenterOfMassRandValue;
            centerOfMass = (gm.CenterOfMassErrorValue + ((gm.lengthForward * massForward) - (gm.lengthBackward * massBackward) + (centerOfMassAircraft * massAircraft)) / (massForward +massBackward+ massAircraft)) * gm.CenterOfMassRandValue;

            if (-0.4f < centerOfMass && centerOfMass < 0.4f){//外れ値除去処理(基本的に重心は±0.4を超えることはない)
                //リジットボディに代入するピッチの値を計算
                //pitchGravity = (GameManager.instance.CenterOfMassErrorValue + (((gm.lengthForward*massLeft)+(gm.lengthForward*massForward)-(gm.lengthBackward*(massBackward + massBackwardLeft))+(centerOfMassAircraft*massAircraft))/(massLeft+massForward+(massBackward + massBackwardLeft)+massAircraft)))*GameManager.instance.CenterOfMassRandValue;
                centerOfMassPilot = ((PlaneRigidbody.mass*centerOfMass)-(massAircraft*centerOfMassAircraft))/massPilot;
                if(NowMass != 0 ){
                    // pitchGravityPilot = (((gm.lengthForward*massLeft)+(gm.lengthForward*massForward)-(gm.lengthBackward*(massBackward + massBackwardLeft)))/(massLeft+massForward+(massBackward + massBackwardLeft)));
                    centerOfMassPilotRaw = (((gm.lengthForward * massForward) - (gm.lengthBackward * massBackward)) / (massForward + massBackward));
                }
                else{
                    centerOfMassPilotRaw = centerOfMassPilot;
                }
            }else{
                Debug.Log("外れ値除去成功！");
                centerOfMass = pitchGravityBefore;
                centerOfMassPilot = pitchGravityPilotBefore;
            }
            */

            // 重心フレーム上での桁中心モーメントについて，（前後センサにかかる荷重によるモーメント）＝（パイロットの体重によるモーメント）とし，その両辺をパイロットの体重で割った式
            centerOfMassPilotRaw = (gm.lengthForward * massForward + gm.lengthBackward * massBackward) / (massForward + massBackward); // 補正前のパイロット重心[m]

            // 補正
            centerOfMassPilot = centerOfMassPilotRaw + gm.centerOfMassPilotOffset; // 補正後のパイロット重心[m]

            // 桁中心モーメントについて，（パイロット体重と空虚重量〈パイロットなしの機体重量〉によるモーメント）＝（全備重量によるモーメント）とし，その両辺を全備重量で割った式
            centerOfMass = (massPilot * centerOfMassPilot + massAircraft * centerOfMassAircraft) / (massPilot + massAircraft);

            if (-0.4f < centerOfMass && centerOfMass < 0.4f)//外れ値除去処理(基本的に重心は±0.4を超えることはない
            { }
            else
            {
                Debug.Log("外れ値除去成功！");
                centerOfMass = pitchGravityBefore;
                centerOfMassPilot = pitchGravityPilotBefore;
            }
        }
        // Get control surface angles
        de = 0.000f;
        dr = 0.000f;

        if (!SerialHandler.Available)
        {
            de = Input.GetAxisRaw("Vertical") * deMAX;
            dr = -Input.GetAxisRaw("Horizontal") * drMAX * gm.RudderRandValue;
        }
        if (Input.GetMouseButton(0)) { dr = drMAX * gm.RudderRandValue; }
        else if (Input.GetMouseButton(1)) { dr = -drMAX * gm.RudderRandValue; }

        if (Input.GetAxisRaw("Trigger") * drMAX != 0)
        {
            dr = -Input.GetAxisRaw("Trigger") * drMAX * gm.RudderRandValue;
        }

        if (SerialHandler.Available)
        {
            //↓必要な処理
            // dr = ((JoyStickNow - gm.JoyStick0) / gm.JoyStickFactor) * drMAX * gm.RudderRandValue;
            dr = drMAX * SerialHandler.rudder; // ラダー駆動角
            // Debug.Log($"dr: {dr}");
        }

        if (gm.RudderError && gm.RudderErrorMode != 0)
        {
            if (gm.RudderErrorMode == 1)
            {
                dr = gm.RudderErrorValue * drMAX;
            }
            else if (gm.RudderErrorMode == 2)
            {
                if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    dr = gm.RudderErrorValue * drMAX;
                }
            }
            else if (gm.RudderErrorMode == 3)
            {
                dr += gm.RudderErrorValue * drMAX;
            }
        }

        // VR Only Mode (重心センサーを使う場合は使用しない)
        if (Config.VrOnlyMode)
        {
            massPilot = 68.0f; // [kg]

            centerOfMassPilot = cm.GetZAxisMovement(); // パイロット重心は直接取得できる.

            // 桁中心モーメントについて，（パイロット体重と空虚重量〈パイロットなしの機体重量〉によるモーメント）＝（全備重量によるモーメント）とし，その両辺を全備重量で割った式
            centerOfMass = (massPilot * centerOfMassPilot + massAircraft * centerOfMassAircraft) / (massPilot + massAircraft);
        }
    }

    private void FixedUpdate()
    {
        FlightModelFixedUpdate();
    }

    public void InputSpecifications()
    {
        // 機体の重量と慣性モーメント - 6
        PlaneRigidbody.mass = AircraftData.mass;
        PlaneRigidbody.centerOfMass = AircraftData.centerOfMass;
        PlaneRigidbody.inertiaTensor = AircraftData.inertiaTensor;
        PlaneRigidbody.inertiaTensorRotation = AircraftData.inertiaTensorRotation;
        massAircraft = AircraftData.massAircraft;
        centerOfMassAircraft = AircraftData.centerOfMassAircraft;

        // 巡航時 - 5
        Airspeed0 = AircraftData.Airspeed0;
        alpha0 = AircraftData.alpha0;
        CDp0 = AircraftData.CDp0;
        Cmw0 = AircraftData.Cmw0;
        CLMAX = AircraftData.CLMAX;

        // 主翼 - 7
        Sw = AircraftData.Sw;
        bw = AircraftData.bw;
        cMAC = AircraftData.cMAC;
        aw = AircraftData.aw;
        hw = AircraftData.hw;
        ew = AircraftData.ew;
        AR = AircraftData.AR;

        // 水平尾翼 - 7
        Downwash = AircraftData.Downwash;
        St = AircraftData.St;
        at = AircraftData.at;
        lt = AircraftData.lt;
        deMAX = AircraftData.deMAX;
        tau = AircraftData.tau;
        VH = AircraftData.VH;

        // 垂直尾翼 - 1
        drMAX = AircraftData.drMAX;

        // 地面効果 - 1
        CGEMIN = AircraftData.CGEMIN;

        // 安定微係数 - 12
        Cyb = AircraftData.Cyb;
        Cyp = AircraftData.Cyp;
        Cyr = AircraftData.Cyr;
        Cydr = AircraftData.Cydr;
        Clb = AircraftData.Clb;
        Clp = AircraftData.Clp;
        Clr = AircraftData.Clr;
        Cldr = AircraftData.Cldr;
        Cnb = AircraftData.Cnb;
        Cnp = AircraftData.Cnp;
        Cnr = AircraftData.Cnr;
        Cndr = AircraftData.Cndr;

        // 離陸 - 1
        YL =  AircraftData.YL;
    }

    private void FlightModelStart()
    {
        Debug.Log("isoSim1");
        // Set take-off speed
        if (GameManager.instance.FlightMode == "BirdmanRally")
        {
            //GameManager.instance.Airspeed_TO = 5.0f; // Airspeed at take-off [m/s]
            PlaneRigidbody.velocity = Vector3.zero;
        }
        else if (GameManager.instance.FlightMode == "TestFlight")
        { //
            PlaneRigidbody.velocity = new Vector3(
                Airspeed0 * Mathf.Cos(Mathf.Deg2Rad * alpha0) * Mathf.Cos(Mathf.Deg2Rad * Config.TakeoffYaw),
                -Airspeed0 * Mathf.Sin(Mathf.Deg2Rad * alpha0),
                -Airspeed0 * Mathf.Cos(Mathf.Deg2Rad * alpha0) * Mathf.Sin(Mathf.Deg2Rad * Config.TakeoffYaw)
            );
        }

        // Calculate CL at cluise
        CL0 = (PlaneRigidbody.mass * Physics.gravity.magnitude) / (0.5f * rho * Airspeed0 * Airspeed0 * Sw);
        CLt0 = (Cmw0 + CL0 * hw) / (VH + (St / Sw) * hw);
        CLw0 = CL0 - (St / Sw) * CLt0;
        if (Downwash) { epsilon0 = (CL0 / (Mathf.PI * ew * AR)) * Mathf.Rad2Deg; }

        dh0 = Screen.height / 2f; // Initial Mouse Position

        //Debug.Log(CLw0);
        hw0 = hw;
        lt0 = lt;
    }

    private void FlightModelFixedUpdate()
    {
        //Debug.Log("isoSim1 FixedUpdate");
        //入力系統
        //リジットボディに代入
        PlaneRigidbody.centerOfMass = new Vector3(centerOfMass, PlaneRigidbody.centerOfMass.y, PlaneRigidbody.centerOfMass.z);

        //hwに代入する重心位置(%MAC)を計算
        hw2 = hw0 - (centerOfMass / cMAC);
        //hwに代入
        hw = hw2;

        lt = lt0 + centerOfMass;

        //if (GameManager.instance.PlaneName == "Tatsumi")
        //{
        //    //float Iyy = (85.6f * centerOfMass * centerOfMass) + (38.63f * centerOfMass) + 1241.85f;
        //    Vector3 tensor = PlaneRigidbody.inertiaTensor;
        //    //tensor.y = Iyy;
        //    tensor.x = 5f;
        //    PlaneRigidbody.inertiaTensor = tensor;
        //}

        // Velocity and AngularVelocity
        float u = transform.InverseTransformDirection(PlaneRigidbody.velocity).x;
        float v = -transform.InverseTransformDirection(PlaneRigidbody.velocity).z;
        float w = -transform.InverseTransformDirection(PlaneRigidbody.velocity).y;
        float p = -transform.InverseTransformDirection(PlaneRigidbody.angularVelocity).x * Mathf.Rad2Deg;
        float q = transform.InverseTransformDirection(PlaneRigidbody.angularVelocity).z * Mathf.Rad2Deg;
        float r = transform.InverseTransformDirection(PlaneRigidbody.angularVelocity).y * Mathf.Rad2Deg;
        float hE = PlaneRigidbody.position.y;
        float Distance = (PlaneRigidbody.position - GameManager.instance.PlatformPosition).magnitude - 10f;

        // Force and Momentum
        Vector3 AerodynamicForce = Vector3.zero;
        Vector3 AerodynamicMomentum = Vector3.zero;
        Vector3 TakeoffForce = Vector3.zero;

        // Hoerner and Borst (Modified)
        CGE = (CGEMIN + 33f * Mathf.Pow((hE / bw), 1.5f)) / (1f + 33f * Mathf.Pow((hE / bw), 1.5f));
        if (GameManager.instance.FlightMode == "BirdmanRally" && Distance < -0.5f)
        {
            //CGE = (CGEMIN+33f*Mathf.Pow((hE/bw),1.5f))/(1f+33f*Mathf.Pow((hE/bw),1.5f));
            CGE = (CGEMIN + 33f * Mathf.Pow((1.5f / bw), 1.5f)) / (1f + 33f * Mathf.Pow((1.5f / bw), 1.5f));
        }
        //Debug.Log(CGE);
        //if (GameManager.instance.MousePitchControl){
        //    dh = -(Input.mousePosition.y-dh0)*0.0002f*GameManager.instance.MouseSensitivity;
        //}

        // Gust
        LocalGustMag = (Wind.GetMagnitude(Distance) + GameManager.instance.GustRandValue) * Mathf.Pow((hE / hE0), 1f / 7f);
        Gust = Quaternion.AngleAxis(Wind.GetDirection(Distance), Vector3.up) * (Vector3.right * LocalGustMag);
        Vector3 LocalGust = this.transform.InverseTransformDirection(Gust);
        float ug = LocalGust.x + 1e-10f;
        float vg = -LocalGust.z;
        float wg = -LocalGust.y;
        if (ug > 0) { LocalGustDirection = Mathf.Atan(vg / (ug + 1e-10f)) * Mathf.Rad2Deg; }
        else { LocalGustDirection = Mathf.Atan(vg / (ug + 1e-10f)) * Mathf.Rad2Deg + vg / Mathf.Abs((vg + 1e-10f)) * 180; }

        // Calculate angles
        Airspeed = Mathf.Sqrt((u + ug) * (u + ug) + (v + vg) * (v + vg) + (w + wg) * (w + wg));
        Groundspeed = Mathf.Sqrt(u * u + v * v);
        if (SensorPoint != null)
        {
            ALT = SensorPoint.transform.position.y;
        }
        //Debug.Log(Groundspeed);
        alpha = Mathf.Atan((w + wg) / (u + ug)) * Mathf.Rad2Deg;
        //Debug.Log(alpha);

        beta = Mathf.Atan((v + vg) / Airspeed) * Mathf.Rad2Deg;

        // Wing and Tail
        CLw = CLw0 + aw * (alpha - alpha0);
        CLt = CLt0 + at * ((alpha + GameManager.instance.TailSetDeg - alpha0) + (1f - CGE * (CLw / CLw0)) * epsilon0 + de * tau + ((lt - dh * cMAC) / Airspeed) * q);
        if (Mathf.Abs(CLw) > CLMAX) { CLw = (CLw / Mathf.Abs(CLw)) * CLMAX; } // Stall
        if (Mathf.Abs(CLt) > CLMAX) { CLt = (CLt / Mathf.Abs(CLt)) * CLMAX; } // Stall

        // Lift and Drag
        CL = CLw + (St / Sw) * CLt; // CL
        CD = CDp0 * (1f + Mathf.Abs(Mathf.Pow((alpha / 9f), 3f))) + ((CL * CL) / (Mathf.PI * ew * AR)) * CGE; // CD

        // Force
        Cx = CL * Mathf.Sin(Mathf.Deg2Rad * alpha) - CD * Mathf.Cos(Mathf.Deg2Rad * alpha); // Cx
        Cy = Cyb * beta + Cyp * (1f / Mathf.Rad2Deg) * ((p * bw) / (2f * Airspeed)) + Cyr * (1f / Mathf.Rad2Deg) * ((r * bw) / (2f * Airspeed)) + Cydr * dr; // Cy
        Cz = -CL * Mathf.Cos(Mathf.Deg2Rad * alpha) - CD * Mathf.Sin(Mathf.Deg2Rad * alpha); // Cz

        // Torque
        Cl = Clb * beta + Clp * (1f / Mathf.Rad2Deg) * ((p * bw) / (2f * Airspeed)) + Clr * (1f / Mathf.Rad2Deg) * ((r * bw) / (2f * Airspeed)) + Cldr * dr; // Cl
        Cm = Cmw0 + CLw * hw - VH * CLt + CL * dh; // Cm
        Cn = Cnb * beta + Cnp * (1f / Mathf.Rad2Deg) * ((p * bw) / (2f * Airspeed)) + Cnr * (1f / Mathf.Rad2Deg) * ((r * bw) / (2f * Airspeed)) + Cndr * dr; // Cn

        AerodynamicForce.x = 0.5f * rho * Airspeed * Airspeed * Sw * Cx;
        AerodynamicForce.y = 0.5f * rho * Airspeed * Airspeed * Sw * (-Cz);
        AerodynamicForce.z = 0.5f * rho * Airspeed * Airspeed * Sw * (-Cy);
        //Debug.Log("CLt"+CLt+"CL"+CL+"Cz"+Cz+"z"+AerodynamicForce.y);
        AerodynamicMomentum.x = 0.5f * rho * Airspeed * Airspeed * Sw * bw * (-Cl);//roll
        AerodynamicMomentum.y = 0.5f * rho * Airspeed * Airspeed * Sw * bw * Cn;//yaw
        AerodynamicMomentum.z = 0.5f * rho * Airspeed * Airspeed * Sw * cMAC * Cm;//pitch

        if (GameManager.instance.FlightMode == "BirdmanRally" && Distance < -0.5f)
        {
            //Debug.Log("Dist: " + Distance);
            CalculateRotation();

            float W = PlaneRigidbody.mass * Physics.gravity.magnitude;//重力
            float L = 0.5f * rho * Airspeed * Airspeed * Sw * (Cx * Mathf.Sin(Mathf.Deg2Rad * theta) - Cz * Mathf.Cos(Mathf.Deg2Rad * theta));//揚力
            float N = (W - L) * Mathf.Cos(Mathf.Deg2Rad * 3.5f); // N=(W-L)*cos(3.5deg)//翼持ちの抵抗力
            float P = (PlaneRigidbody.mass * Config.TakeoffSpeed * Config.TakeoffSpeed) / (2f * 10f); // P=m*Vto*Vto/2*L//推進力

            //離陸方向をYaw回転に合わせて水平方向に修正
            //Vector3 takeoffDirection = Quaternion.Euler(0f, Config.TakeoffYaw, 0f) * Vector3.forward;
            //TakeoffForce = takeoffDirection * P;

            //TakeoffForce.y = N*Mathf.Cos(Mathf.Deg2Rad*3.5f);

            //float TOFh = P;
            //float TOFv = N*Mathf.Cos(Mathf.Deg2Rad*3.5f);
            //TakeoffForce.x = TOFv*Mathf.Sin(GameManager.instance.TailRotation) + TOFh*Mathf.Cos(GameManager.instance.TailRotation);
            //TakeoffForce.y = TOFv*Mathf.Cos(GameManager.instance.TailRotation) - TOFh*Mathf.Sin(GameManager.instance.TailRotation);
            //Debug.Log("Power:"+P);

            TakeoffForce.x = P * Mathf.Cos(Mathf.Deg2Rad * Config.TakeoffYaw);
            TakeoffForce.y = N * Mathf.Cos(Mathf.Deg2Rad * 3.5f);
            TakeoffForce.z = -P * Mathf.Sin(Mathf.Deg2Rad * Config.TakeoffYaw);

            AerodynamicForce.z = 0f;
            AerodynamicMomentum.x = 0f;//
            AerodynamicMomentum.y = 0f;

            //transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, GameManager.instance.TailRotation);
            //PlaneRigidbody.constraints = RigidbodyConstraints.FreezePositionZ;

            if (AerodynamicMomentum.x <= 0)
            {//左から右に吹く風 左翼がより大きな揚力を生む
                if (Mathf.Abs(AerodynamicMomentum.x) > YL * YMin)
                {//左翼が翼持ちの手を離れている状態
                    //Debug.Log("A1");
                    YlMoment = 0;//既に翼持ちを離れている為、翼持ちはモーメントを与えられない
                }
                else
                {
                    //Debug.Log("B1");
                    YlMoment = -YL * YMin - AerodynamicMomentum.x;//翼持ちが与えるモーメントは、機体を支える最小限のモーメントから風が与えるそれを引いた値である
                }

                if (Mathf.Abs(AerodynamicMomentum.x + YlMoment) <= YL * YrMax)
                {//右翼持ちにまだ余裕がある状態
                    //Debug.Log("C1");
                    YrMoment = -(AerodynamicMomentum.x + YlMoment);//翼持ちに風と逆の翼持ちのモーメントを足した大きな負荷が掛かるが、まだ耐えられる
                }
                else
                {
                    //Debug.Log("D1");
                    YrMoment = YL * YrMax;//つり合いが取れずに右翼持ちのモーメントが足りない状態
                }
            }
            else
            {//右から左に吹く風 右翼がより大きな揚力を生む
                if (Mathf.Abs(AerodynamicMomentum.x) > YL * YMin)
                {//右翼が翼持ちの手を離れている状態
                    //Debug.Log("A2");
                    YrMoment = 0;
                }
                else
                {
                    //Debug.Log("B2");
                    YrMoment = YL * YMin - AerodynamicMomentum.x;
                }

                if (Mathf.Abs(AerodynamicMomentum.x + YrMoment) <= YL * YlMax)
                {//左翼持ちにまだ余裕がある状態
                    //Debug.Log("C2");
                    YlMoment = AerodynamicMomentum.x + YrMoment;
                }
                else
                {
                    //Debug.Log("D2");
                    YlMoment = YL * YlMax;
                }
            }
            //Debug.Log("YlMoment:"+YlMoment+"YrMoment:"+YrMoment+"aeroX:"+AerodynamicMomentum.x);
            //AerodynamicMomentum.x += YrMoment + YlMoment;//最終的なロールモーメントの計算//一旦消す
            GameManager.instance.TakeOff = false;
        }
        else
        {
            GameManager.instance.TakeOff = true;
            //PlaneRigidbody.constraints = RigidbodyConstraints.None;
        }
        //else if(GameManager.instance.FlightMode=="BirdmanRally" && !AddTaleForce){
        //    AddTaleForce =true;
        //}
        //Debug.Log(AerodynamicForce.z);
        PlaneRigidbody.AddRelativeForce(AerodynamicForce, ForceMode.Force);
        PlaneRigidbody.AddRelativeTorque(AerodynamicMomentum, ForceMode.Force);
        PlaneRigidbody.AddForce(TakeoffForce, ForceMode.Force);
        nz = AerodynamicForce.y / (PlaneRigidbody.mass * Physics.gravity.magnitude);
    }

    void CalculateRotation()
    {
        float q1 = GameManager.instance.Plane.transform.rotation.x;
        float q2 = -GameManager.instance.Plane.transform.rotation.y;
        float q3 = -GameManager.instance.Plane.transform.rotation.z;
        float q4 = GameManager.instance.Plane.transform.rotation.w;
        float C11 = q1 * q1 - q2 * q2 - q3 * q3 + q4 * q4;
        float C22 = -q1 * q1 + q2 * q2 - q3 * q3 + q4 * q4;
        float C12 = 2f * (q1 * q2 + q3 * q4);
        float C13 = 2f * (q1 * q3 - q2 * q4);
        float C32 = 2f * (q2 * q3 - q1 * q4);

        phi = -Mathf.Atan(-C32 / C22) * Mathf.Rad2Deg;
        theta = -Mathf.Asin(C12) * Mathf.Rad2Deg;
        psi = -Mathf.Atan(-C13 / C11) * Mathf.Rad2Deg;
    }

    /*
    public virtual void FlightModelStart()
    {
    }

    public virtual void FlightModelFixedUpdate()
    {
    }
    */
}
